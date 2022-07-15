using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Dedicate_Server_Unity3d_LastVersion
{
    internal class CloserConnection
    {
        public static void Close(MySqlConnection conn, int ms)
        {
            Thread.Sleep(ms);


            conn.Close();
            conn.Dispose();
        }
    }
}
