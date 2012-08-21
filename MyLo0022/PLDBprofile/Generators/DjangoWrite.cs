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
    public class DjangoWriter : IDatabaseCodeGenerator
    {
        private string _potentialUnicodeName;
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
        private List<IProperty> Props = new List<IProperty>();
        private List<IProperty> FKProps = new List<IProperty>();
        private List<IProperty> AbsProps = new List<IProperty>();
        private List<IProperty> RefProps = new List<IProperty>();
        private List<IProperty> PKProps = new List<IProperty>();
        private List<IProperty> CompProps = new List<IProperty>();

        public void AddStreamWriter(StreamWriter fsPy)
        {
            _fsPy = fsPy;
        }

        public DjangoWriter()
        {
            InitializeSqlTypes();
        }

        public void Initialize()
        {
            FKcons = new List<FKConstraint>();
            _potentialUnicodeName = String.Empty;
            Props = new List<IProperty>();
            FKProps = new List<IProperty>();
            AbsProps = new List<IProperty>();
            PKProps = new List<IProperty>();
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

        public void WriteFKConstraints()
        {
            if (FKcons.Count != 0)
            {
                int i = 0;
                foreach (FKConstraint f in FKcons)
                {
                    i++;
                    this.WriteFKConstraint(f.Name, f.SourceColumn, f.TargetColumn, f.TargetTable, f.DeleteAction);
                }
            }
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

        public void WriteStructureBody(IClass cls, string pkName)
        {
            this.WriteProperties(cls, pkName);
            this.WriteFKConstraints();
        }


        private void WriteProperties(IClass cls, string pk)
        {
            foreach (IProperty p in cls.Members)
            {
                IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance column in columns)
                {
                    if (p.Name.Contains("Name")) { _potentialUnicodeName = LowerCaseVarName(p.Name); }
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
        }

        public void WriteHeader()
        {
            DateTime now = DateTime.Now;
            _fsPy.WriteLine("from django.db import models");
            _fsPy.WriteLine("");
            _fsPy.WriteLine("#  Django Class definitions for Schrodinger Bluebird Project and Curated Protein Ligand Database");
            _fsPy.WriteLine("#  Schrodinger Inc. Confidential - All Rights Reserved");
            _fsPy.WriteLine("#  Generated on {0}", now.ToString());
            _fsPy.WriteLine("");
        }


        public void WriteStructureFinish(IClass cls)
        {
            PluralizationService pluralizationService = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-US"));
            _fsPy.WriteLine("    class Meta:");
            _fsPy.WriteLine("        db_table = u'{0}'", cls.Name);
            _fsPy.WriteLine("        verbose_name_plural = '{0}'", pluralizationService.Pluralize(cls.Name));
            _fsPy.WriteLine();
            _fsPy.WriteLine("    def __unicode__(self):");
            _fsPy.WriteLine("        return self.{0}", String.IsNullOrEmpty(_potentialUnicodeName) ? "__class__.__name__" : _potentialUnicodeName);
            _fsPy.WriteLine();
        }

        private string LowerCaseVarName(string p)
        {
            return char.ToLower(p[0]) + p.Substring(1);
        }

        private string BuildDjangoDataType(IStereotypeInstance column)
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

        public void WriteStructureStart(IClass cls)
        {
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("# Class Definition for {0}", cls.Name);
            _fsPy.WriteLine("#");
            _fsPy.WriteLine("  ");
            _fsPy.WriteLine("class {0}(models.Model):", cls.Name);
        }

        public void WriteProperty(IProperty p, IStereotypeInstance column)
        {
            _fsPy.WriteLine("    {0} = models.{1}db_column='{2}', blank={3} {4})",
                LowerCaseVarName(p.Name), BuildDjangoDataType(column), p.Name,
                    String.IsNullOrEmpty(UmlHelper.GetNull(column)) ? "True" : "False",
                    String.IsNullOrEmpty(UmlHelper.GetDefaultValue(p)) ? "" : ", default={0})", UmlHelper.GetDefaultValue(p));
            //description = models.CharField(max_length=768, db_column='Description', blank=True) 
        }

        public void WritePKProperty(IProperty p, IStereotypeInstance column)
        {
            _fsPy.WriteLine("    {0} = models.BigIntegerField(primary_key=True, db_column='{1}')", LowerCaseVarName(p.Name), p.Name);
        }

        public void WriteFKProperty(IProperty ae)
        {
            //_fsSql.WriteLine("    {0}Id bigInt,", ae.Name);
        }

        public void WriteReferenceProperty(IProperty ae)
        {
            _fsPy.WriteLine("    {0} = models.BigIntegerField(null=True, db_column='{1}', blank=True)", LowerCaseVarName(ae.Name), ae.Name);
            //projecttemplateid = models.BigIntegerField(null=True, db_column='ProjectTemplateId', blank=True)
        }

        public void WriteAbstractReferenceProperty(IProperty ae)
        {
            _fsPy.WriteLine("    {0}Id = models.BigIntegerField(null=True, db_column='{1}', blank=True)", LowerCaseVarName(ae.Name), ae.Name);
            _fsPy.WriteLine("    {0}Table = models.CharField(max_length=128, db_column='{1}Table')", LowerCaseVarName(ae.Name), ae.Name);
        }


        private void WriteFKConstraint(string name, string sourceColumn, string targetColumn, string targetTable, string deleteAction)
        {
            _fsPy.WriteLine("    {0} = models.ForeignKey({1}, null=True, db_column='{2}', blank=True, related_name='{3}')",
                LowerCaseVarName(sourceColumn), targetTable, sourceColumn, name);
            //parentprojectid = models.ForeignKey(Project, null=True, db_column='ParentProjectId', blank=True, related_name='parent_set') # Field name made lowercase
        }

        public void AddIndexConstraint(string indexName, string columns, string indexType)
        {
        }

        public void WriteTrailer()
        {
            _fsPy.WriteLine();
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


    }
}
