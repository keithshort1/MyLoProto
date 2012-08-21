using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLoFacebookContextReaderNS;
using System.Diagnostics;
using GPSlookupNS;

namespace MyLoTests
{
    [TestClass]
    public class TestLoadFromFBContext
    {
        [TestMethod]
        public void SimpleLoad01()
        {
            try
            {
                BingMapsGPSlookup gps = new BingMapsGPSlookup();
                MyLoFacebookContextReader _fbContext = new MyLoFacebookContextReader(gps);
                _fbContext.InitializeContextFromFile();

                _fbContext.MyLoFacebookAlignment02();
                _fbContext.MyLoSchemaReasoner();
                _fbContext.SaveContextToFile();

                // Not sure if this is a bug in the dotNetRDF library, but it seems to require a fresh store and read from file
                // to make the second inference rules work ... investigating ...
                MyLoFacebookContextReader _fbContext2 = new MyLoFacebookContextReader(gps);
                _fbContext2.InitializeContextFromFile();
                _fbContext2.SaveContextToDB("Keith");   
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SimpleLoad01 Error");
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AlignmentRun()
        {
            try
            {
                BingMapsGPSlookup gps = new BingMapsGPSlookup();
                MyLoFacebookContextReader _fbContext = new MyLoFacebookContextReader(gps);
                _fbContext.InitializeContextFromFile();
                _fbContext.MyLoFacebookAlignment01();
                _fbContext.MyLoSchemaReasoner();
                _fbContext.MyLoFacebookAlignment02();
                _fbContext.MyLoSchemaReasoner();
                _fbContext.SaveContextToFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AlignmentRun Error");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
