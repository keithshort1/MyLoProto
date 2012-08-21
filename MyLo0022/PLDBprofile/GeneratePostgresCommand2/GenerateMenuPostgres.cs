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
using System.Windows.Forms;

namespace GeneratePostgresCommand
{
    [ClassDesignerExtension]


    // All menu commands must export ICommandExtension:
    [Export(typeof(ICommandExtension))]

    public class GenerateMenuPostgres : ICommandExtension
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
            get { return "Generate Postgres Database"; }
        }

        public void Execute(IMenuCommand command)
        {
            IDiagram diagram = this.DiagramContext.CurrentDiagram;
            IModelStore modelStore = diagram.ModelStore;

            string SqlFile = @"C:\MyLoStore\MyLoStorePostgres.sql";
            string PythonFile = @"C:\MyLoStore\MyLoStore.py";

            FolderBrowserDialog openFolderDialog1 = new FolderBrowserDialog();
            openFolderDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            openFolderDialog1.Description =
                "Select the directory that you want to use for generated output";
            
            if (openFolderDialog1.ShowDialog() == DialogResult.OK)
            {
                string folderName = openFolderDialog1.SelectedPath;
                SqlFile = folderName + @"\MyLoStorePostgres.sql";
                PythonFile = folderName + @"\MyLoStore.py";

                using (StreamWriter fsSql = new StreamWriter(SqlFile, false))
                {
                    using (StreamWriter fsPy = new StreamWriter(PythonFile, false))
                    {

                        SQLWriter mySql = new SQLWriter(SQLGenerateRun.Postgres);
                        PythonClassWriter py = new PythonClassWriter();
                        SQLGenerator sqlGen = new SQLGenerator(fsSql, mySql, fsPy, py, modelStore);
                        sqlGen.GenerateMySQL();
                    }
                }
            }

        }
    }
}
