using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace MyLoDBNS
{
    public interface IMyLoIndexer
    {
        void InitializeMyLoIndexer(long userId, NpgsqlConnection conn, MyLoDB store);

        int ExecuteIndexerOnDataStore();
    }
}
