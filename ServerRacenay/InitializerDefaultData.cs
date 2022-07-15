using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace ServerRacenay
{
    class InitializerDefaultData
    {
        static string PlayerDataJson;

        public static void Initialize()
        {
            PlayerDataJson = JsonSerializer.Serialize(new PlayerData(new List<ModelCar>()
            {
                new ModelCar(0, "NameCar0", new int[] { 25, 20, 27 }, 100, 100), // ?
            }));

            Console.WriteLine("Default data initialized!");
        }

        public static string GetNewPlayerData() => PlayerDataJson;
    }
}
