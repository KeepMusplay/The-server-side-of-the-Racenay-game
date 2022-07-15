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
            string database = "name_database";
            string username = "root";
            string password = "password_database";

            return (res = DBMMySQLUtils.GetDBConnection(host, port, database, username, password));
        }
    }
}
