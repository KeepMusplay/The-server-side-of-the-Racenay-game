using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace ServerRacenay
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.Title = "Game Server";
            Console.WriteLine("Getting connection to mysql server");
            MySqlConnection res = DBUtils.GetDBConnection();

            try
            {
                Console.WriteLine("Openning connection to mysql");

                res.Open();

                Console.WriteLine("Connection to mysql successfully!");
            }
            catch (Exception _ex)
            {
                Console.WriteLine("Error:" + _ex.Message);
            }

            Server.Start(24555);

            InitializerDefaultData.Initialize();



            // launch background tasks

            Thread distributor = new Thread(Distributor.Spread);
            distributor.IsBackground = true;
            distributor.Start();






            while (true)
            {
                string _command = Console.ReadLine();

                if (_command == "dis server")
                {
                    await DisconnectServer();
                    Console.WriteLine("Server disconnected");
                    break; // shutdown
                }
                else if (_command == "stats")
                {
                    try
                    {
                        int countActiveRoom = 0;
                        Server.rooms.ForEach((e) => countActiveRoom += e != null ? 1 : 0);
                        Console.WriteLine($"Пользователей всего: {Server.players.Count}, Активные комнаты: {countActiveRoom}, Комнат всего: {Server.rooms.Count - 1}, Всего приходов на сервер: {Server.NextIdNewClient}.");
                    }
                    catch (Exception _exc)
                    {
                        Console.WriteLine(_exc.Message + " " + _exc.StackTrace);
                    }
                    continue;
                }
                else
                {
                    continue;
                }
            }

            async System.Threading.Tasks.Task DisconnectServer()
            {
                await Server.DisconnectServer();

                Console.WriteLine("DisconnectServer complete successfully!");
            }

        }

        static readonly string Version = "v1.0.0";

        public static readonly string VersionClientUnity3d = "v1.0.0";
    }
}
