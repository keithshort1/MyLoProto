/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application

------------------------------------------------------------------------
*/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLoIndexerNS;
using System.Diagnostics;
using MyLoDBNS;

namespace MyLoTests
{
    [TestClass]
    public class IndexTest01
    {
        [TestMethod]
        public void IndexFolder2012()
        {
            LinearClusterMyLoIndexer indexer = new LinearClusterMyLoIndexer();
            MyLoIndexer mx = new MyLoIndexer(indexer);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                mx.UserLogin(1);
                int count = mx.StartIndexing();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Debug.WriteLine(String.Format("Finished Indexing; Time: {0}; Number: {1}", elapsedTime, count));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format(ex.Message));
            }
        }
    }
}
