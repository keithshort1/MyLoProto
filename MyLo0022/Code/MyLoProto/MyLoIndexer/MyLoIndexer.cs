/*

Copyright Keith Short 2012
keith_short@hotmail.com

This file is part of the MyLo application
Uses FotoFly v0.5 under Microsoft Public License (Ms-PL)

------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using FotoFly;
using MyLoDBNS;
using MyLoExceptions;


namespace MyLoIndexerNS
{
    public class MyLoIndexer 
    {
        private int _count;
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        long _userId;
        private MyLoDB _myLoStore;
        private IMyLoIndexer _indexer;


        /// <summary>
        /// Creates a new Loader for the given top level folder
        /// </summary>
        public MyLoIndexer(IMyLoIndexer indexer)
        {
            _myLoStore = new MyLoDB();
            _count = 0;
            _userId = 0;
            _indexer = indexer;
        }

        /// <summary>
        /// Used when User Name has been validated and cached
        /// </summary>
        /// <param name="userId">MyLo Account Holder Id</param>
        public void UserLogin(long userId)
        {
            _userId =userId;
        }


        /// <summary>
        /// Start a recursive breadth first traversal of the folders from the given top level folder
        /// </summary>
        /// <param name="userName">MyLo Account Holder Name</param>
        /// <param name="dbName">PostgreSQL database Name</param>
        public int StartIndexing()
        {
            if (_userId != 0)
            {
                _count = _myLoStore.IndexDataStore(_userId, _indexer);
                return _count;
            }
            else
            {
                throw new MyLoAccountIdException("MyLo User not signed in");
            }
        }
    }
    
}
