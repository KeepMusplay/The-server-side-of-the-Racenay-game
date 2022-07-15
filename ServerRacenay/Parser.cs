using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ServerRacenay
{
    // main class responsible for receive query from player
    // and subsequent handle his and extradition answer to player
    // format response and request: json
    // necessarily parameters: type
    class Parser
    {
        Player player;


        public Parser(Player _p) => this.player = _p;


        // main method
        public void Parse(string action)
        {
            dynamic obj = JObject.Parse(action);
            try
            {
                player.tcp.resp = Handler.Handle(obj, player);
                return;
            } 
            catch(Exception ex)
            {
                player.tcp.resp = $"{{\"error\": \"Неизвестная ошибка\"}}";
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return;
            }
        }
    }
}
