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
        private String pythonClsName;

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
            this.WriteValidateMethod(cls);
            this.WriteInsertMethod(cls);
            //this.WriteGetByIdMethod(cls);
        }


        private void WriteValidateMethod(IClass cls)
        {
            _fsPy.WriteLine("    def validate(self, {0}):", pythonClsName);
            _fsPy.Write("        return not(");
            int i = 1;
            foreach (IProperty p in MandatoryProps.Where(k => k.Name != PKName))
            {  
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                if (columns.Count() != 0)
                {
                    foreach (IStereotypeInstance column in columns)
                    {
                        string dataType = UmlHelper.GetDataType(column);
                        if (IsNumeric(dataType))
                        {
                            _fsPy.Write("{0}.{1} == 0 ", pythonClsName, LowerCaseVarName(p.Name));
                        }
                        else
                        {
                            _fsPy.Write("{0}.{1} == '' ", pythonClsName, LowerCaseVarName(p.Name));
                        }
                    }
                }
                else
                {
                    // If the p is in MandatoryProps but not of stereotype Column, it must be an FK prop
                    _fsPy.Write("{0}.{1}Id == 0 ", pythonClsName, LowerCaseVarName(p.Name));
                }
                if (i < MandatoryProps.Count - 1)  // We subtract 1 to allow for the fact that the Primary Key is omitted here
                {
                    _fsPy.Write("or ");
                }
                i++;
            }
            _fsPy.WriteLine(")");
            _fsPy.WriteLine("");
        }


        private void WriteInitMethod(IClass cls)
        {
            _fsPy.WriteLine("    def __init__(self, cursor):");
            _fsPy.WriteLine("        self.cursor  = cursor");
            _fsPy.WriteLine("");

            foreach (IProperty p in Props)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (!String.IsNullOrEmpty(UmlHelper.GetNull(column)))
                    {
                        MandatoryProps.Add(p);
                    }
                }
            }
            foreach (IProperty p in FKProps)
            {
                MandatoryProps.Add(p);
            }
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
            _fsPy.WriteLine("import sys");
            _fsPy.WriteLine("import psycopg2");
            _fsPy.WriteLine("import datetime");
            _fsPy.WriteLine("import logging");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("#  Python Class definitions for Schrodinger Bluebird Project and Curated Protein Ligand Database");
            _fsPy.WriteLine("#  Schrodinger Inc. Confidential - All Rights Reserved");
            _fsPy.WriteLine("#  Generated on {0}", now.ToString());
            _fsPy.WriteLine("");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("log = logging.getLogger(':')");
            _fsPy.WriteLine("ch = logging.StreamHandler()");
            _fsPy.WriteLine("formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')");
            _fsPy.WriteLine("ch.setFormatter(formatter)");
            _fsPy.WriteLine("log.addHandler(ch)");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("log.setLevel(logging.INFO)");
            _fsPy.WriteLine("");
        }


        public void WriteStructureFinish(IClass cls)
        {
            _fsPy.WriteLine();
            WriteTranferClass(cls);
        }


        public void WriteStructureStart(IClass cls)
        {
            pythonClsName = LowerCaseVarName(cls.Name);
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("# Data Access Class Definition for {0}", pythonClsName);
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("  ");
            _fsPy.WriteLine("class {0}Access:", pythonClsName);
            _currClassName = cls.Name;
        }


        private void WriteInsertMethod(IClass cls)
        {
            _fsPy.WriteLine("    def insert(self, {0}):", pythonClsName);
            _fsPy.WriteLine("        if self.validate({0}):", pythonClsName);
            _fsPy.WriteLine("            try:");
            _fsPy.Write("                insStr = '''INSERT INTO {0}(", pythonClsName);

            int numProps = Props.Count + FKProps.Count + AbsProps.Count * 2 + RefProps.Count - 1;
            // We subtract 1 to allow for the fact tha we'll skip the Primary Key column
            int numCols = 0;

            foreach (IProperty p in Props.Where(k => k.Name != this.PKName))
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
            _fsPy.Write("                        VALUES(''' + '");
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
            //_fsPy.Write(")' % (");
            _fsPy.WriteLine(");'''");
            _fsPy.Write("                data = (");
            int numVars = 0;
            foreach (IProperty p in Props.Where(k => k.Name != this.PKName))
            {
                numVars++;
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (numVars < numCols)
                    { _fsPy.Write("{0}.{1}, ", pythonClsName, LowerCaseVarName(p.Name)); }
                    else
                    { _fsPy.Write("{0}.{1}", pythonClsName, LowerCaseVarName(p.Name)); }

                }
            }
            foreach (IProperty p in FKProps)
            {
                numVars++;
                if (numVars < numCols)
                { _fsPy.Write("{0}.{1}Id, ", pythonClsName, LowerCaseVarName(p.Name)); }
                else
                { _fsPy.Write("{0}.{1}Id", pythonClsName, LowerCaseVarName(p.Name)); }

            }
            foreach (IProperty p in AbsProps)
            {
                numVars += 2;
                if (numVars < numCols)
                {
                    _fsPy.Write("{0}.{1}Id, ", pythonClsName, LowerCaseVarName(p.Name));
                    _fsPy.Write("{0}.{1}Table, ", pythonClsName, LowerCaseVarName(p.Name));
                }
                else
                {
                    _fsPy.Write("{0}.{1}Id, ", pythonClsName, LowerCaseVarName(p.Name));
                    _fsPy.Write("{0}.{1}Table", pythonClsName, LowerCaseVarName(p.Name));
                }

            }
            foreach (IProperty p in RefProps)
            {
                numVars++;
                if (numVars < numCols)
                { _fsPy.Write("{0}.{1}Id, ", pythonClsName, LowerCaseVarName(p.Name)); }
                else
                { _fsPy.Write("{0}.{1}Id", pythonClsName, LowerCaseVarName(p.Name)); }

            }
            _fsPy.WriteLine(")");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("                log.debug(insStr)");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("                self.cursor.execute(insStr, data)");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("                results = self.cursor.fetchall()");
            _fsPy.WriteLine("                for result in results:");
            _fsPy.WriteLine("                    {0}.{1} = result[0]", pythonClsName, LowerCaseVarName(PKName));
            _fsPy.WriteLine("                return {0}.{1}", pythonClsName, LowerCaseVarName(PKName));
            _fsPy.WriteLine("");
            _fsPy.WriteLine("            except Exception, e:");
            _fsPy.WriteLine("                raise Exception(e)");
            _fsPy.WriteLine("        else:");
            _fsPy.WriteLine("            raise Exception('Validation failed for {0}')", pythonClsName);
            _fsPy.WriteLine("");
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


        private void WriteTranferClass(IClass cls)
        {
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("# Class Definition for Transfer Class {0}", pythonClsName);
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("  ");
            _fsPy.WriteLine("class {0}:", pythonClsName);
            _fsPy.WriteLine("  ");
            _currClassName = pythonClsName;
            _fsPy.Write("    def __init__(self");

            bool isResource = AnySuperClassNamed(cls, "Resource");
            if (!isResource)
            {
                _fsPy.Write("    , folderId=0");
            }

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
                MandatoryProps.Add(p);
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
            foreach (IProperty p in RefProps)
            {
                WriteReferenceProperty(p);
            }
            if (!isResource)
            {
                _fsPy.WriteLine("        self.folderId = folderId");
            }
            _fsPy.WriteLine("");
        }


        //private void WriteGetByIdMethod(IClass cls)
        //{
        //    _fsPy.WriteLine("    def getById(self, {0}):", LowerCaseVarName(PKName));

        //    _fsPy.Write("        selStr = '''SELECT ");
        //    int numProps = Props.Count + FKProps.Count + AbsProps.Count * 2 + RefProps.Count;
        //    int numCols = 0;

        //    foreach (IProperty p in Props)
        //    {
        //        IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
        //        foreach (IStereotypeInstance column in columns)
        //        {
        //            numCols++;
        //            if (numCols < numProps)
        //            { _fsPy.Write("{0}, ", p.Name); }
        //            else
        //            { _fsPy.Write("{0}", p.Name); }
        //        }
        //    }
        //    foreach (IProperty p in FKProps)
        //    {
        //        numCols++;
        //        if (numCols < numProps)
        //        { _fsPy.Write("{0}Id, ", p.Name); }
        //        else
        //        { _fsPy.Write("{0}Id", p.Name); }

        //    }
        //    foreach (IProperty p in AbsProps)
        //    {
        //        numCols += 2;
        //        if (numCols < numProps)
        //        {
        //            _fsPy.Write("{0}Id", p.Name);
        //            _fsPy.Write(", {0}Table, ", p.Name);
        //        }
        //        else
        //        {
        //            _fsPy.Write("{0}Id, ", p.Name);
        //            _fsPy.Write("{0}Table", p.Name);
        //        }

        //    }
        //    foreach (IProperty p in RefProps)
        //    {
        //        numCols++;
        //        if (numCols < numProps)
        //        { _fsPy.Write("{0}Id, ", p.Name); }
        //        else
        //        { _fsPy.Write("{0}Id", p.Name); }

        //    }
        //    _fsPy.WriteLine("");
        //    _fsPy.Write("              FROM {0} WHERE {1} = %s''' % {2}", cls.Name, PKName, LowerCaseVarName(PKName));
        //    _fsPy.WriteLine("");
        //    _fsPy.WriteLine("        try:");
        //    _fsPy.WriteLine("            log.debug(selStr)");
        //    _fsPy.WriteLine("");
        //    _fsPy.WriteLine("            cursor.execute(selStr)");
        //    _fsPy.WriteLine("            row = self.cursor.fetchone()");
        //    _fsPy.WriteLine("            if row is None:");
        //    _fsPy.WriteLine("                return None");
        //    _fsPy.Write("            _{0} = {1}(", pythonClsName, pythonClsName);
        //    _fsPy.Write(CommaListProps(PKName, "row"));
        //    _fsPy.WriteLine(")");
        //    _fsPy.WriteLine("            return _{0}", pythonClsName);
        //    _fsPy.WriteLine("");
        //    _fsPy.WriteLine("        except Exception, e:");
        //    _fsPy.WriteLine("            raise Exception(e)");
        //}

        private void WriteProperty(IProperty p, IStereotypeInstance column)
        {
            _fsPy.WriteLine("        self.{0} = {1}", LowerCaseVarName(p.Name), LowerCaseVarName(p.Name));
        }

        private void WritePKProperty(IProperty p, IStereotypeInstance column)
        {
            _fsPy.WriteLine("        self.{0} = 0", LowerCaseVarName(p.Name));
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

        private string CommaListProps(string PK, string suffix)
        {
            int cursorCounter = 0;
            string resultStr = String.Empty;
            List<String> allPropNames = new List<String>();
            foreach (IProperty p in Props)
            {
                //if (p.Name != PK)
                //{
                    allPropNames.Add(p.Name);
                //}
            }
            foreach (IProperty fkp in FKProps)
            {
                allPropNames.Add(fkp.Name + "Id");
            }
            foreach (IProperty abp in AbsProps)
            {
                allPropNames.Add(abp.Name);
            }
            foreach (IProperty refp in RefProps)
            {
                allPropNames.Add(refp.Name);
            }
            int i = 0;
            foreach (String p in allPropNames)
            {
                i++;
                if (i == allPropNames.Count)
                {
                    resultStr = resultStr + LowerCaseVarName(p) + "=" + suffix + "[" + cursorCounter.ToString() + "]";
                }
                else
                {
                    resultStr = resultStr + LowerCaseVarName(p) + "=" + suffix + "[" + cursorCounter.ToString() + "]" + ", ";
                }
                cursorCounter++;
            }
            return resultStr;
        }


        private void InitializeSqlTypes()
        {
            SqlPythonTypes.Add("datetime", "DateTimeField");
            SqlPythonTypes.Add("bigint", "BigIntegerField");
            SqlPythonTypes.Add("varchar", "CharField");
            SqlPythonTypes.Add("binary", "BinaryField");
            SqlPythonTypes.Add("blob", "FileField");
            SqlPythonTypes.Add("double", "FloatField");
            SqlPythonTypes.Add("varbinary", "BinaryField");
            SqlPythonTypes.Add("date", "DateField");
            SqlPythonTypes.Add("decimal()", "DecimalField");
            SqlPythonTypes.Add("guid()", "TextField");
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
