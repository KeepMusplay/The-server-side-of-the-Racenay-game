using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerRacenay
{
    class Race
    {
        // look at the doc.
        public const int LEFT_BORDER = 0; // left edge per y // ?
        public const int RIGHT_BORDER = 100; // rigth edge per y // ?
        public const float COEFF_SPEED = 1.0f; // research from unity // ?
        public const int LENGTH_PLATE = 100; // ?
        public const int WIDTH_PLATE = 100; // ?
        public const int MAX_COUNT_PLATES = 100; // ?
        public const int TIME_RACE = 400; // ms, //?
        public const int WIDTH_ROAD = 20; // ?
        public const int MAX_COUNT_CARS_ON_LINE = 5; // ?
        public const int Y_POS_CENTER_LINE_1 = 1; // ?
        public const int Y_POS_CENTER_LINE_2 = 2; // ?
        public const int Y_POS_CENTER_LINE_3 = 3; // ?
        public const int Y_POS_CENTER_LINE_4 = 4; // ?
        public const int Y_POS_CENTER_TERRAIN_0 = 0; // ?
        public const int Y_POS_CENTER_TERRAIN_5 = 5; // ?
        public const int X_PROFIT_MIN_NEXT_PROFIT_ON_LINE = 10; // ?
        public const int X_PROFIT_NEXT_CENTER_ON_TERRAIN = 250; // ?
        public const int MAX_DISTANCE_BETWEEN_TRAFFIC_CARS = 1; // ?
        public const int X_POS_LAST_PLATE = MAX_COUNT_PLATES * LENGTH_PLATE;
        public const int DIFF_BETWEEN_LAST_CAR_AND_BOT_FOR_DELETE = 100; // ?
        public const int X_POS_FINISH = (MAX_COUNT_PLATES - 1) * LENGTH_PLATE;
        public const float PERCENT_WIN_BET = 0.7f;
        public const float PERCENT_DEFEAT_BET = 0.3f;
        public static readonly int[] POS_START_PLAYER_1 = new int[] {  0,  0 }; // ?
        public static readonly int[] POS_START_PLAYER_2 = new int[] { 10, 10 }; // ?



        public int Status; // 0 - waiting, 1 - started rice, 2 - rice, 3 - rice completed



        dynamic data;




        // <idPlate, Plate>
        Dictionary<int, Plate> plates;

        int XPosLastPlayer;


        Traffic traffic;


        public async void Init(Room room)
        {
            if (Status >= 0)
                throw new Exception("Гонка уже была инициализирована!");

            foreach (var p in room.players)
                if (p == null)
                    throw new Exception("Недостаточно гонщиков");


            // success
            room.locked = true;



            room.players[0].SetActiveRace(this);
            room.players[1].SetActiveRace(this);


            
            // create environment ...
            // 1. create plates
            for (int i = 0; i < MAX_COUNT_PLATES; i++)
            {
                int idPlate = i * LENGTH_PLATE;
                this.plates.Add( idPlate, new Plate(idPlate) );
            }

            // 2. generate env for 0 and 1 plates
            Environment.Generate( 0, this.data );
            Environment.Generate( 1 * LENGTH_PLATE, this.data );

            // 3. spawning cars players
            {
                ModelCar car1 = room.players[0].GetCurrentCar();
                var carPlayer1 = new Car(POS_START_PLAYER_1, car1.lengthAside, car1.lengthForward, car1.UniqueNumCar, car1.Characteristics[0], room.players[0]);
                this.data[carPlayer1.id] = carPlayer1.GetData();

                ModelCar car2 = room.players[1].GetCurrentCar();
                var carPlayer2 = new Car(POS_START_PLAYER_2, car2.lengthAside, car2.lengthForward, car2.UniqueNumCar, car2.Characteristics[0], room.players[1]);
                this.data[carPlayer2.id] = carPlayer2.GetData();
            }




            Status = 1;

            // loading every player in map
            await System.Threading.Tasks.Task.Delay(5000);


            await System.Threading.Tasks.Task.Delay(5000); // 5, 4, 3, 2, 1... GO!

            Thread rice = new Thread(() => this.Start());
            rice.IsBackground = true;
            rice.Start();
        }



        public async void Init(Player p1, Player p2)
        {
            if (Status >= 0)
                throw new Exception("Гонка уже была инициализирована!");

            if (p1 == null)
                throw new Exception("Обьекта гонщика 1 не существует");
            else if (p2 == null)
                throw new Exception("Обьекта гонщика 2 не существует");


            // success




            // create environment ...
            // 1. create plates
            for (int i = 0; i < MAX_COUNT_PLATES; i++)
            {
                int idPlate = i * LENGTH_PLATE;
                this.plates.Add(idPlate, new Plate(idPlate));
            }

            // 2. generate env for 0 and 1 plates
            Environment.Generate(0, this.data);
            Environment.Generate(1 * LENGTH_PLATE, this.data);

            // 3. spawning cars players
            {
                ModelCar car1 = p1.GetCurrentCar();
                var carPlayer1 = new Car(POS_START_PLAYER_1, car1.lengthAside, car1.lengthForward, car1.UniqueNumCar, car1.Characteristics[0], p1);
                this.data[carPlayer1.id] = carPlayer1.GetData();

                ModelCar car2 = p2.GetCurrentCar();
                var carPlayer2 = new Car(POS_START_PLAYER_2, car2.lengthAside, car2.lengthForward, car2.UniqueNumCar, car2.Characteristics[0], p2);
                this.data[carPlayer2.id] = carPlayer2.GetData();
            }



            Status = 1;

            // loading every player in map
            await System.Threading.Tasks.Task.Delay(5000);


            await System.Threading.Tasks.Task.Delay(5000); // 5, 4, 3, 2, 1... GO!

            Thread rice = new Thread(() => this.Start());
            rice.IsBackground = true;
            rice.Start();
        }



        bool existWinner = false;
        object[] bet;



        private async void Start() // launch rice
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            int s = 0;
            int currentPlateForUnableSer = 0;
            while (s++ < TIME_RACE)
            {
                sw.Start();

                bool existPlayers = false;



                // conduct manipulation ... 


                // 1. updated position obj and removing unnecessary objects, update contents plates



                var dic = (IDictionary<System.String, System.Object>)this.data;

                var keys = new List<string>(dic.Keys); // ids Objects

                for(int i = 0; i < keys.Count; i++)
                {
                    var expObj = (dynamic)dic[keys[i]];

                    var instance = expObj.instance;

                    if(instance is Car)
                    {
                        existPlayers = true;

                        expObj = instance.Drive();

                        if (expObj.pos[0] < XPosLastPlayer)
                            XPosLastPlayer = expObj.pos[0];

                        if(X_POS_FINISH < expObj.pos[0])
                        {
                            if (!existWinner) // win player!
                            {
                                existWinner = true;

                                if(this.bet != null)
                                {
                                    string nameCurrency = (string)bet[0];
                                    int count = (int)bet[1];

                                    if (nameCurrency == "GoldBolt")
                                        instance.driver.personalData.GoldBolt += count * PERCENT_WIN_BET;
                                    else
                                        instance.driver.personalData.SilverBolt += count * PERCENT_WIN_BET;
                                }
                            }
                            else // 2 place =(
                            {
                                if (this.bet != null)
                                {
                                    string nameCurrency = (string)bet[0];
                                    int count = (int)bet[1];

                                    if (nameCurrency == "GoldBolt")
                                        instance.driver.personalData.GoldBolt += count * PERCENT_DEFEAT_BET;
                                    else
                                        instance.driver.personalData.SilverBolt += count * PERCENT_DEFEAT_BET;
                                }
                            }

                            instance.driver.SetActiveRace(null);
                            dic.Remove(keys[i]);
                            continue;
                        }
                    }
                    else if(instance is TrafficCar)
                    {
                        if (expObj.pos[0] < XPosLastPlayer - DIFF_BETWEEN_LAST_CAR_AND_BOT_FOR_DELETE || expObj.pos[0] > X_POS_LAST_PLATE)
                        {
                            dic.Remove(keys[i]);
                            continue;
                        }

                        expObj = instance.Drive();
                    }
                    else if(instance is ServerRacenay.Object)
                        if (expObj.pos[0] < XPosLastPlayer - DIFF_BETWEEN_LAST_CAR_AND_BOT_FOR_DELETE || expObj.pos[0] > X_POS_LAST_PLATE)
                        {
                            dic.Remove(keys[i]);
                            continue;
                        }

                    int indexPlate = (int)Math.Floor((decimal)(expObj.pos[0] / LENGTH_PLATE));
                }


                // 2. unable serialization visited platforms
                if(XPosLastPlayer - DIFF_BETWEEN_LAST_CAR_AND_BOT_FOR_DELETE > currentPlateForUnableSer * LENGTH_PLATE)
                    plates[currentPlateForUnableSer++].isVisited = true;





                Console.WriteLine($"Заняло {sw.ElapsedMilliseconds}ms на обработку кадра");
                int diff = 100 - (int)sw.ElapsedMilliseconds;
                sw.Reset();

                if (!existPlayers)
                    return;

                await System.Threading.Tasks.Task.Delay((diff <= 0 ? 0 : diff)); // 10 times at one second update data
            }
        }



        ~Race() => Console.WriteLine("Обьект гонки был уничтожен");



        public Race(object[] bet)
        {
            if (bet.Length != 2)
                throw new Exception("Массив 'bet' должен содержать только два параметра формата 'валюта', 'кол-во'");

            this.bet = bet;

            this.data = new global::System.Dynamic.ExpandoObject();
            this.plates = new Dictionary<int, Plate>();
            this.Status = 0;
            traffic = new Traffic();
        }

        public Race()
        {
            this.data = new global::System.Dynamic.ExpandoObject();
            this.plates = new Dictionary<int, Plate>();
            this.Status = 0;
            traffic = new Traffic();
            bet = null;
        }
    }
}
