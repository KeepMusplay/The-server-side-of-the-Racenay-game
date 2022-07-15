using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServerRacenay
{
    class Player
    {
        public TCP tcp { get; }
        public int id_user { get; set; } = -1; // == -1 if user not auth
        public string nickname { get; set; }
        public PlayerData personalData { get; set; }

        #region Notifications
        public dynamic? notification
        {
            get
            {
                dynamic? n = _notification;
                _notification = null;

                return n;
            }
            set
            {
                if (_notification != null)
                    return;
                else
                    this._notification = value;
            }
        }
        System.Dynamic.ExpandoObject? _notification = null;
        #endregion




        ModelCar car; // current selected car
        Room room; // current room in which he being
        Race race; // current race



        public Room? GetActiveRoom() => this.room;
        public void SetActiveRoom(Room room) => this.room = room;



        public ModelCar GetCurrentCar() => this.car;
        public void SetCurrentCar(int idCar) => this.car = this.personalData.modelsCars[idCar];



        public Race? GetActiveRace() => this.race;
        public void SetActiveRace(Race race) => this.race = race;



        public Player() => this.tcp = new TCP(this);
    }



    class TCP
    {
        public static int dataBufferSize = 3096000;

        public TcpClient socket;
        public NetworkStream stream;
        public Player player;
        public Parser parser;

        public string resp
        {
            set
            {
                responce = Encoding.UTF8.GetBytes(value);

                WriteResponce();
            }
        }

        private byte[] receiveBuffer;
        private byte[] data;
        private byte[] responce;

        private string action;
        private int byteLength;

        public bool waitDis { get; private set; } = false;

        public TCP(Player _player)
        {
             parser = new Parser(_player);
        }

        public async void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receiveBuffer = new byte[dataBufferSize];

            int byteLength = 0;
            try
            {
                byteLength = await stream.ReadAsync(receiveBuffer, 0, dataBufferSize);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.StackTrace);
                return;
            }
            ReceiveCallback(byteLength);
            // TODO: send welcome packet
        }

        private async void ReceiveCallback(int _byteLength)
        {
            try
            {
                if (_byteLength <= 0)
                {
                    // save information in DB
                    await DisconnectPlayer();

                    // TODO: disconnect
                    return;
                }

                data = new byte[_byteLength];
                Array.Copy(receiveBuffer, data, _byteLength);
                action = Encoding.UTF8.GetString(data);
                Console.WriteLine($"Action: {action.Substring(0, action.Length <= 107 ? action.Length : 107)} commited {socket.Client.RemoteEndPoint}");

                // parse action transmitting parser string with data
                Thread thread = new Thread(() => parser.Parse(action));
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
                // TODO: disconnect
            }
        }

        private async void WriteResponce()
        {
            try
            {
                // TODO: send response client
                await stream.WriteAsync(responce, 0, responce.Length);

                // TODO: handle data
                byteLength = await stream.ReadAsync(receiveBuffer, 0, dataBufferSize);
                ReceiveCallback(byteLength);
            }
            catch (Exception _ex)
            {
                if (_ex.Message.IndexOf("Unable to read data from the transport connection") != -1)
                {
                    return;
                }
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.StackTrace);
            }
        }

        public void SaveDataInDB()
        {
            if (player.id_user != 0)
            {
                try
                {
                    QUERY.UPDATE("users", new string[] { "PlayerData" }, new string[] { JsonSerializer.Serialize(player.personalData), player.id_user.ToString() }, "WHERE id = @id");

                    //Console.WriteLine("Data in DB was updated");
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex.Message);
                    Console.WriteLine(_ex.StackTrace);
                }
            }
        }
        public async Task<bool> DisconnectPlayer() // (!)
        {
            if (waitDis)
            {
                await Task.Delay(7_000);
                return false;
            }

            waitDis = true;
            Console.WriteLine("Through 5 sec disconnect client(id_user:" + player.id_user + ")");
            await Task.Delay(3_000);
            socket.Close();
            await Task.Delay(2_000);

            //Console.WriteLine($"Was: Count allClients: {Server.allClients.Count}, Count IdsAndClients: {Server.IdsAndClients.Count}, Count onlinePlayers: {Server.onlinePlayers.Count}");
            //Console.WriteLine("------------------------------");

            int id_user_copy = player.id_user;

            try
            {
                SaveDataInDB();
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.StackTrace);
            }

            if (Server.players.ContainsKey(id_user_copy))
            {
                Server.players.Remove(id_user_copy);
            }

            return true;
        }
    }



    // data player, incoming from db and outcoming to db
    [Serializable]
    class PlayerData
    {
        public int GoldBolt { get; set; }
        public int SilverBolt { get; set; }
        public int Petrol { get; set; }
        public int LastUpdatePetrol { get; set; } // timestamp since last update count petrol


        public List<ModelCar> modelsCars { get; }



        public PlayerData(List<ModelCar> modelsCars)
        {
            this.GoldBolt = 0;
            this.SilverBolt = 1000;
            this.Petrol = 100;
            this.LastUpdatePetrol = TimeManager.GetTimestamp();
            this.modelsCars = modelsCars;
        }
    }
}