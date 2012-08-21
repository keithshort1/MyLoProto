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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Generators;

namespace GenerateDBCommand
{
    [ClassDesignerExtension]

    // All menu commands must export ICommandExtension:
    [Export(typeof(ICommandExtension))]
    public class GenerateMenu : ICommandExtension
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
            get { return "Generate MySQL Database INNODB"; }
        }

        public void Execute(IMenuCommand command)
        {
            IDiagram diagram = this.DiagramContext.CurrentDiagram;
            IModelStore modelStore = diagram.ModelStore;

            const string newPath = @"C:\PLDB";
            System.IO.Directory.CreateDirectory(newPath);
            string SqlFile = Path.Combine(newPath, @"PLDBmysql.sql");
            string PythonFile = Path.Combine(newPath, @"PLDB.py");

            using (StreamWriter fsSql = new StreamWriter(SqlFile))
            {
                using (StreamWriter fsPy = new StreamWriter(PythonFile))
                {
                    SQLWriter mySql = new SQLWriter(SQLGenerateRun.INNODB);
                    //DjangoWriter py = new DjangoWriter();
                    PythonClassWriter py = new PythonClassWriter();
                    SQLGenerator sqlGen = new SQLGenerator(fsSql, mySql, fsPy, py, modelStore);
                    sqlGen.GenerateMySQL();

                }
            }
            

            // TODO debug this code: seems like I'm missing an assembly reference!
            //var outWindow = Package.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            //Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;
            //IVsOutputWindowPane generalPane;
            //outWindow.GetPane(ref generalPaneGuid, out generalPane);
            //generalPane.OutputString("============= SQL Generation Successful =================");
            //generalPane.Activate(); // Brings this pane into view
            
        }
    }

}
