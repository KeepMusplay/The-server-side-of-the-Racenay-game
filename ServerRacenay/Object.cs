using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    internal class Object
    {
        public int id { get; } // unique id obj

        protected int[] position; // (x, y)
        protected int lengthAside;
        protected int lengthForward;
        protected int idPrefab; // id obj on client

        private dynamic data;



        // return main data this obj, such as position, id, idPrefab
        public virtual System.Dynamic.ExpandoObject GetData()
        {
            this.data.id = this.id;
            this.data.idPrefab = this.idPrefab;
            this.data.pos = this.position;
            this.data.instance = this;

            return this.data;
        }


        public Object(int[] position, int lengthAside, int lengthForward, int idPrefab)
        {
            this.position = position;
            this.lengthAside = lengthAside;
            this.lengthForward = lengthForward;
            this.id = int.Parse($"{this.position[0]}_{this.position[1]}");
            this.idPrefab = idPrefab;
            this.data = new System.Dynamic.ExpandoObject();
        }
    }
}
