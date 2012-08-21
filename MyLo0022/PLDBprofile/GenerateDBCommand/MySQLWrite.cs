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
    public class MySQLWriter : IDatabaseTableGenerator
    {
        private StreamWriter _fsSql;
        private SQLGenerateRun _run;

        List<SubtreeProcedure> SubtreeProcs = new List<SubtreeProcedure>();
        List<IndexConstraint> IndexCons = new List<IndexConstraint>();
        List<FKConstraint> FKcons = new List<FKConstraint>();
        private List<IProperty> Props = new List<IProperty>();
        private List<IProperty> FKProps = new List<IProperty>();
        private List<IProperty> AbsProps = new List<IProperty>();
        private List<IProperty> RefProps = new List<IProperty>();
        private List<IProperty> PKProps = new List<IProperty>();
        private List<IProperty> CompProps = new List<IProperty>();
        
        struct SubtreeProcedure
        {
            public String HierarchyTableName;
            public String TableName;
            public String ChildName;
            public String ParentName;
        }


        struct IndexConstraint
        {
            public String Name;
            public String TargetColumns;
            public String IndexType;
        }

        public struct FKConstraint
        {
            public String Name;
            public String TargetTable;
            public String TargetColumn;
            public String SourceColumn;
            public String DeleteAction;
        }

        public MySQLWriter(SQLGenerateRun run)
        {
            _run = run;
        }

        public void Initialize()
        {
            IndexCons = new List<IndexConstraint>();
            FKcons = new List<FKConstraint>();
            Props = new List<IProperty>();
            FKProps = new List<IProperty>();
            AbsProps = new List<IProperty>();
            PKProps = new List<IProperty>();
            RefProps = new List<IProperty>();
            CompProps = new List<IProperty>();
            SubtreeProcs = new List<SubtreeProcedure>();
        }

        public void AddStreamWriter(StreamWriter fsSql)
        {
            _fsSql = fsSql;
        }

        public void AddIndexConstraint(string indexName, string columns, string indexType)
        {
            IndexConstraint ic = new IndexConstraint();
            ic.Name = indexName;
            ic.TargetColumns = columns;
            ic.IndexType = indexType;
            IndexCons.Add(ic);
        }

        //public void AddTableHierarchy(string clsName, string tableHierachyName, string parentRole, string childRole)
        //{
        //    throw new NotImplementedException();
        //}

        public void AddProperty(IProperty p)
        {
            Props.Add(p);
        }

        public void AddAbstractReferenceProperty(IProperty p)
        {
            AbsProps.Add(p);
        }

        public void AddReferenceProperty(IProperty p)
        {
            RefProps.Add(p);
        }

        public void AddFKProperty(IProperty p)
        {
            FKProps.Add(p);
        }

        public void AddPKProperty(IProperty p)
        {
            PKProps.Add(p);
        }

        public void AddCompositeProperty(IProperty p)
        {
            CompProps.Add(p);
        }

        public void WriteStructureBody(IClass cls, string pkName)
        {
            this.WriteProperties(cls, pkName);
            this.WritePKConstraint(pkName);
            this.WriteIndexConstraints();
            this.WriteFKConstraints();
        
        }

        private void WriteProperties(IClass cls, string pk)
        {
            foreach (IProperty p in Props)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (p.Name == pk)
                    {
                        this.WritePKProperty(p, column);
                    }
                    else
                    {
                        this.WriteProperty(p, column);
                    }
                }
            }
            foreach (IProperty abs in AbsProps)
            {
                WriteAbstractReferenceProperty(abs);
            }
            foreach (IProperty r in RefProps)
            {
                WriteReferenceProperty(r);
            }
            foreach (IProperty fks in FKProps)
            {
                WriteFKProperty(fks);
            }
        }

        private void WriteIndexConstraints()
        {
            if (IndexCons.Count != 0)
            {
                int i = 0;
                foreach (IndexConstraint c in IndexCons)
                {
                    i++;
                    this.WriteIndexConstraint(c);
                }
            }
        }

        private void WriteFKConstraints()
        {
            if (FKcons.Count != 0 && _run == SQLGenerateRun.INNODB )
            {
                int i = 0;
                foreach (FKConstraint f in FKcons)
                {
                    i++;
                    //this.WriteFKConstraint(i, f);
                    this.WriteFKConstraint(f.Name, f.SourceColumn, f.TargetColumn, f.TargetTable, f.DeleteAction, i);
                }
            }
        }


        public void AddFKConstraint(String constraintName, String className, String sourceColName, String targetTableName, String targetColumn, String deleteAction)
        {
            FKConstraint fk = new FKConstraint();
            fk.TargetTable = targetTableName;
            fk.TargetColumn = targetColumn;
            fk.SourceColumn = sourceColName + "Id";
            fk.DeleteAction = (deleteAction == "NoAction") ? "No Action" : "Cascade";  // There seems to be a bug in UML that requires this workaround
            fk.Name = String.IsNullOrEmpty(constraintName) ? className + fk.SourceColumn + "_" + fk.TargetColumn : constraintName;
            FKcons.Add(fk);
        }


        public void WriteStructureFinish(IClass cls)
        {
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine(");");
            }
            else
            {
                _fsSql.WriteLine(") ENGINE={0};", _run == SQLGenerateRun.INNODB ? "INNODB" : "MyISAM");
                _fsSql.WriteLine("SET character_set_client = @saved_cs_client;");
            }
            _fsSql.WriteLine();
            _fsSql.WriteLine();
        }

        public void WriteStructureStart(IClass cls)
        {
            _fsSql.WriteLine("--");
            _fsSql.WriteLine("-- Table Structure for {0}", cls.Name);
            _fsSql.WriteLine("--");
            _fsSql.WriteLine("  ");
            _fsSql.WriteLine("Drop Table If Exists {0};", cls.Name);
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("  ");
            }
            else
            {
                _fsSql.WriteLine("SET @saved_cs_client     = @@character_set_client;  ");
                _fsSql.WriteLine("SET character_set_client = utf8;  ");
            }
            _fsSql.WriteLine("CREATE TABLE {0}(", cls.Name);
        }

        public void WriteHeader()
        {
            DateTime now = DateTime.Now;
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("-- Postgres 9.0.4 Database for Schrodinger Bluebird Project and Curated Protein Ligand Database");
            }
            else
            {
                _fsSql.WriteLine("-- MySQL Database for Schrodinger Bluebird Project and Curated Protein Ligand Database");
            }
            _fsSql.WriteLine("-- Schrodinger Inc. Confidential - All Rights Reserved");
            _fsSql.WriteLine("-- Schema Generated on {0}", now.ToString());
            _fsSql.WriteLine();
            if (_run == SQLGenerateRun.INNODB) 
            { 
                _fsSql.WriteLine("SET FOREIGN_KEY_CHECKS = 0; "); 
            } 
            else
            { 
                _fsSql.WriteLine(); 
            }
            _fsSql.WriteLine();
        }


        private string BuildDataType(IStereotypeInstance column)
        {
            string s = UmlHelper.GetDataType(column);
            StringBuilder dt = new StringBuilder(s);
            if (s.Contains("()"))
            {
                dt.Insert(dt.Length - 1, UmlHelper.GetLength(column));
            }
            return dt.ToString();
        }


        private void WriteProperty(IProperty p, IStereotypeInstance column)
        {
            _fsSql.WriteLine("    {0} {1} {2} {3},", p.Name, BuildDataType(column), UmlHelper.GetNull(column), UmlHelper.GetDefaultValue(p));
        }


        private void WritePKProperty(IProperty p, IStereotypeInstance column)
        {
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("    {0} bigserial {1} {2},", p.Name, UmlHelper.GetNull(column), UmlHelper.GetDefaultValue(p));
            }
            else
            {
                _fsSql.WriteLine("    {0} {1} {2} Auto_Increment {3},", p.Name, BuildDataType(column), UmlHelper.GetNull(column), UmlHelper.GetDefaultValue(p));
            }
        }


        private void WritePKConstraint(String pk)
        {
            if (_run == SQLGenerateRun.MyISAM)
            {
                _fsSql.WriteLine("    Primary Key ({0})", pk);
                  
            }
            else
            {
                _fsSql.WriteLine("    Primary Key ({0}){1}", pk, 0 == FKcons.Count ? String.Empty : ",");
            }
        }

        private void WriteReferenceProperty(IProperty ae)
        {
            _fsSql.WriteLine("    {0}Id bigInt,", ae.Name);
        }

        private void WriteAbstractReferenceProperty(IProperty ae)
        {
            _fsSql.WriteLine("    {0}Id bigInt not null,", ae.Name);
            _fsSql.WriteLine("    {0}Table varchar(56) not null,", ae.Name);
        }


        private void WriteFKProperty(IProperty ae)
        {
            _fsSql.WriteLine("    {0}Id bigInt,", ae.Name);
        }

        private void WriteFKConstraint(string name, string sourceColumn, string targetColumn, string targetTable, string deleteAction, int i)
        {

                _fsSql.WriteLine("    Constraint {0} Foreign Key ({1}) References {2} ({3}) On Delete {4}{5}",
                    name, sourceColumn, targetTable, targetColumn, deleteAction, i == FKcons.Count ? String.Empty : ",");
            
        }


        private void WriteIndexConstraint(IndexConstraint c)
        {
            _fsSql.WriteLine("    {0}Index {1} ({2}){3}",
                String.IsNullOrEmpty(c.IndexType) ? "" : c.IndexType + " ", c.Name, c.TargetColumns, FKcons.Count == 0 ? String.Empty : ",");
        }

        
        public void AddSubtreeProcedure(string hierachyTableName, string kindName, string parentName, string childName)
        {
            SubtreeProcedure sp = new SubtreeProcedure();
            sp.HierarchyTableName = hierachyTableName;
            sp.TableName = kindName;
            sp.ChildName = childName;
            sp.ParentName = parentName;
            SubtreeProcs.Add(sp);
        }


        public void WriteSubtreeProcedure()
        {
            foreach (SubtreeProcedure sp in SubtreeProcs)
            {
                _fsSql.WriteLine("--");
                _fsSql.WriteLine("-- Procedure for Building Subtrees of {0}", sp.TableName);
                _fsSql.WriteLine("--");
                _fsSql.WriteLine("  ");
                _fsSql.WriteLine("Drop Procedure If Exists {0}Subtree;", sp.TableName);
                _fsSql.WriteLine("DELIMITER //");
                _fsSql.WriteLine("CREATE PROCEDURE {0}Subtree( root bigint )", sp.TableName);
                _fsSql.WriteLine("BEGIN");
                _fsSql.WriteLine("  DROP TABLE IF EXISTS {0}Subtree;", sp.TableName);
                _fsSql.WriteLine("  CREATE TABLE {0}Subtree", sp.TableName);
                _fsSql.WriteLine("    SELECT {0}, {1}", sp.ParentName, sp.ChildName);
                _fsSql.WriteLine("    FROM {0}", sp.HierarchyTableName);
                _fsSql.WriteLine("    WHERE {0} = root;", sp.ParentName);
                _fsSql.WriteLine("    ALTER TABLE {0}Subtree ADD PRIMARY KEY ({1}, {2});", sp.TableName, sp.ParentName, sp.ChildName);
                _fsSql.WriteLine("  REPEAT");
                _fsSql.WriteLine("    INSERT IGNORE INTO {0}Subtree", sp.TableName);
                _fsSql.WriteLine("      SELECT p.{0}, p.{1}", sp.ParentName, sp.ChildName);
                _fsSql.WriteLine("      FROM {0} as p", sp.HierarchyTableName);
                _fsSql.WriteLine("      INNER JOIN {0}Subtree as ps ON p.{1} = ps.{2};", sp.TableName, sp.ParentName, sp.ChildName);
                _fsSql.WriteLine("  UNTIL Row_Count() = 0 END REPEAT;");
                _fsSql.WriteLine("END //");
                _fsSql.WriteLine("DELIMITER ;  ");
                _fsSql.WriteLine("  ");
                _fsSql.WriteLine("  ");
            }
        }

        public void WriteTrailer()
        {
            if (_run == SQLGenerateRun.INNODB) 
            { 
                _fsSql.WriteLine("SET FOREIGN_KEY_CHECKS = 1; "); 
            } 
            else 
            { 
                _fsSql.WriteLine(); 
            }
        }
    }

}
