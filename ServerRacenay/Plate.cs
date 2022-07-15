using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    internal class Plate
    {
        private int startPosX;


        public System.Dynamic.ExpandoObject content;

        public string jsonContent
        {
            get 
            {
                if (_content == null)
                    throw new Exception($"У плиты {startPosX} нет данных");
                else return _content;
            }
            set
            {
                this._content = value;
            }
        }
        private string _content = null;

        public bool isVisited = false;



        public Plate(int startPosX) => this.startPosX = startPosX;
    }
}
