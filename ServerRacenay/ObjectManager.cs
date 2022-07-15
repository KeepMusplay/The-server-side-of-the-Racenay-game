using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    // responsible for identifying unique objects
    internal static class ObjectManager
    {
        static System.Random rnd = new System.Random();



        // player cars ( 0 - 29 prefabs )
        public static List<int> idsPlayerCars { get; } = new List<int>()
        {
            0, // car, when player just reg in game
        };



        // traffic cars ( 30 - 59 prefabs )
        static List<int> idsTrafficCars = new List<int>()
        {
            30, // ...
        };

        public static int GetRandomPrefabTrafficCar() => idsTrafficCars[rnd.Next(0, idsTrafficCars.Count)]; // range traffic cars



        // plot of territory ( 60 - 99 prefabs )
        static List<int> idTerrains = new List<int>()
        {
            60, // ...
        };

        public static int GetRandomPrefabTerrain() => idsTrafficCars[rnd.Next(0, idTerrains.Count)]; // range terrains
    }
}
