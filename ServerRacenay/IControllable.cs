using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    public interface IControllable
    {
        // [ controller methods ]
        public void WringPedal();
        public void PressGas();
        public void PressBrake();
        public void WringRudder();
        public void TurnLeftRudder();
        public void TurnRigthRudder();
    }
}
