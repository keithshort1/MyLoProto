using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Presentation;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Uml;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Uml.AuxiliaryConstructs;
using Microsoft.VisualStudio.Uml.Classes;
using System.Text;
using Generators;

namespace GenerateDBCommandMyISAM
{
    [ClassDesignerExtension]


    // All menu commands must export ICommandExtension:
    [Export(typeof(ICommandExtension))]

    public class GenerateMenuMyISAM : ICommandExtension
    {
        [Import]
        public IDiagramContext DiagramContext { get; set; }

        public void QueryStatus(IMenuCommand command)
        {   // Set command.Visible or command.Enabled to false
            // to disable the menu command.
            command.Visible = command.Enabled = true;
        }

        public string Text
        {
            get { return "Generate MySQL Database MyISAM"; }
        }

        public void Execute(IMenuCommand command)
        {
            IDiagram diagram = this.DiagramContext.CurrentDiagram;
            IModelStore modelStore = diagram.ModelStore;

            const string SqlFile = @"C:\PLDB\PLDBmysql.sql";
            const string PythonFile = @"C:\PLDB\PLDB.py";

            using (StreamWriter fsSql = new StreamWriter(SqlFile))
            {
                using (StreamWriter fsPy = new StreamWriter(PythonFile))
                {
                    SQLWriter mySql = new SQLWriter(SQLGenerateRun.MyISAM);
                    PythonClassWriter py = new PythonClassWriter();
                    SQLGenerator sqlGen = new SQLGenerator(fsSql, mySql, fsPy, py, modelStore);
                    sqlGen.GenerateMySQL();
                }
            }

        }
    }
}
