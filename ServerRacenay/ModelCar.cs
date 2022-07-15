using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    [Serializable]
    sealed internal class ModelCar
    {
        public int UniqueNumCar { get; } // id prefab(start with zero)
        public string NameModelCar { get; }
        public int[] Characteristics { get; private set; } // speed, maneuverability, control and etc...
        public int[] Tuning { get; private set; } // color, spoiler and etc...
        public bool Gained { get; set; }
        public int StageUpgrade { get; private set; }

        public int lengthAside { get; }
        public int lengthForward { get; }



        private const int COUNT_STAGES_UPGRADE = 16;



        public void UpgradeCar()
        {
            // up speed
            this.Characteristics[0] += (int)Math.Floor( (COUNT_STAGES_UPGRADE - StageUpgrade) * 0.0025 * Characteristics[0] );

            // up maneuverability
            this.Characteristics[1] += (int)Math.Floor( (COUNT_STAGES_UPGRADE - StageUpgrade) * 0.0025 * Characteristics[1] );

            // up control
            this.Characteristics[2] += (int)Math.Floor( (COUNT_STAGES_UPGRADE - StageUpgrade) * 0.0025 * Characteristics[2] );

            this.StageUpgrade += 1;
        }



        public ModelCar(int uniqueNumCar, string nameModelCar, int[] Characteristics, int lengthAside, int lengthForward)
        {
            if (Characteristics.Length != 3)
                throw new Exception("Массив 'Characteristics' должен содержать обязательно только 3 элемента");

            UniqueNumCar = uniqueNumCar;
            NameModelCar = nameModelCar;
            this.Characteristics = Characteristics;
            this.Tuning = new int[] { 0 }; // [ grey color ]
            Gained = false;
            StageUpgrade = 0;
            this.lengthAside = lengthAside;
            this.lengthForward = lengthForward;
        }
    }
}
