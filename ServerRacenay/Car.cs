using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    internal class Car : Object, IMoveable, IControllable
    {
        public Player driver; // owner this car

        #region [ system characteristics ]
        readonly int maxSpeed;
        readonly int acceleration;
        readonly int lateralSpeed;
        #endregion


        int Pedal = 0;  // 1 - gas, 2 - brake
        int Rudder = 0; // left-right controller , 1 - left, 2 - fight


        #region [ controller methods ]
        public void WringPedal() => Pedal = 0;
        public void PressGas() => Pedal = 1;
        public void PressBrake() => Pedal = 2;
        public void WringRudder() => Rudder = 0;
        public void TurnLeftRudder() => Rudder = 1;
        public void TurnRigthRudder() => Rudder = 2;
        #endregion

        int currentSpeed = 0;
        public System.Dynamic.ExpandoObject Drive()
        {
            return Drive(this.Pedal, this.Rudder, ref this.currentSpeed);
        }

        /// <param name="Pedal">info about pedals</param>
        /// <param name="Rudder">info about rudder</param>
        /// <returns>json string with base parameters this machine</returns>
        private System.Dynamic.ExpandoObject Drive(int Pedal, int Rudder, ref int currentSpeed)
        {
            if (Pedal == 0 && Rudder == 0)
            {
                currentSpeed -= (int)(acceleration * IMoveable.coeffBrake);
            }
            else if (Pedal == 0 && Rudder == 1)
            {
                currentSpeed -= (int)(acceleration * IMoveable.coeffBrake);
                LateralMove(Side.Left);
            }
            else if (Pedal == 0 && Rudder == 2)
            {
                currentSpeed -= (int)(acceleration * IMoveable.coeffBrake);
                LateralMove(Side.Right);
            }
            else if (Pedal == 1 && Rudder == 0)
            {
                Gas(ref currentSpeed);
            }
            else if (Pedal == 1 && Rudder == 1)
            {
                Gas(ref currentSpeed);
                LateralMove(Side.Left);
            }
            else if (Pedal == 1 && Rudder == 2)
            {
                Gas(ref currentSpeed);
                LateralMove(Side.Right);
            }
            else if (Pedal == 2 && Rudder == 0)
            {
                Brake(ref currentSpeed);
            }
            else if (Pedal == 2 && Rudder == 1)
            {
                Brake(ref currentSpeed);
                LateralMove(Side.Left);
            }
            else if (Pedal == 2 && Rudder == 2)
            {
                Brake(ref currentSpeed);
                LateralMove(Side.Right);
            }
            else throw new Exception($"Неизвестная комбинация!({Pedal}, {Rudder})");

            currentSpeed = currentSpeed < 0              ? 0              : currentSpeed;
            currentSpeed = currentSpeed > this.maxSpeed  ? this.maxSpeed  : currentSpeed;

            Move(currentSpeed);


            return GetData();
        }


        #region movement methods
        void Move(int speed)
        {
            this.position[0] += speed;
        }

        int indexCoeffAcc = 0;
        void Gas(ref int currentSpeed)
        {
            if (indexCoeffAcc < IMoveable.coeffsAccFromTime.Length - 1)
                currentSpeed += (int)(acceleration * IMoveable.coeffsAccFromTime[indexCoeffAcc++]);
        }

        void Brake(ref int currentSpeed)
        {
            currentSpeed -= (int)(acceleration * IMoveable.coeffBrake);
            indexCoeffAcc = 0;
        }

        void LateralMove(Side side)
        {
            switch(side)
            {
                case Side.Left: this.position[1] -= lateralSpeed;  break;
                case Side.Right: this.position[1] += lateralSpeed; break;
            }

            if (Race.LEFT_BORDER > this.position[1] - this.lengthAside)
                this.position[1] = Race.LEFT_BORDER + this.lengthAside;
            else if (Race.RIGHT_BORDER < this.position[1] + this.lengthAside)
                this.position[1] = Race.RIGHT_BORDER - this.lengthAside;
        }
        #endregion

        



        // return main data this obj, such as position, id, idPrefab and etc..
        public override System.Dynamic.ExpandoObject GetData()
        {
            dynamic obj = base.GetData();
            obj.instance = this;

            return obj;
        }



        // for real drivers
        public Car(int[] position, int lengthAside, int lengthForward, int idPrefab, int maxSpeed, Player player) : base(position, lengthAside, lengthForward, idPrefab)
        {
            this.maxSpeed = maxSpeed;
            this.acceleration = (int)(maxSpeed * 0.05);
            int ls = (int)(this.maxSpeed * 0.05);
            this.lateralSpeed = ls > 10 ? 10 : ls;
            this.driver = player;
        }
    }
}
