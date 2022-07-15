using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ServerRacenay
{
    class DBUtils
    { // for connect to remote mysql database using here ip computer

        public static MySqlConnection res;

        public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "host1832718";
            string username = "root";
            string password = "host1832718";

            return (res = DBMMySQLUtils.GetDBConnection(host, port, database, username, password));
        }
    }
}
