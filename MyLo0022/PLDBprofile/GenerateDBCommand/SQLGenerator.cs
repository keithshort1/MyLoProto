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


namespace GenerateDBCommand
{
    public enum SQLGenerateRun
    {
        INNODB,
        MyISAM,
        Postgres
    }

    public partial class SQLGenerator
    {

        private IModelStore _ms;
        private IDatabaseCodeGenerator _py;
        private IDatabaseTableGenerator _mySql;

        private Dictionary<String, String> TableKeys = new Dictionary<String, String>();


        public SQLGenerator(StreamWriter fsSql, IDatabaseTableGenerator mySql, StreamWriter fsPy, IDatabaseCodeGenerator py, IModelStore ms)
        {
            _ms = ms;
            _py = py;
            _mySql = mySql;
            _py.AddStreamWriter(fsPy);
            _mySql.AddStreamWriter(fsSql);
        }

        public void GenerateMySQL()
        {
            FindAllPrimaryKeys();
            WriteTables();
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

        private void WriteTables()
        {
            _mySql.WriteHeader();
            _py.WriteHeader();

            foreach (IClass cls in _ms.AllInstances<IClass>().Where(cl => !cl.IsAbstract))
            {
                IEnumerable<IStereotypeInstance> tables = cls.AppliedStereotypes.Where(s => s.Name == "Table" || s.Name == "TableHierarchy");
                foreach (IStereotypeInstance table in tables)
                {
                    _mySql.Initialize();
                    _py.Initialize();

                    _mySql.WriteStructureStart(cls);
                    _py.WriteStructureStart(cls);

                    String pk = TableKeys[cls.Name];

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
                            _mySql.AddProperty(p);
                        }
                    }

                    foreach (IProperty ae in superRoles)
                    {
                        IAssociation association = ae.Association;

                        IEnumerable<IProperty> comps = association.MemberEnds.Where(s => s.IsComposite && s.Name == ae.Name);
                        foreach (IProperty pComp in comps)
                        {
                            _py.AddCompositeProperty(pComp);
                            _mySql.AddCompositeProperty(pComp);
                        }

                        IEnumerable<IStereotypeInstance> fks = association.AppliedStereotypes.Where(s => s.Name == "ForeignKey");
                        foreach (IStereotypeInstance fk in fks)
                        {
                            if (IsNavigableAssoc(ae) && fk != null)
                            {
                                _py.AddFKProperty(ae);
                                _mySql.AddFKProperty(ae);
                                _mySql.AddFKConstraint(association.Name, cls.Name, ae.Name, ae.Type.Name, TableKeys[ae.Type.Name], UmlHelper.GetDeleteAction(fk));
                            }
                        }

                        IEnumerable<IStereotypeInstance> ars = association.AppliedStereotypes.Where(s => s.Name == "AbstractReference");
                        foreach (IStereotypeInstance ar in ars)
                        {
                            if (IsNavigableAssoc(ae))
                            {
                                _mySql.AddAbstractReferenceProperty(ae);
                                _mySql.AddIndexConstraint(ae.Name + "AbstractRefIndex", ae.Name + "Id, " + ae.Name + "Table", "");
                                _py.AddAbstractReferenceProperty(ae);
                            }
                        }

                        IEnumerable<IStereotypeInstance> refs = association.AppliedStereotypes.Where(s => s.Name == "Reference");
                        foreach (IStereotypeInstance r in refs)
                        {
                            DealWithReflexiveReferences(ae, association);
                        }
                    }

                    if (table.Name == "TableHierarchy")
                    {
                        // aditional processing for Table Hierarchies
                        String childName = String.Empty;
                        String parentName = String.Empty;
                        String kindName = String.Empty;

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
                                    _mySql.AddFKProperty(p);
                                    _mySql.AddFKConstraint(association.Name, cls.Name, p.Name, p.Type.Name, TableKeys[p.Type.Name],"Delete");
                                }
                            }
                            IEnumerable<IStereotypeInstance> child = association.AppliedStereotypes.Where(s => s.Name == "Child");
                            foreach (IStereotypeInstance fk in child)
                            {
                                if (IsNavigableAssoc(p) && fk != null)
                                {
                                    childName = p.Name + "Id";
                                    _py.AddFKProperty(p);
                                    _mySql.AddFKProperty(p);
                                    _mySql.AddFKConstraint(association.Name, cls.Name, p.Name, p.Type.Name, TableKeys[p.Type.Name], "Delete");
                                }
                            }
                        }

                        _mySql.AddIndexConstraint(cls.Name + "Index", parentName + ", " + childName, "");

                        _mySql.AddSubtreeProcedure(cls.Name, kindName, parentName, childName);

                    }

                    _mySql.WriteStructureBody(cls, pk);
                    _py.WriteStructureBody(cls, pk);

                    _mySql.WriteStructureFinish(cls);
                    _py.WriteStructureFinish(cls);

                    _mySql.WriteSubtreeProcedure();

                }
            }

            _mySql.WriteTrailer();
        }


        private void DealWithReflexiveReferences(IProperty ae, IAssociation association)
        {
            if (IsReflexiveAssociation(association))
            {
                // UML tool arbitrarily picks an end as "outgoing" for reflexive associations
                // hence this workaround to pick the Navigable one
                if (IsNavigableAssoc(ae)) 
                {
                    _mySql.AddReferenceProperty(ae);
                    _py.AddReferenceProperty(ae);
                } 
                else 
                {
                    _mySql.AddReferenceProperty(ae.Opposite);
                    _py.AddReferenceProperty(ae.Opposite);
                }
            }
            else if (IsNavigableAssoc(ae))
            {
                _mySql.AddReferenceProperty(ae);
                _py.AddReferenceProperty(ae);
            }
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
