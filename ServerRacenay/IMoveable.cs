using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    public interface IMoveable
    {
        static readonly float[] coeffsAccFromTime = new float[] { 0.08f, 0.2f, 0.4f, 0.75f, 0.93f, 1.15f, 1.34f, 1.40f, 1.5f, 1.45f, 1.40f, 1.35f, 1.20f, 1.15f, 1.10f, 1.00f, 0.90f, 0.8f, 0.75f };
        const float coeffBrake = 1f;

        public System.Dynamic.ExpandoObject Drive(); // main method for control ride
    }

    public enum Side
    {
        Left = 0,
        Right = 1
    }
}
