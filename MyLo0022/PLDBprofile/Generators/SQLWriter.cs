
// Schrodinger Inc. Confidential - All Rights Reserved

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
    public class SQLWriter : IDatabaseTableGenerator
    {
        private StreamWriter _fsSql;
        private SQLGenerateRun _run;
        private int _numCons;
        private int _consWritten;
        private string PKName;

        List<UniqueConstraint> UniqueConstraints = new List<UniqueConstraint>();
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

        struct UniqueConstraint
        {
            public List<string> Columns;
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
            public String SourceTable;
            public String SourceColumn;
            public String DeleteAction;
        }

        public SQLWriter(SQLGenerateRun run)
        {
            _run = run;
            FKcons = new List<FKConstraint>();
        }

        public void Initialize()
        {
            IndexCons = new List<IndexConstraint>();
            if (_run != SQLGenerateRun.Postgres)
            {
                FKcons = new List<FKConstraint>();
            }
            Props = new List<IProperty>();
            FKProps = new List<IProperty>();          
            AbsProps = new List<IProperty>();
            PKProps = new List<IProperty>();
            RefProps = new List<IProperty>();
            CompProps = new List<IProperty>();
            SubtreeProcs = new List<SubtreeProcedure>();
            UniqueConstraints = new List<UniqueConstraint>();
            _numCons = 0;
            _consWritten = 0;
        }

        public void AddStreamWriter(StreamWriter fsSql)
        {
            _fsSql = fsSql;
        }

        public void WriteAlterTables()
        {
            if (_run == SQLGenerateRun.Postgres && FKcons.Count != 0)
            {
                WriteAlterTablesFKs();
            }
        }

        public void WriteHeader()
        {
            DateTime now = DateTime.Now;
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("-- Postgres 9.0.4 Database for MyLo Store");
            }
            else
            {
                _fsSql.WriteLine("-- MySQL Database for MyLo Store");
            }
            _fsSql.WriteLine("-- MyLo Inc. Confidential - All Rights Reserved");
            _fsSql.WriteLine("-- Schema Generated on {0}", now.ToString());
            _fsSql.WriteLine();
            if (_run == SQLGenerateRun.INNODB)
            {
                _fsSql.WriteLine("SET FOREIGN_KEY_CHECKS = 0; ");
            }
            else if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("BEGIN; ");
            }
            else
            {
                _fsSql.WriteLine();
            }
            _fsSql.WriteLine();
        }


        public void AddIndexConstraint(string indexName, string columns, string indexType)
        {
            IndexConstraint ic = new IndexConstraint();
            ic.Name = indexName;
            ic.TargetColumns = columns;
            ic.IndexType = indexType;
            IndexCons.Add(ic);
        }

        public void AddUniqueConstraint(List<string> columns)
        {
            UniqueConstraint uc = new UniqueConstraint();
            uc.Columns = columns;
            UniqueConstraints.Add(uc);
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
            this.PKName = pkName;
            if (_run == SQLGenerateRun.Postgres)
            {
                _numCons = UniqueConstraints.Count;
            }
            else
            {
                _numCons = FKcons.Count + UniqueConstraints.Count + IndexCons.Count;
            }
            
            this.WriteProperties(cls, pkName);
            this.WritePKConstraint(pkName);
            if (_run == SQLGenerateRun.INNODB || _run == SQLGenerateRun.MyISAM)
            {
                this.WriteInlineIndexConstraints();
                this.WriteInlineFKConstraints();
            }
            
            this.WriteUniqueConstraints();
        
        }

        private void WriteIndexConstraints(string tableName)
        {
            if (IndexCons.Count != 0)
            {
                int i = 0;
                foreach (IndexConstraint c in IndexCons)
                {
                    i++;
                    this.WriteIndexConstraint(c, tableName);
                }
            }
        }

        private void WriteInlineFKConstraints()
        {
            if (FKcons.Count != 0 && _run == SQLGenerateRun.INNODB)
            {
                foreach (FKConstraint f in FKcons)
                {
                    //this.WriteFKConstraint(i, f);
                    this.WriteInlineFKConstraint(f.Name, f.SourceColumn, f.TargetColumn, f.TargetTable, f.DeleteAction);
                }
            }
        }

        private void WriteAlterTablesFKs()
        {
            if (FKcons.Count != 0 && _run != SQLGenerateRun.MyISAM)
            {
                foreach (FKConstraint f in FKcons)
                {
                    this.WriteAlterTablesFKConstraint(f.Name, f.SourceTable, f.SourceColumn, f.TargetColumn, f.TargetTable, f.DeleteAction);
                }
            }
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
            bool isResource = AnySuperClassNamed(cls, "Resource");
            if (!isResource)
            {
                //Hack to avoid problems with MyLoUser
                if (cls.Name != "MyLoUser")
                {
                    _fsSql.WriteLine("    {0} bigInt not null,", "MyLoAccountId");
                }
                // TODO decide whether this is checked by Insert or uses a FK
                //FKConstraint fk = new FKConstraint();
                //fk.SourceTable = cls.Name + "_Base";
                //fk.SourceColumn = "MyLoAccountId";
                //fk.TargetColumn = "MyLoAccountId";
                //fk.TargetTable = "MyLoUser_Base";
                //fk.DeleteAction = "Cascade";
                //fk.Name = cls.Name + "MyLoAccountId";
                //FKcons.Add(fk);
            }
        }

        private void WriteInlineIndexConstraints()
        {
            if (IndexCons.Count != 0)
            {
                int i = 0;
                foreach (IndexConstraint c in IndexCons)
                {
                    i++;
                    this.WriteInlineIndexConstraint(c);
                }
            }
        }

        private void WriteUniqueConstraints()
        {
            if (UniqueConstraints.Count != 0)
            {
                int i = 0;
                foreach (UniqueConstraint c in UniqueConstraints)
                {
                    i++;
                    this.WriteUniqueConstraint(c);
                }
            }
        }


        //private void WriteAlterTablesFK()
        //{
        //    if (FKcons.Count != 0 && _run != SQLGenerateRun.MyISAM)
        //    {
        //        foreach (FKConstraint f in FKcons)
        //        {
        //            //this.WriteFKConstraint(i, f);
        //            this.WriteAlterTablesFKConstraint(f.Name, f.SourceTable, f.SourceColumn, f.TargetColumn, f.TargetTable, f.DeleteAction);
        //        }
        //    }
        //}


        public void AddFKConstraint(String constraintName, String className, String sourceColName, String targetTableName, String targetColumn, String deleteAction)
        {
            FKConstraint fk = new FKConstraint();
            //fk.TargetTable = targetTableName + (targetTableName == "Folder" ? String.Empty : "_Base");
            fk.TargetTable = targetTableName + "_Base";
            fk.TargetColumn = targetColumn;
            fk.SourceTable = className + "_Base";
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
                this.WriteIndexConstraints(cls.Name);
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
            
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("DROP TABLE IF EXISTS {0}_Base CASCADE;", cls.Name);
                _fsSql.WriteLine("  ");
            }
            else
            {
                _fsSql.WriteLine("DROP TABLE IF EXISTS {0}_Base;", cls.Name);
                _fsSql.WriteLine("SET @saved_cs_client     = @@character_set_client;  ");
                _fsSql.WriteLine("SET character_set_client = utf8;  ");
            }
            _fsSql.WriteLine("CREATE TABLE {0}_Base(", cls.Name);

        }


        public void WriteSecureView(IClass cls, string PKName)
        {
            if (_run != SQLGenerateRun.Postgres)
                return;

            _fsSql.WriteLine("--");
            _fsSql.WriteLine("-- Modifiable View Structure for {0}", cls.Name);
            _fsSql.WriteLine("--");
            _fsSql.WriteLine("  ");
            _fsSql.WriteLine("DROP SEQUENCE IF EXISTS {0}Sequence;", cls.Name);
            _fsSql.WriteLine("  ");
            _fsSql.WriteLine("CREATE SEQUENCE {0}Sequence;", cls.Name);
            _fsSql.WriteLine("  ");
            _fsSql.WriteLine("DROP VIEW IF EXISTS {0} CASCADE;", cls.Name);
            _fsSql.WriteLine("  ");
            _fsSql.WriteLine("CREATE VIEW {0} AS", cls.Name);

            bool isResource = AnySuperClassNamed(cls,"Resource");
            if (isResource)
            {
                _fsSql.Write("        SELECT  ");
            }
            else
            {
                _fsSql.Write("        SELECT  v.MyLoAccountId, ");
            }
            //int numProps = Props.Count + FKProps.Count + AbsProps.Count * 2 + RefProps.Count - 1;
            int numProps = Props.Count + FKProps.Count + AbsProps.Count * 2 + RefProps.Count;
            // We subtract 1 to allow for the fact tha we'll skip the Primary Key column
            int numCols = 0;

            //foreach (IProperty p in Props.Where(k => k.Name != PKName))
            foreach (IProperty p in Props)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    numCols++;
                    if (numCols < numProps)
                    { _fsSql.Write("v.{0}, ", p.Name); }
                    else
                    { _fsSql.Write("v.{0}", p.Name); }
                }
            }
            foreach (IProperty p in FKProps)
            {
                numCols++;
                if (numCols < numProps)
                { _fsSql.Write("v.{0}Id, ", p.Name); }
                else
                { _fsSql.Write("v.{0}Id", p.Name); }

            }
            foreach (IProperty p in AbsProps)
            {
                numCols += 2;
                if (numCols < numProps)
                {
                    _fsSql.Write("v.{0}Id", p.Name);
                    _fsSql.Write(", v.{0}Table, ", p.Name);
                }
                else
                {
                    _fsSql.Write("v.{0}Id, ", p.Name);
                    _fsSql.Write("v.{0}Table", p.Name);
                }

            }
            foreach (IProperty p in RefProps)
            {
                numCols++;
                if (numCols < numProps)
                { _fsSql.Write("v.{0}Id, ", p.Name); }
                else
                { _fsSql.Write("v.{0}Id", p.Name); }

            }
            _fsSql.WriteLine("");

            _fsSql.WriteLine("                FROM {0}_Base AS v;", cls.Name);
            //_fsSql.WriteLine("                INNER JOIN ReadableFoldersRecursive() AS RFR ON v.MyLoAccountId = RFR.Id;");
            _fsSql.WriteLine("");
            _fsSql.WriteLine("CREATE OR REPLACE FUNCTION {0}InsteadOfInsertProc(NEW {1}) RETURNS bigint AS $$", cls.Name, cls.Name);
            _fsSql.WriteLine("      DECLARE");
            _fsSql.WriteLine("              _error        boolean;");
            _fsSql.WriteLine("              _{0}          bigint;", PKName);
            _fsSql.WriteLine("              _mId          bigint;");
            if (isResource)
            {
                _fsSql.WriteLine("              _resId        bigint;");
                _fsSql.WriteLine("              _nsPrefId     bigint;");
                _fsSql.WriteLine("              _uriPath      text;");
                // Added _uriQuery told hold dynamically create UriQuery string in case UriQuery was not passed into procedure
                _fsSql.WriteLine("              _uriQuery     varchar(256);");
            }
            _fsSql.WriteLine("      BEGIN");
            _fsSql.WriteLine("              _{0} := nextval('{1}Sequence');", PKName, cls.Name);
            if (isResource)
            {
                // If UriQuery is not defined create it as class name and primary key
                _fsSql.WriteLine("              IF NEW.UriQuery IS NULL THEN");
                _fsSql.WriteLine("                      _uriQuery := '{0}' || '=' || _Id::varchar(256);", cls.Name);
                _fsSql.WriteLine("              ELSE");
                _fsSql.WriteLine("                      _uriQuery := NEW.UriQuery;");
                _fsSql.WriteLine("              END IF;");
                _fsSql.WriteLine("");
                _fsSql.WriteLine("              _resId := nextval('ResourcesSequence');");
            }
            //_fsSql.WriteLine("              _error := NEW.MyLoAccountId NOT IN (SELECT * FROM WritableFoldersRecursive());");
            //_fsSql.WriteLine("              IF _error THEN");
            //_fsSql.WriteLine("                      RAISE EXCEPTION 'Access Violation on Folder %', NEW.MyLoAccountId;");
            //_fsSql.WriteLine("              ELSE");
            _fsSql.WriteLine("              _mId := (SELECT M.MyLoAccountId FROM MyLoUser_Base AS M WHERE M.MyLoAccountId = NEW.MyLoAccountId);");
            _fsSql.WriteLine("              IF _mId IS NULL THEN");
            _fsSql.WriteLine("                      RAISE EXCEPTION 'MyLoAccountId is % unknown', NEW.MyLoAccountId;");
            _fsSql.WriteLine("              ELSE");
            if (isResource)
            {
                _fsSql.WriteLine("                      INSERT INTO {0}_Base ({1}, {2})", cls.Name, PKName, CommaListProps(PKName, ""));
                _fsSql.WriteLine("                              VALUES (_{0}, {1});", PKName, CommaListProps(PKName, "NEW."));               
                _fsSql.WriteLine("                      _uriPath := PathFromFolderId(NEW.MyLoAccountId);");
                _fsSql.WriteLine("                      _nsPrefId := GetOrInsertNamespacePrefix(NEW.MyLoAccountId, _uriPath);");
                _fsSql.WriteLine("                      INSERT INTO Resources (Id, NsPrefId, Entity, Name, EntityId, UriQuery, LoadDate, MyLoAccountId)");
                _fsSql.WriteLine("                           VALUES (_resId, _nsPrefId, '{0}', nameFromQuery(_uriQuery), _{1}, _uriQuery, now(), NEW.MyLoAccountId);", cls.Name, PKName);
            }
            else
            {
                _fsSql.WriteLine("                      INSERT INTO {0}_Base ({1}, {2}, {3})", cls.Name, "MyLoAccountId", PKName, CommaListProps(PKName, ""));
                _fsSql.WriteLine("                              VALUES ({0}, _{1}, {2});", "NEW.MyLoAccountId", PKName, CommaListProps(PKName, "NEW."));
            }
            _fsSql.WriteLine("              END IF;");
            _fsSql.WriteLine("              RETURN _{0};", PKName);
            _fsSql.WriteLine("      END;");
            _fsSql.WriteLine("$$ LANGUAGE plpgsql;");
            _fsSql.WriteLine("");
            _fsSql.WriteLine("CREATE OR REPLACE RULE {0}InsteadOfInsert AS ON INSERT TO {1}", cls.Name, cls.Name);
            _fsSql.WriteLine("      DO INSTEAD");
            _fsSql.WriteLine("      SELECT {0}InsteadOfInsertProc(NEW);", cls.Name);
            _fsSql.WriteLine("");
            _fsSql.WriteLine("");
        }

        private string CommaListProps(string PK, string prefix)
        {
            string resultStr = String.Empty;
            List<String> allPropNames = new List<String>();
            foreach (IProperty p in Props)
            {
                if (p.Name != PK)
                {
                    allPropNames.Add(p.Name);
                }
            }
            foreach (IProperty fkp in FKProps)
            {
                allPropNames.Add(fkp.Name + "Id");
            }
            foreach (IProperty abp in AbsProps)
            {
                allPropNames.Add(abp.Name + "Id");
                allPropNames.Add(abp.Name + "Table");
            }
            foreach (IProperty refp in RefProps)
            {
                allPropNames.Add(refp.Name + "Id");
            }
            int i = 0;
            foreach (String p in allPropNames)
            {
                i++;
                if (i == allPropNames.Count)
                {
                    resultStr = resultStr + prefix + p;
                }
                else
                {
                    resultStr = resultStr + prefix + p + ", ";
                }
            }
            return resultStr;
        }


        private string BuildDataType(IStereotypeInstance column)
        {
            // TODO  - rewrite this using dictionaries for the various DBs
            // and map to datatypes allowed in DSL
            string s = UmlHelper.GetDataType(column);
            StringBuilder dt = new StringBuilder(s);
            if (_run == SQLGenerateRun.Postgres)
            {
                if (s == "float()")
                {
                    return "real";
                }
                else if (s == "datetime")
                {
                    return "timestamp";
                }
                else if (s == "datetimezone")
                {
                    return "timestamp with time zone";
                }
                else if (s == "blob")
                {
                    return "bytea";
                }
                else if (s == "double")
                {
                    return "double precision";
                }
                else if (s == "varbinary()")
                {
                    return "boolean";
                }
                else if (s == "tinyint")
                {
                    return "smallint";
                }
                else if (s == "text")
                {
                    return "text";
                }
                else if (s == "guid")
                {
                    return "uuid";
                }
                else if (s.Contains("()"))
                {
                    dt.Insert(dt.Length - 1, UmlHelper.GetLength(column));
                    return dt.ToString();
                }
                else
                {
                    return dt.ToString();
                }
            }
            else if (_run == SQLGenerateRun.INNODB || _run == SQLGenerateRun.MyISAM)
            {
                if (s.Contains("()"))
                {
                    dt.Insert(dt.Length - 1, UmlHelper.GetLength(column));
                    return dt.ToString();
                }
                return dt.ToString();
            }
            else
            {
                return "";
            }
        }


        private void WriteProperty(IProperty p, IStereotypeInstance column)
        {
            _fsSql.WriteLine("    {0} {1} {2} {3},", p.Name, BuildDataType(column), UmlHelper.GetNull(column), UmlHelper.GetDefaultValue(p));
        }


        private void WritePKProperty(IProperty p, IStereotypeInstance column)
        {
            if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("    {0} bigint {1} {2},", p.Name, UmlHelper.GetNull(column), UmlHelper.GetDefaultValue(p));
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
                _fsSql.WriteLine("    Primary Key ({0}){1}", pk, _consWritten == _numCons ? String.Empty : ",");
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
            _fsSql.WriteLine("    {0}Id bigInt not null,", ae.Name);
        }

        private void WriteInlineFKConstraint(string name, string sourceColumn, string targetColumn, string targetTable, string deleteAction)
        {
            _consWritten += 1;
            _fsSql.WriteLine("    Constraint {0} Foreign Key ({1}) References {2} ({3}) On Delete {4}{5}",
                    name, sourceColumn, targetTable, targetColumn, deleteAction, _consWritten == _numCons ? String.Empty : ",");    
        }


        private void WriteAlterTablesFKConstraint(string name, string sourceTable, string sourceColumn, string targetColumn, string targetTable, string deleteAction)
        {
            _fsSql.WriteLine("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) REFERENCES {3} ({4}) On Delete {5};",
                    sourceTable, name, sourceColumn, targetTable, targetColumn, deleteAction);
        }


        private void WriteInlineIndexConstraint(IndexConstraint c)
        {
            _consWritten += 1;
            _fsSql.WriteLine("    {0}Index {1} ({2}){3}",
            String.IsNullOrEmpty(c.IndexType) ? "" : c.IndexType + " ", c.Name, c.TargetColumns, _consWritten == _numCons ? String.Empty : ",");  
        }

        private void WriteUniqueConstraint(UniqueConstraint c)
        {
            if (UniqueConstraints.Count != 0)
            {
                _consWritten += 1;
                string uniqueNames = flattenList(c.Columns);
                _fsSql.WriteLine("    UNIQUE ({0}){1}", uniqueNames, _consWritten == _numCons ? String.Empty : ",");  
            }
        }

        private void WriteIndexConstraint(IndexConstraint c, string tableName)
        {
            if (FKcons.Count != 0)
            {
                _fsSql.WriteLine("");
                _fsSql.WriteLine("DROP INDEX IF EXISTS {0};", c.Name);
                _fsSql.WriteLine("");
                _fsSql.WriteLine("CREATE {0}INDEX {1} on {2}_Base ({3});",
                    String.IsNullOrEmpty(c.IndexType) ? "" : c.IndexType + " ", c.Name, tableName, c.TargetColumns);
            }
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
            if (_run != SQLGenerateRun.Postgres)
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
        }

        public void WriteTrailer()
        {
            if (_run == SQLGenerateRun.INNODB) 
            { 
                _fsSql.WriteLine("SET FOREIGN_KEY_CHECKS = 1; "); 
            }
            else if (_run == SQLGenerateRun.Postgres)
            {
                _fsSql.WriteLine("COMMIT; ");
            }
            else
            {
                _fsSql.WriteLine();
            }
        }

        private string flattenList(List<string> list)
        {
            int i = 0;
            string temp = String.Empty;
            foreach (string s in list)
            {
                temp += s; i++;
                if (i != list.Count) { temp += ", "; }
            }
            return temp;
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


        private string LowerCaseVarName(string p)
        {
            return char.ToLower(p[0]) + p.Substring(1);
        }

    }

}
