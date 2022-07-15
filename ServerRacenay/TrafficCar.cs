using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    // bot
    internal class TrafficCar : Object
    {
        private const int MIN_SPEED_TRAFFIC_CAR = 30; // ?
        private const int MAX_SPEED_TRAFFIC_CAR = 70; // ?
        private static System.Random rnd = new Random();


        readonly int maxSpeed;

        public bool Stopped { get; private set; }





        // return main data this obj, such as position, id, idPrefab and etc..
        public override System.Dynamic.ExpandoObject GetData()
        {
            dynamic obj = base.GetData();
            obj.instance = this;

            return obj;
        }



        public TrafficCar(global::System.Int32[] position, global::System.Int32 lengthAside, global::System.Int32 lengthForward, global::System.Int32 idPrefab) : base(position, lengthAside, lengthForward, idPrefab)
        {
            Stopped = false; // car stops when she beyond edges XPosLastPlayer
            maxSpeed = rnd.Next(MIN_SPEED_TRAFFIC_CAR, MAX_SPEED_TRAFFIC_CAR);
        }
    }
}
