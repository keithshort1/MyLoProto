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
    public class PythonClassWriter : IDatabaseCodeGenerator
    {
        private StreamWriter _fsPy;

        struct FKConstraint
        {
            public String Name;
            public String TargetTable;
            public String TargetColumn;
            public String SourceColumn;
            public String DeleteAction;
        }

        private Dictionary<String, String> SqlPythonTypes = new Dictionary<String, String>();
        private List<FKConstraint> FKcons = new List<FKConstraint>();
        private String _currClassName;
        private List<IProperty> Props = new List<IProperty>();
        private List<IProperty> FKProps = new List<IProperty>();
        private List<IProperty> AbsProps = new List<IProperty>();
        private List<IProperty> PKProps = new List<IProperty>();
        private List<IProperty> RefProps = new List<IProperty>();
        private List<IProperty> MandatoryProps = new List<IProperty>();
        private List<IProperty> CompProps = new List<IProperty>();
        private String PKName;

        public PythonClassWriter()
        {
            InitializeSqlTypes();
        }

        public void AddStreamWriter(StreamWriter fsPy)
        {
            _fsPy = fsPy;
        }

        public void Initialize()
        {
            FKcons = new List<FKConstraint>();
            Props = new List<IProperty>();
            FKProps = new List<IProperty>();
            AbsProps = new List<IProperty>();
            PKProps = new List<IProperty>();
            MandatoryProps = new List<IProperty>();
            RefProps = new List<IProperty>();
            CompProps = new List<IProperty>();
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

        public void WriteFKConstraints()
        {
            //if (FKcons.Count != 0)
            //{
            //    int i = 0;
            //    foreach (FKConstraint f in FKcons)
            //    {
            //        i++;
            //        this.WriteFKConstraint(f.Name, f.SourceColumn, f.TargetColumn, f.TargetTable, f.DeleteAction);
            //    }
            //}
        }

        public void WriteStructureBody(IClass cls, string pkName)
        {
            this.PKName = pkName;
            this.WriteInitMethod(cls);
            this.WriteValidateMethod();
            this.WriteInsertMethod(cls);
        }

        private void WriteValidateMethod()
        {
            _fsPy.WriteLine("    def validate(self);");
            _fsPy.Write("        return not(");
            int i = 1;
            foreach (IProperty p in MandatoryProps)
            {  
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    string dataType = UmlHelper.GetDataType(column);
                    if (IsNumeric(dataType))
                    {
                        _fsPy.Write("self.{0} == 0 ", LowerCaseVarName(p.Name));
                        if (i < MandatoryProps.Count)
                        {
                            _fsPy.Write("or ");
                        }
                    }
                    else
                    {
                        _fsPy.Write("self.{0} == '' ", LowerCaseVarName(p.Name));
                        if (i < MandatoryProps.Count)
                        {
                            _fsPy.Write("or ");
                        }
                    }
                }
                i++;
            }
            _fsPy.WriteLine(")");
        }


        private void WriteInitMethod(IClass cls)
        {
            _fsPy.Write("    def __init__(self");
            foreach (IProperty p in Props)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (!String.IsNullOrEmpty(UmlHelper.GetNull(column)))
                    {
                        MandatoryProps.Add(p);
                    }
                    // We want to handle id's ourselves, so omit the id from the parameter list
                    if (p.Name != this.PKName)
                    {
                        WritePropertyWithDefault(p);
                    }
                                        
                }
            }
            foreach (IProperty p in FKProps)
            {
                    _fsPy.Write(", {0}Id=0", LowerCaseVarName(p.Name));
            }
            foreach (IProperty p in AbsProps)
            {
                    _fsPy.Write(", {0}Id=0", LowerCaseVarName(p.Name));
                    _fsPy.Write(", {0}Table=''", LowerCaseVarName(p.Name));
            }
            foreach (IProperty p in RefProps)
            {
                _fsPy.Write(", {0}Id=0", LowerCaseVarName(p.Name));
            }
            _fsPy.WriteLine("):"); 

            foreach (IProperty p in Props)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (p.Name == this.PKName)
                    {
                        this.WritePKProperty(p, column);
                    }
                    else
                    {
                        this.WriteProperty(p, column);
                    }
                }
            }
            foreach (IProperty p in FKProps)
            {
                WriteFKProperty(p);
            }
            foreach (IProperty p in AbsProps)
            {
                WriteAbstractReferenceProperty(p);
            }
            foreach (IProperty p in CompProps)
            {             
                _fsPy.WriteLine("        self.{0} = []", LowerCaseVarName(p.Name));
            }
            foreach (IProperty p in RefProps)
            {
                WriteReferenceProperty(p);
            }
            _fsPy.WriteLine("        {0}.current{1}Id += 1", cls.Name, cls.Name);
        }

        private void WritePropertyWithDefault(IProperty p)
        {
            IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
            foreach (IStereotypeInstance column in columns)
            {
                string defaultValue = UmlHelper.GetDefaultValue(p);
                if (String.IsNullOrEmpty(defaultValue))
                {
                    string dataType = UmlHelper.GetDataType(column);
                    if (IsNumeric(dataType))
                    {
                        _fsPy.Write(", {0}=0", LowerCaseVarName(p.Name));
                    }
                    else
                    {
                        _fsPy.Write(", {0}=''", LowerCaseVarName(p.Name));
                    }
                }
                else
                {
                    _fsPy.Write(", {0}={1}", LowerCaseVarName(p.Name), defaultValue);
                }
            }
        }


        public void WriteHeader()
        {
            DateTime now = DateTime.Now;
            _fsPy.WriteLine("import models sys");
            _fsPy.WriteLine("import models mysql.connector");
            _fsPy.WriteLine("import models random");
            _fsPy.WriteLine("import models datetime");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("#  Python Class definitions for Schrodinger Bluebird Project and Curated Protein Ligand Database");
            _fsPy.WriteLine("#  Schrodinger Inc. Confidential - All Rights Reserved");
            _fsPy.WriteLine("#  Generated on {0}", now.ToString());
            _fsPy.WriteLine("");
            _fsPy.WriteLine("_dbPLDB = mysql.connector.Connect(host='localhost', user='root', password='abc123', database='testmodel')");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("_cursorPLDB = _dbPLDB.cursor()");
            _fsPy.WriteLine("");
        }


        public void WriteStructureFinish(IClass cls)
        {
            _fsPy.WriteLine("    def get{0}Id(self):", cls.Name);
            _fsPy.WriteLine("        return self.{0}", LowerCaseVarName(PKName));
            _fsPy.WriteLine();
        }


        public void WriteStructureStart(IClass cls)
        {
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("# Class Definition for {0}", cls.Name);
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("  ");
            _fsPy.WriteLine("class {0}:", cls.Name);
            _fsPy.WriteLine("    current{0}Id = 1", cls.Name);
            _currClassName = cls.Name;
        }


        private void WriteInsertMethod(IClass cls)
        {
            _fsPy.WriteLine("    def insert(self):");
            _fsPy.WriteLine("        if self.validate():");
            _fsPy.Write("            insStr = '''INSERT INTO {0}(", cls.Name);

            // In Python, we don't care about type, so we use the fact that the Id is in the Props collection

            int numProps = Props.Count + FKProps.Count + AbsProps.Count * 2 + RefProps.Count;
            int numCols = 0;

            foreach (IProperty p in Props)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    numCols++;
                    if (numCols < numProps)
                    { _fsPy.Write("{0}, ", p.Name); }
                    else
                    { _fsPy.Write("{0}", p.Name); }
                }
            }
            foreach (IProperty p in FKProps)
            {
                numCols++;
                if (numCols < numProps)
                { _fsPy.Write("{0}Id, ", p.Name); }
                else
                { _fsPy.Write("{0}Id", p.Name); }

            }
            foreach (IProperty p in AbsProps)
            {
                numCols += 2;
                if (numCols < numProps)
                {
                    _fsPy.Write("{0}Id", p.Name);
                    _fsPy.Write(", {0}Table, ", p.Name);
                }
                else
                {
                    _fsPy.Write("{0}Id, ", p.Name);
                    _fsPy.Write("{0}Table", p.Name);
                }

            }
            foreach (IProperty p in RefProps)
            {
                numCols++;
                if (numCols < numProps)
                { _fsPy.Write("{0}Id, ", p.Name); }
                else
                { _fsPy.Write("{0}Id", p.Name); }

            }
            _fsPy.WriteLine(")");
            _fsPy.Write("                    VALUES(''' + '");
            int numParms = 0;
            while (numParms < numCols)
            {
                numParms++;
                if (numParms < numCols)
                {
                    _fsPy.Write("%s, ");
                }
                else
                {
                    _fsPy.Write("%s");
                }

            }
            _fsPy.Write(")' % (");
            int numVars = 0;
            foreach (IProperty p in Props)
            {
                numVars++;
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (numVars < numCols)
                    { _fsPy.Write("self.{0}, ", LowerCaseVarName(p.Name)); }
                    else
                    { _fsPy.Write("self.{0}", LowerCaseVarName(p.Name)); }

                }
            }
            foreach (IProperty p in FKProps)
            {
                numVars++;
                if (numVars < numCols)
                { _fsPy.Write("self.{0}Id, ", LowerCaseVarName(p.Name)); }
                else
                { _fsPy.Write("self.{0}Id", LowerCaseVarName(p.Name)); }

            }
            foreach (IProperty p in AbsProps)
            {
                numVars += 2;
                if (numVars < numCols)
                {
                    _fsPy.Write("self.{0}Id, ", LowerCaseVarName(p.Name));
                    _fsPy.Write("self.{0}Table, ", LowerCaseVarName(p.Name));
                }
                else
                {
                    _fsPy.Write("self.{0}Id, ", LowerCaseVarName(p.Name));
                    _fsPy.Write("self.{0}Table", LowerCaseVarName(p.Name));
                }

            }
            foreach (IProperty p in RefProps)
            {
                numVars++;
                if (numVars < numCols)
                { _fsPy.Write("self.{0}Id, ", LowerCaseVarName(p.Name)); }
                else
                { _fsPy.Write("self.{0}Id", LowerCaseVarName(p.Name)); }

            }
            _fsPy.WriteLine(")");
            _fsPy.WriteLine("            _cursorPLDB.execute(insStr)");

        }

        private void WriteAddItemMethods()
        {
            foreach (IProperty p in CompProps)
            {
                String className = LowerCaseVarName(p.Type.Name.ToString());
                _fsPy.WriteLine("    def add{0}(self, {1}):", p.Type.Name.ToString(), className);
                _fsPy.WriteLine("        self.{0}.append({1})", LowerCaseVarName(p.Name), className);
            }
        }


        private void WriteProperty(IProperty p, IStereotypeInstance column)
        {
            _fsPy.WriteLine("        self.{0} = {1}", LowerCaseVarName(p.Name), LowerCaseVarName(p.Name));
        }

        private void WritePKProperty(IProperty p, IStereotypeInstance column)
        {
            _fsPy.WriteLine("        self.{0} = {1}.current{2}Id", LowerCaseVarName(p.Name), _currClassName, _currClassName);
        }

        private void WriteFKProperty(IProperty p)
        {
            _fsPy.WriteLine("        self.{0}Id = {1}Id", LowerCaseVarName(p.Name), LowerCaseVarName(p.Name));
        }

        private void WriteReferenceProperty(IProperty p)
        {
            _fsPy.WriteLine("        self.{0}Id = {1}Id", LowerCaseVarName(p.Name), LowerCaseVarName(p.Name));
        }

        public void WriteAbstractReferenceProperty(IProperty p)
        {
            _fsPy.WriteLine("        self.{0}Id = {1}Id", LowerCaseVarName(p.Name), LowerCaseVarName(p.Name));
            _fsPy.WriteLine("        self.{0}Table = {1}Table", LowerCaseVarName(p.Name), LowerCaseVarName(p.Name));
        }

        private void WriteFKConstraint(string name, string sourceColumn, string targetColumn, string targetTable, string deleteAction)
        {
            throw new NotImplementedException();
        }

        private string BuildPythonDataType(IStereotypeInstance column)
        {
            string s = UmlHelper.GetDataType(column);
            string dt = String.Empty;
            if (s.Contains("()"))
            {
                // Special Processing for floats and decimals
                if (s == "float()" || s == "decimal()")
                {
                    string[] maxAndPlaces = UmlHelper.GetLength(column).ToString().Split(',');
                    dt = SqlPythonTypes[s] + String.Format("(max_digits={0}, decimal_places={1}, ", maxAndPlaces[0], maxAndPlaces[1]);
                }
                else
                {
                    dt = SqlPythonTypes[s] + String.Format("(max_length={0}, ", UmlHelper.GetLength(column).ToString());
                }
            }
            else
            {
                dt = SqlPythonTypes[s] + "(";
            }
            return dt;
        }


        private string LowerCaseVarName(string p)
        {
            return char.ToLower(p[0]) + p.Substring(1);
        }

        private void InitializeSqlTypes()
        {
            SqlPythonTypes.Add("datetime", "DateTimeField");
            SqlPythonTypes.Add("bigint", "BigIntegerField");
            SqlPythonTypes.Add("varchar", "CharField");
            SqlPythonTypes.Add("binary", "BinaryField");
            SqlPythonTypes.Add("blob", "FileField");
            SqlPythonTypes.Add("varbinary", "BinaryField");
            SqlPythonTypes.Add("date", "DateField");
            SqlPythonTypes.Add("decimal()", "DecimalField");
            SqlPythonTypes.Add("time", "TimeField");
            SqlPythonTypes.Add("text", "TextField");
            SqlPythonTypes.Add("xml", "XmlField");
            SqlPythonTypes.Add("varchar()", "CharField");
            SqlPythonTypes.Add("varbinary()", "BinaryField");
            SqlPythonTypes.Add("float()", "FloatField");
            SqlPythonTypes.Add("char()", "CharField");
            SqlPythonTypes.Add("int", "IntegerField");
            SqlPythonTypes.Add("tinyint", "PositiveSmallIntegerField");
            SqlPythonTypes.Add("image", "ImageField");
        }

        private bool IsNumeric(string dataType)
        {
            switch (dataType)
            {
                case "int": return true;
                case "bigint": return true;
                case "decimal()": return true;
                case "float()": return true;
                case "tinyint": return true;
                default: return false;
            }
        }

        public void WriteTrailer()
        {
            _fsPy.WriteLine();
        }
    }
}
