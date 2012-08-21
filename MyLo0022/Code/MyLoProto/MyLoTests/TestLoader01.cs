using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using PhotoLoaderNS;

namespace MyLoTests
{
    [TestClass]
    public class TestLoader01
    {
        [TestMethod]
        public void Load2012photos()
        {
            string folderName = @"C:\Users\Keith\Dropbox\Photos\2012";
            long _userId = 0;
            PhotoLoader mx = new PhotoLoader();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                _userId = mx.UserLogin("Keith");
                if (_userId != 0)
                {
                    int count = mx.StartLoading(folderName);
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    Debug.WriteLine(String.Format("Finished Indexing {0}; Time: {1}; Number: {2}", folderName, elapsedTime, count));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format(ex.Message));
            }
        }
    }
}
