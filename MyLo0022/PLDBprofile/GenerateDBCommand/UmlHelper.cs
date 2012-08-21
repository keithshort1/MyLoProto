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

namespace GenerateDBCommand
{
    public static class UmlHelper
    {
        public static string GetDefaultValue(IProperty p)
        {
            return (p.DefaultValue != null) ? "Default " + p.DefaultValue.ToString() : String.Empty;
        }

        public static string GetDataType(IStereotypeInstance column)
        {
            IStereotypePropertyInstance dataType = column.PropertyInstances.Where(p => p.Name == "DataType").First();
            return dataType.Value;
        }

        public static string GetLength(IStereotypeInstance column)
        {
            IStereotypePropertyInstance length = column.PropertyInstances.Where(p => p.Name == "Length").First();
            return length.Value;
        }

        public static string GetNull(IStereotypeInstance column)
        {
            IStereotypePropertyInstance allowNulls = column.PropertyInstances.Where(p => p.Name == "AllowNulls").First();
            return bool.Parse(allowNulls.Value) == true ? string.Empty : "not null";
        }

        public static bool IsPrimaryKey(IStereotypeInstance column)
        {
            IStereotypePropertyInstance pk = column.PropertyInstances.Where(p => p.Name == "PrimaryKey").First();
            return bool.Parse(pk.Value) == true ? true : false; ;
        }

        public static string GetDeleteAction(IStereotypeInstance assoc)
        {
            IStereotypePropertyInstance deleteRule = assoc.PropertyInstances.Where(p => p.Name == "DeleteRule").First();
            return deleteRule.Value;
        }

    }
}
