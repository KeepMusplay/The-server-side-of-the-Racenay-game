using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Microsoft;

namespace ServerRacenay
{
    class Server
    {
        public static int Port { get; private set; }
        public static int NextIdNewClient { get; private set; } = 0;
        public static Dictionary<int, Player> players { get; } = new Dictionary<int, Player>();
        public static List<Room> rooms { get; } = new List<Room>(1) { null };



        public const int COEFF_SEPARATION = 15; // ?
        public static Dictionary<int, List<Player>> goldenQueue { get; } = new Dictionary<int, List<Player>>()
        {
            [1] = new List<Player>(),
            [2] = new List<Player>(),
            [3] = new List<Player>(),
            [4] = new List<Player>(),
            [5] = new List<Player>(),
            [6] = new List<Player>(),
            [7] = new List<Player>(),
            [8] = new List<Player>(),
            [9] = new List<Player>(),
            [10] = new List<Player>(),
            [11] = new List<Player>(),
            [12] = new List<Player>(),
            [13] = new List<Player>(),
            [14] = new List<Player>(),
            [15] = new List<Player>(),
        };
        public static Dictionary<int, List<Player>> silverQueue { get; } = new Dictionary<int, List<Player>>()
        {
            [1] = new List<Player>(),
            [2] = new List<Player>(),
            [3] = new List<Player>(),
            [4] = new List<Player>(),
            [5] = new List<Player>(),
            [6] = new List<Player>(),
            [7] = new List<Player>(),
            [8] = new List<Player>(),
            [9] = new List<Player>(),
            [10] = new List<Player>(),
            [11] = new List<Player>(),
            [12] = new List<Player>(),
            [13] = new List<Player>(),
            [14] = new List<Player>(),
            [15] = new List<Player>(),
        };


        private static TcpListener tcpListener;
        
        public static void Start(int _port)
        {
            Port = _port;

            Console.WriteLine("Starting server...");

            var host = Dns.GetHostEntry(Dns.GetHostName());

            tcpListener = new TcpListener(host.AddressList[1], Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            Console.WriteLine($"Server started on port {Port}.(IP Server:{host.AddressList[1]})");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            Random rand = new Random();
            int id = ++NextIdNewClient;
            Player custom = new Player();
            custom.tcp.Connect(_client);
            players.Add(id, custom);

            return;
        }

        public static async System.Threading.Tasks.Task<bool> DisconnectServer()
        {
            try
            {
                List<System.Threading.Tasks.Task<bool>> tasks = new List<System.Threading.Tasks.Task<bool>>();
                foreach (KeyValuePair<int, Player> keyValue in players)
                {
                    try
                    {
                        tasks.Add(keyValue.Value.tcp.DisconnectPlayer());
                    }
                    catch (Exception _ex)
                    {
                        Console.WriteLine(_ex.Message);
                        Console.WriteLine(_ex.StackTrace);
                    }
                }

                try
                {
                    await System.Threading.Tasks.Task.WhenAll(tasks);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex.Message);
                    Console.WriteLine(_ex.StackTrace);
                }

                Console.WriteLine("Server disconnected");
            }
            catch (Exception _ex)
            {
                Console.WriteLine("RICENAY: Serious mistake in method DisconnectServer!");
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.StackTrace);
            }

            return true;
        }
    }
}