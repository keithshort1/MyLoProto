using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Uml.AuxiliaryConstructs;
using Microsoft.VisualStudio.Uml.Classes;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Presentation;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Uml;
using System.Data.Entity.Design.PluralizationServices;


namespace Generators
{
    public enum SQLGenerateRun
    {
        INNODB,
        MyISAM,
        Postgres
    }

    public class SQLGenerator
    {

        private IModelStore _ms;
        private IDatabaseCodeGenerator _py;
        private IDatabaseTableGenerator _sql;

        private Dictionary<String, String> TableKeys = new Dictionary<String, String>();


        public SQLGenerator(StreamWriter fsSql, IDatabaseTableGenerator mySql, StreamWriter fsPy, IDatabaseCodeGenerator py, IModelStore ms)
        {
            _ms = ms;
            _py = py;
            _sql = mySql;
            _py.AddStreamWriter(fsPy);
            _sql.AddStreamWriter(fsSql);
        }

        public void GenerateMySQL()
        {
            FindAllPrimaryKeys();
            WriteTables();
            WriteAlterTables();
            WriteTrailers();
        }

        private void FindAllPrimaryKeys()
        {
            foreach (IClass cls in _ms.AllInstances<IClass>().Where(cl => !cl.IsAbstract))
            {
                foreach (IProperty p in cls.Members)
                {
                    IEnumerable<IStereotypeInstance> cols = p.AppliedStereotypes.Where(s => s.Name == "Column");
                    foreach (IStereotypeInstance col in cols)
                    {
                        if (UmlHelper.IsPrimaryKey(col))
                        {
                            TableKeys.Add(cls.Name, p.Name);
                        }
                    }
                }
            }
        }

        private bool IgnoreClass(string name)
        {
            //return (name == "Folder" || name == "FolderHierarchy" || name == "FolderTemplate" ? true : false);
            return (name == "MyLoUser" || name == "UserProfile" || name == "Preferences" || name == "Photo" ? true : false);
        }

        private void WriteTables()
        {
            _sql.WriteHeader();
            _py.WriteHeader();

            foreach (IClass cls in _ms.AllInstances<IClass>().Where(cl => !cl.IsAbstract && !IgnoreClass(cl.Name)))
            //foreach (IClass cls in _ms.AllInstances<IClass>().Where(cl => !cl.IsAbstract))
            {
                IEnumerable<IStereotypeInstance> tables = cls.AppliedStereotypes.Where(s => s.Name == "Table" || 
                                                                                        s.Name == "HierarchyTable" || 
                                                                                        s.Name == "AssociationTable");
                foreach (IStereotypeInstance table in tables)
                {
                    String pk = TableKeys[cls.Name];
                    _sql.Initialize();
                    _py.Initialize();

                    _sql.WriteStructureStart(cls);
                    _py.WriteStructureStart(cls);

                    if (table.Name == "Table")
                    {
                        TableProcessing(table, cls);
                    }
                    else if (table.Name == "HierarchyTable")
                    {
                        HierarchyTableProcessing(table, cls);
                    }
                    // TODO Remove the following code in the next revision - TableHierarchy will be dropped
                    else if (table.Name == "TableHierarchy")
                    {
                        HierarchyTableProcessing(table, cls);
                    }
                    else if (table.Name == "AssociationTable")
                    {
                        AssociationTableProcessing(table, cls);
                    }

                    _sql.WriteStructureBody(cls, pk);
                    _py.WriteStructureBody(cls, pk);

                    _sql.WriteStructureFinish(cls);
                    _py.WriteStructureFinish(cls);

                    _sql.WriteSecureView(cls, pk);

                    _sql.WriteSubtreeProcedure();
                }
            }

        }


        private void WriteAlterTables()
        {
            _sql.WriteAlterTables();
        }

        private void WriteTrailers()
        {
            _sql.WriteTrailer();
            _py.WriteTrailer();
        }

        private void AssociationTableProcessing(IStereotypeInstance table, IClass cls)
        {
            foreach (IProperty p in cls.Members)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    _py.AddProperty(p);
                    _sql.AddProperty(p);
                }
            }

            List<string> unique = new List<string>();

            foreach (IProperty ae in cls.GetOutgoingAssociationEnds())
            {
                IAssociation association = ae.Association;

                IEnumerable<IProperty> comps = association.MemberEnds.Where(s => s.IsComposite && s.Name == ae.Name);
                foreach (IProperty pComp in comps)
                {
                    _py.AddCompositeProperty(pComp);
                    _sql.AddCompositeProperty(pComp);
                    unique.Add(pComp.Name);
                }

                IEnumerable<IStereotypeInstance> fks = association.AppliedStereotypes.Where(s => s.Name == "ForeignKey");
                foreach (IStereotypeInstance fk in fks)
                {
                    if (IsNavigableAssoc(ae) && fk != null)
                    {
                        unique.Add(ae.Name + "Id");
                        _py.AddFKProperty(ae);
                        _sql.AddFKProperty(ae);
                        _sql.AddFKConstraint(association.Name, cls.Name, ae.Name, ae.Type.Name, TableKeys[ae.Type.Name], UmlHelper.GetDeleteAction(fk));
                    }
                }

                IEnumerable<IStereotypeInstance> ars = association.AppliedStereotypes.Where(s => s.Name == "AbstractReference");
                foreach (IStereotypeInstance ar in ars)
                {
                    if (IsNavigableAssoc(ae))
                    {
                        _sql.AddAbstractReferenceProperty(ae);
                        _sql.AddIndexConstraint(ae.Name + "AbstractRefIndex", ae.Name + "Id, " + ae.Name + "Table", "");
                        _py.AddAbstractReferenceProperty(ae);
                        unique.Add(ae.Name + "Id, " + ae.Name + "Table");
                    }
                }

                IEnumerable<IStereotypeInstance> refs = association.AppliedStereotypes.Where(s => s.Name == "Reference");
                foreach (IStereotypeInstance r in refs)
                {
                    DealWithReflexiveReferences(ae, association);
                    unique.Add(ae.Name + "Id");
                }
            }

            _sql.AddUniqueConstraint(unique);
        }

        private void TableProcessing(IStereotypeInstance table, IClass cls)
        {
            List<IProperty> superRoles = new List<IProperty>();

            FindAllAssocsInSuperClasses(superRoles, cls.SuperClasses);

            foreach (IProperty p in cls.GetOutgoingAssociationEnds())
            {
                superRoles.Add(p);
            }

            foreach (IProperty p in cls.Members)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    _py.AddProperty(p);
                    _sql.AddProperty(p);
                }
            }

            foreach (IProperty ae in superRoles)
            {
                IAssociation association = ae.Association;

                IEnumerable<IProperty> comps = association.MemberEnds.Where(s => s.IsComposite && s.Name == ae.Name);
                foreach (IProperty pComp in comps)
                {
                    _py.AddCompositeProperty(pComp);
                    _sql.AddCompositeProperty(pComp);
                }

                IEnumerable<IStereotypeInstance> fks = association.AppliedStereotypes.Where(s => s.Name == "ForeignKey");
                foreach (IStereotypeInstance fk in fks)
                {
                    if (IsNavigableAssoc(ae) && fk != null)
                    {
                        _py.AddFKProperty(ae);
                        _sql.AddFKProperty(ae);
                        _sql.AddFKConstraint(association.Name, cls.Name, ae.Name, ae.Type.Name, TableKeys[ae.Type.Name], UmlHelper.GetDeleteAction(fk));
                    }
                }

                IEnumerable<IStereotypeInstance> ars = association.AppliedStereotypes.Where(s => s.Name == "AbstractReference");
                foreach (IStereotypeInstance ar in ars)
                {
                    if (IsNavigableAssoc(ae))
                    {
                        _sql.AddAbstractReferenceProperty(ae);
                        _sql.AddIndexConstraint(ae.Name + "AbstractRefIndex", ae.Name + "Id, " + ae.Name + "Table", "");
                        _py.AddAbstractReferenceProperty(ae);
                    }
                }

                IEnumerable<IStereotypeInstance> refs = association.AppliedStereotypes.Where(s => s.Name == "Reference");
                foreach (IStereotypeInstance r in refs)
                {
                    DealWithReflexiveReferences(ae, association);
                }
            }
        }

        private void HierarchyTableProcessing(IStereotypeInstance table, IClass cls)
        {
            String childName = String.Empty;
            String parentName = String.Empty;
            String kindName = String.Empty;

            foreach (IProperty p in cls.Members)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    _py.AddProperty(p);
                    _sql.AddProperty(p);
                }
            }

            foreach (IProperty p in cls.GetOutgoingAssociationEnds())
            {
                IAssociation association = p.Association;

                IEnumerable<IStereotypeInstance> parent = association.AppliedStereotypes.Where(s => s.Name == "Parent");
                foreach (IStereotypeInstance fk in parent)
                {
                    //kindName is the Table kind for which this table is a hierarchy
                    kindName = association.EndTypes.Where(s => s.Name != cls.Name).First().Name;
                    if (IsNavigableAssoc(p) && fk != null)
                    {
                        parentName = p.Name + "Id";
                        _py.AddFKProperty(p);
                        _sql.AddFKProperty(p);
                        _sql.AddFKConstraint(association.Name, cls.Name, p.Name, p.Type.Name, TableKeys[p.Type.Name], "Delete");
                    }
                }
                IEnumerable<IStereotypeInstance> child = association.AppliedStereotypes.Where(s => s.Name == "Child");
                foreach (IStereotypeInstance fk in child)
                {
                    if (IsNavigableAssoc(p) && fk != null)
                    {
                        childName = p.Name + "Id";
                        _py.AddFKProperty(p);
                        _sql.AddFKProperty(p);
                        _sql.AddFKConstraint(association.Name, cls.Name, p.Name, p.Type.Name, TableKeys[p.Type.Name], "Delete");
                    }
                }
            }

            _sql.AddIndexConstraint(cls.Name + "Index", parentName + ", " + childName, "");

            _sql.AddSubtreeProcedure(cls.Name, kindName, parentName, childName);
        }


        private void DealWithReflexiveReferences(IProperty ae, IAssociation association)
        {
            if (IsReflexiveAssociation(association))
            {
                // UML tool arbitrarily picks an end as "outgoing" for reflexive associations
                // hence this workaround to pick the Navigable one
                if (IsNavigableAssoc(ae)) 
                {
                    _sql.AddReferenceProperty(ae);
                    _py.AddReferenceProperty(ae);
                } 
                else 
                {
                    _sql.AddReferenceProperty(ae.Opposite);
                    _py.AddReferenceProperty(ae.Opposite);
                }
            }
            else if (IsNavigableAssoc(ae))
            {
                _sql.AddReferenceProperty(ae);
                _py.AddReferenceProperty(ae);
            }
        }


        private bool AnySuperClassNamed(IClass cls, string superClassName)
        {
            bool isResource;
            isResource = cls.SuperClasses.Any(x => x.Name == superClassName);
            if (!isResource)
            {
                foreach (IClass c in cls.SuperClasses)
                {
                    isResource = AnySuperClassNamed(c, superClassName);
                }
            }
            return isResource;
        }

        private bool IsNavigableAssoc(IProperty p)
        {
            return (p.Association.NavigableOwnedEnds.Contains(p));
        }

        private bool IsReflexiveAssociation(IAssociation a)
        {
            return (a.MemberEnds.First().Type == a.MemberEnds.Last().Type);
        }


        private void FindAllAssocsInSuperClasses(List<IProperty> roles, IEnumerable<IClass> superclasses)
        {
            foreach (IClass c in superclasses)
            {
                foreach (IProperty p in c.GetOutgoingAssociationEnds())
                {
                    roles.Add(p);
                }
                FindAllAssocsInSuperClasses(roles, c.SuperClasses);
            }
        }

        
    }
}
