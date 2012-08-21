using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Uml;
using Microsoft.VisualStudio.Uml.Classes;
using Microsoft.VisualStudio.Uml.AuxiliaryConstructs;

namespace PLDBValidation

{
    public class PLDBValidationConstraints
    {

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod( ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateClassNames (ValidationContext context, IClass cls)
        {
            // Property and Role names must be unique within a class hierarchy.

            List<string> foundNames = new List<string>();
            List<IProperty> allPropertiesInHierarchy = new List<IProperty>();
            List<IProperty> superRoles = new List<IProperty>();
            FindAllAssocsInSuperClasses(superRoles, cls.SuperClasses);

            foreach (IProperty p in cls.GetOutgoingAssociationEnds()) { superRoles.Add(p); }
            foreach (IProperty p in superRoles) { allPropertiesInHierarchy.Add(p); }
            foreach (IProperty p in cls.Members) { allPropertiesInHierarchy.Add(p); }

            foreach (IProperty attribute in allPropertiesInHierarchy)
            {
                string name = attribute.Name;
                if (!string.IsNullOrEmpty(name) && foundNames.Contains(name))
                {
                    context.LogError(
                      string.Format("Duplicate property or role name '{0}' in class '{1}'", name, cls.Name),
                      "001", cls);
                }
                foundNames.Add(name);
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidatePrimaryKeys(ValidationContext context, IClass cls)
        {
            // A class which is a <<Table>> must have exactly one column marked as a Primary Key.
            IEnumerable<IStereotypeInstance> classKinds = cls.AppliedStereotypes.Where(s => s.Name == "Table");
            foreach (IStereotypeInstance classKind in classKinds)
            {
                if (!cls.IsAbstract)
                {
                    List<IProperty> props = new List<IProperty>();

                    foreach (IProperty p in cls.Members)
                    {
                        IEnumerable<IStereotypeInstance> columns = p.AppliedStereotypes.Where(s => s.Name == "Column");
                        foreach (IStereotypeInstance column in columns)
                        {
                            if (IsPrimaryKey(column))
                            {
                                props.Add(p);
                            }
                        }
                    }
                    if (props.Count == 0)
                    {
                        context.LogError(
                          string.Format("Class '{0}' has no properties of type 'Column' marked as Primary Keys", cls.Name),
                          "002", cls);
                    }
                    else if (props.Count > 1)
                    {
                        context.LogError(
                          string.Format("Class '{0}' has {1} properties of type 'Column' marked as Primary Keys", cls.Name, props.Count),
                          "003", cls);
                    }
                }
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateAssociations(ValidationContext context, IAssociation assoc)
        {
            // All associations between classes must have a stereotype.

            IEnumerable<IStereotypeInstance> assocStereotypes = assoc.AppliedStereotypes;
            
            if (assocStereotypes.Count() == 0)
            {
                context.LogError(
                    string.Format("Association '{0}' has no stereotype for SQL generation", assoc.Name),
                    "004A", assoc);
            }
            else if (assocStereotypes.Count() > 1)
            {
                context.LogError(
                   string.Format("Association '{0}' has more than one stereotype for SQL generation", assoc.Name),
                   "004B", assoc);
            }
        }


        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateAttributesAreColumns(ValidationContext context, IProperty prop)
        {
            // All attributes of a Class must be stereotyped as Columns.

            if (prop.Class != null)
            {
                IEnumerable<IStereotypeInstance> propColumnStereotypes = prop.AppliedStereotypes.Where(s => s.Name == "Column");

                if (propColumnStereotypes.Count() == 0)
                {
                    context.LogError(
                        string.Format("Property '{0}' of Class '{1}' has no Column stereotype for SQL generation", prop.Name, prop.Class.Name),
                        "005", prop.Class);
                }
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateColumnsHaveTypes(ValidationContext context, IProperty prop)
        {
            // All Columns must have a Data Type.

            if (prop.Class != null)
            {
                IEnumerable<IStereotypeInstance> propColumnStereotypes = prop.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance col in propColumnStereotypes)
                {
                    if (GetDataType(col) == null)
                    {
                        context.LogError(
                        string.Format("Property Column '{0}' of Class '{1}' has no Column Datatype for SQL generation", prop.Name, prop.Class.Name),
                        "006", prop.Class);
                    }
                }
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateVarColumnsHaveLengths(ValidationContext context, IProperty prop)
        {
            // All Columns whose Data Type is var..() must also have a length.

            if (prop.Class != null)
            {
                IEnumerable<IStereotypeInstance> propColumnStereotypes = prop.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance col in propColumnStereotypes)
                {
                    
                    string s = GetDataType(col);
                    StringBuilder dt = new StringBuilder(s);
                    if (s.Contains("()"))
                    {
                        if (String.IsNullOrEmpty(GetLength(col)))
                        {
                            context.LogError(
                            string.Format("Property Column '{0}' of Class '{1}' has variable Data Type but no Length for SQL generation", prop.Name, prop.Class.Name),
                            "007", prop.Class);
                        }
                    }
                }
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateFKNameLength(ValidationContext context, IAssociation assoc)
        {
            // All Associations of type "ForeignKey" must either have a name < 64 chars or derived name  < 64 chars.

            IEnumerable<IStereotypeInstance> assocFKColumnStereotypes = assoc.AppliedStereotypes.Where(s => s.Name == "ForeignKey");
            foreach (IStereotypeInstance fk in assocFKColumnStereotypes)
            {

                if (String.IsNullOrEmpty(assoc.Name))
                {
                    string me1 = assoc.MemberEnds.First().Type.Name;
                    string s = me1 + assoc.NavigableOwnedEnds.First().Name;
                    if (!IsValidIdentifierLength(s))
                    {
                        context.LogError(
                        string.Format("Association Name is missing and derived Name '{0}' is too long", s),
                        "008", assoc);
                    }
                }
                else if (!IsValidIdentifierLength(assoc.Name))
                {
                    context.LogError(
                        string.Format("Association Name '{0}' too long", assoc.Name),
                        "008", assoc);
                }
                
            }
        }


        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateFKNameLength(ValidationContext context, IProperty prop)
        {
            // All Properties of type "Column" must have a name < 64 chars.

            if (prop.Class != null)
            {
                IEnumerable<IStereotypeInstance> propColumnStereotypes = prop.AppliedStereotypes.Where(s => s.Name == "Column");
                foreach (IStereotypeInstance col in propColumnStereotypes)
                {
                    string n = prop.Name;
                    if (String.IsNullOrEmpty(n) || (!IsValidIdentifierLength(n)))
                    {
                        context.LogError(
                            string.Format("Property Column '{0}' of Class '{1}' must have a Name with less than 64 characters", prop.Name, prop.Class.Name),
                            "009", prop.Class);
                    }
                }
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateClassStereotypes(ValidationContext context, IClass cls)
        {
            // All  classes must have zero or exactly one stereotype.

            IEnumerable<IStereotypeInstance> clsStereotypes = cls.AppliedStereotypes;

            if (clsStereotypes.Count() > 1)
            {
                context.LogError(
                   string.Format("Class '{0}' has more than one stereotype for SQL generation", cls.Name),
                   "0010", cls);
            }
        }


        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateTableHierarchies(ValidationContext context, IClass cls)
        {
            // All  Classes that are TableHierarchies must have no inheritance

            IEnumerable<IStereotypeInstance> classKinds = cls.AppliedStereotypes.Where(s => s.Name == "TableHierarchy");
            foreach (IStereotypeInstance classKind in classKinds)
            {
                if (cls.SuperClasses.Count() > 0)
                {
                    context.LogError(
                        string.Format("Class '{0}' is TableHierarchy so must not have base classes", cls.Name),
                        "0011", cls);
                }
            }

        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        public void ValidateTableHierarchAssociations(ValidationContext context, IClass cls)
        {
            // All  Classes that are TableHierarchies must have exactly one Parent and Child Association

            IEnumerable<IStereotypeInstance> classKinds = cls.AppliedStereotypes.Where(s => s.Name == "TableHierarchy");
            foreach (IStereotypeInstance classKind in classKinds)
            {
                int parentCount = 0; int childCount = 0;
                foreach (IProperty prop in cls.GetOutgoingAssociationEnds())
                {
                    IAssociation assoc = prop.Association;
                    IEnumerable<IStereotypeInstance> kindsParent = assoc.AppliedStereotypes.Where(s => s.Name == "Parent");
                    IEnumerable<IStereotypeInstance> kindsChild = assoc.AppliedStereotypes.Where(s => s.Name == "Child");
                    parentCount += kindsParent.Count();
                    childCount += kindsChild.Count();
                    
                }
                if (!(parentCount == 1 && childCount == 1))
                {
                    context.LogError(
                    string.Format("Class '{0}' is a TableHierarchy so must not have exactly one Parent and Child association type", cls.Name),
                    "0012", cls);
                }
            }

        }

        private bool IsValidIdentifierLength(string p)
        {
            return !(p.Length > 64);
        }

        //[Export(typeof(System.Action<ValidationContext, object>))]
        //[ValidationMethod(ValidationCategories.Save | ValidationCategories.Open | ValidationCategories.Menu)]
        //public void ValidateNoInheritanceLoops(ValidationContext context, IModel model)
        //{
        //    try
        //    {
        //        // The helper method follows the inheritance relation from the starting point we give.
        //        // Don't check again those that have been seen while following links.
        //        Dictionary<IClass, string> nodesChecked = new Dictionary<IClass, string>();
        //        //foreach (IClass cls in model.OwnedElements)
        //        foreach (IType t in model.OwnedTypes)
        //        {
        //            if (t is IClass)
        //            {
        //                IClass cls = (IClass)t;
        //                if (!nodesChecked.ContainsKey(cls))
        //                {
        //                    ValidationHelperNoGeneralizationLoops(cls, new Stack<IClass>(), nodesChecked);
        //                }
        //            }
        //        }
        //    }
        //    catch (ClassValidationLoopException validationException)
        //    {
        //        IClass problem = validationException.Problem;
        //        context.LogError(
        //                    string.Format("Error in Inheritance Loop checking on class'{0}'", problem.Name),
        //                    "008", problem);
        //    }
        //}

        //private void ValidationHelperNoGeneralizationLoops(IClass toCheck, Stack<IClass> currentPath, Dictionary<IClass, string> doneNodes)
        //{
        //    if (currentPath.Contains(toCheck))
        //    {
        //        throw new ClassValidationLoopException(toCheck);;

        //    }
        //    if (!doneNodes.ContainsKey(toCheck)) // don't bother if already examined this in an earlier scan
        //    {
        //        currentPath.Push(toCheck);
        //        foreach (IClass next in toCheck.SuperClasses)
        //        {
        //            ValidationHelperNoGeneralizationLoops(next, currentPath, doneNodes);
        //        }
        //        currentPath.Pop();
        //        doneNodes[toCheck] = "";
        //    }
        //}

        // Helper Methods - should place these in reusable assembly really

        private string GetDefaultValue(IProperty p)
        {
            return (p.DefaultValue != null) ? "Default " + p.DefaultValue.ToString() : String.Empty;
        }

        private string GetDataType(IStereotypeInstance column)
        {
            IStereotypePropertyInstance dataType = column.PropertyInstances.Where(p => p.Name == "DataType").First();
            return dataType.Value;
        }

        private string GetLength(IStereotypeInstance column)
        {
            IStereotypePropertyInstance length = column.PropertyInstances.Where(p => p.Name == "Length").First();
            return length.Value;
        }

        private string GetNull(IStereotypeInstance column)
        {
            IStereotypePropertyInstance allowNulls = column.PropertyInstances.Where(p => p.Name == "AllowNulls").First();
            return bool.Parse(allowNulls.Value) == true ? string.Empty : "not null";
        }

        private bool IsPrimaryKey(IStereotypeInstance column)
        {
            IStereotypePropertyInstance pk = column.PropertyInstances.Where(p => p.Name == "PrimaryKey").First();
            return bool.Parse(pk.Value) == true ? true : false; ;
        }

        private string GetDeleteAction(IStereotypeInstance assoc)
        {
            IStereotypePropertyInstance deleteRule = assoc.PropertyInstances.Where(p => p.Name == "DeleteRule").First();
            return deleteRule.Value;
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
        internal class ClassValidationLoopException : Exception
        {
            private IClass problem;
            public IClass Problem { get { return problem; } }
            public ClassValidationLoopException(IClass fault) { problem = fault; }
        }
    }
}
