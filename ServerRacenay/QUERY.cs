﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace ServerRacenay
{
    // class for queries to DB
    // _SORT_STRING example: "WHERE id = @id"
    class QUERY
    {
        private static object locked = new object();

        public static int INSERT(string _table, string[] _parameters, string[] _values, string _SORT_STRING)
        {
            lock (locked)
            {
                if (_table == "" || _parameters.Length == 0 || _values.Length == 0) return -1;
                string parameters = " (?";
                for (int i = 0; i < _parameters.Length; i++)
                {
                    parameters += "`" + _parameters[i] + "`";
                    if (i != (_parameters.Length - 1))
                    {
                        parameters += ", ?";
                    }
                    else
                    {
                        parameters += ") ";
                    }
                }

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = DBUtils.res;
                cmd.CommandText = "INSERT INTO " + _table + parameters.Replace("?", "") + "VALUES " + parameters.Replace("?", "@").Replace("`", "") + _SORT_STRING;

                for (int j = 0; j < _values.Length; j++)
                {
                    if ("0".Equals(_values[j][0]))
                    {
                        int number = int.Parse(_values[j]);
                        cmd.Parameters.AddWithValue(("@" + _parameters[j]), number);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(("@" + _parameters[j]), _values[j]);
                    }
                }

                cmd.Prepare();
                return cmd.ExecuteNonQuery();
            }
        }
        
        public static int UPDATE(string _table, string[] _parameters, string[] _values, string _SORT_STRING)
        {
            lock (locked)
            {
                if (_table == "" || _parameters.Length == 0 || _values.Length == 0) return -1;
                string parameters = "";
                for (int i = 0; i < _parameters.Length; i++)
                {
                    if (i == (_parameters.Length - 1))
                    {
                        parameters += "`" + _parameters[i] + "`" + " = @" + _parameters[i] + " ";
                    }
                    else
                    {
                        parameters += "`" + _parameters[i] + "`" + " = @" + _parameters[i] + ", ";
                    }
                }

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = DBUtils.res;
                cmd.CommandText = "UPDATE " + _table + " SET " + parameters + _SORT_STRING;
                if (_SORT_STRING != "")
                {
                    if ("W".Equals(_SORT_STRING[0].ToString()))
                    {
                        string lastParameter = _SORT_STRING.Substring((_SORT_STRING.IndexOf("@")));
                        if ("0".Equals(_values[_values.Length - 1][0]))
                        {
                            cmd.Parameters.AddWithValue(lastParameter, int.Parse(_values[(_values.Length - 1)]));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(lastParameter, _values[_values.Length - 1]);
                        }
                    }
                }

                for (int j = 0; j < _parameters.Length; j++)
                {
                    if ("0".Equals(_values[j][0]))
                    {
                        int number = int.Parse(_values[j]);
                        cmd.Parameters.AddWithValue(("@" + _parameters[j]), number);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(("@" + _parameters[j]), _values[j]);
                    }
                }

                cmd.Prepare();
                return cmd.ExecuteNonQuery();
            }
        }

        public static void SELECT(string _table, string[] _parameters, string[] _values, string _SORT_STRING, out MySqlCommand cmd)
        { // _parameters for SELECT
          // _values for WHERE

            lock (locked)
            {
                string parameters;
                if ("*".Equals(_parameters[0][0]))
                {
                    parameters = "*";
                }
                else
                {
                    for (int i = 0; i < _parameters.Length; i++)
                        _parameters[i] = "`" + _parameters[i] + "`";

                    parameters = String.Join(", ", _parameters);
                }
                cmd = new MySqlCommand();
                cmd.Connection = DBUtils.res;
                cmd.CommandText = "SELECT " + parameters + " FROM " + _table + " " + _SORT_STRING;

                if (_SORT_STRING != "")
                {
                    int[] positionsParameters = new int[4]; // 4 - max count parameters in sql WHERE 
                    string[] arguments = new string[4];

                    void Recursion(int _i, int _pos)
                    {
                        if ((_pos = _SORT_STRING.IndexOf("@", _pos)) != -1)
                        {
                            positionsParameters[_i] = _pos;
                            Recursion(++_i, ++_pos);
                        }
                    }
                    Recursion(0, 0);

                    _SORT_STRING += " ";
                    for (int i = 0; i < positionsParameters.Length; i++)
                    {
                        arguments[i] = _SORT_STRING.Substring(positionsParameters[i], (_SORT_STRING.IndexOf(" ", positionsParameters[i]) - positionsParameters[i]));
                    }

                    for (int j = 0; j < _values.Length; j++)
                    {
                        if ("0".Equals(_values[j][0]))
                        {
                            int number = int.Parse(_values[j]);
                            cmd.Parameters.AddWithValue(arguments[j], number);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(arguments[j], _values[j]);
                        }
                    }
                }
                cmd.Prepare();
            }
        }

        public static int DELETE(string _table, string[] _values, string _SORT_STRING)
        { // _values for WHERE
            lock (locked)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = DBUtils.res;
                cmd.CommandText = "DELETE FROM " + _table + " " + _SORT_STRING;

                if (_SORT_STRING != "")
                {
                    int[] positionsParameters = new int[4]; // 4 - max count parameters in sql WHERE 
                    string[] arguments = new string[4];

                    void Recursion(int _i, int _pos)
                    {
                        if ((_pos = _SORT_STRING.IndexOf("@", _pos)) != -1)
                        {
                            positionsParameters[_i] = _pos;
                            Recursion(++_i, ++_pos);
                        }
                    }
                    Recursion(0, 0);

                    _SORT_STRING += " ";
                    for (int i = 0; i < positionsParameters.Length; i++)
                    {
                        arguments[i] = _SORT_STRING.Substring(positionsParameters[i], (_SORT_STRING.IndexOf(" ", positionsParameters[i]) - positionsParameters[i]));
                    }

                    for (int j = 0; j < _values.Length; j++)
                    {
                        if ("0".Equals(_values[j][0]))
                        {
                            int number = int.Parse(_values[j]);
                            cmd.Parameters.AddWithValue(arguments[j], number);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(arguments[j], _values[j]);
                        }
                    }
                }
                cmd.Prepare();
                return cmd.ExecuteNonQuery();
            }
        }

        public static int DELETE(string _table)
        { // _values for WHERE

            lock (locked)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = DBUtils.res;
                cmd.CommandText = "DELETE FROM " + _table;
                cmd.Prepare();
                return cmd.ExecuteNonQuery();
            }
        }

        public static void TRUNCATE(string _table)
        {
            lock (locked)
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = DBUtils.res;
                cmd.CommandText = "TRUNCATE TABLE " + _table;
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
