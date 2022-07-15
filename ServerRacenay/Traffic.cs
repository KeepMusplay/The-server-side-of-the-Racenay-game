using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    internal class Traffic
    {
        public delegate void Generator(int idPlate, dynamic data);
        public event Generator GetNextPlate;
        void GenerateNewTraffic(int idPlate, dynamic data)
        {
            // here add new traffic in race
        }

        

        public Traffic()
        {
            Generator generator;
            generator = GenerateNewTraffic;
            this.GetNextPlate += generator;
        }
    }
}
