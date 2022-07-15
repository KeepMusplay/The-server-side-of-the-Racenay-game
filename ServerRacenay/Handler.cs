using System;
using System.Dynamic;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace ServerRacenay
{
    internal class Handler
    {
        static object locked = new object();



        public static string Handle(dynamic obj, Player player) => obj.type switch
        {
            "reg" =>
            #region reg
            ((Func<string>)(() =>
            {
                // input: nickname, password
                // output: type, result, nickname, PlayerData, idCurrentCar

                dynamic response = new ExpandoObject();
                response.type = "reg";



                // check that name is unusing
                MySqlCommand cmd;
                QUERY.SELECT("users", new string[] { "id" }, new string[] { obj.nickname }, "WHERE nickname = @nickname", out cmd);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                }


                // create new row for new user
                QUERY.INSERT("users", new string[] { "nickname", "password", "PlayerData", "currentCarId", "currentRoomId" }, new string[] { obj.nickname, obj.password, InitializerDefaultData.GetNewPlayerData(), "0", "0" }, "");


                // getting id new user
                int idNewUser = -1;
                MySqlCommand cmd1;
                QUERY.SELECT("users", new string[] { "id" }, new string[] { obj.nickname }, "WHERE nickname = @nickname", out cmd1);
                using (DbDataReader reader = cmd1.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                    else while (reader.Read())
                            idNewUser = reader.GetInt32(reader.GetOrdinal("id"));
                }

                if (idNewUser == -1)
                {
                    response.result = 0;
                    return JsonSerializer.Serialize(response);
                }


                // then authorization process..
                response.result = 1;
                response.nickname = obj.nickname;
                response.PlayerData = InitializerDefaultData.GetNewPlayerData();
                response.idCurrentCar = 0;

                player.personalData = JsonSerializer.Deserialize<PlayerData>(response.PlayerData);
                player.id_user = idNewUser;
                player.nickname = response.nickname;
                player.SetCurrentCar(0);

                return JsonSerializer.Serialize(response);
            }))(),
            #endregion



            "check" =>
            #region check
            ((Func<string>)(() =>
            {
                // input: nickname
                // output: type, result

                dynamic response = new ExpandoObject();
                response.type = "check";



                MySqlCommand cmd;
                QUERY.SELECT("users", new string[] { "id" }, new string[] { obj.nickname }, "WHERE nickname = @nickname", out cmd);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                }

                response.result = 1;
                return JsonSerializer.Serialize(response);


            }))(),
            #endregion



            "auth" =>
            #region auth
            ((Func<string>)(() =>
            {
                // input: nickname, password
                // output: type, result, nickname, PlayerData, idCurrentCar, Room

                dynamic response = new ExpandoObject();
                response.type = "auth";



                // getting user
                int idNewUser = -1;
                MySqlCommand cmd;
                QUERY.SELECT("users", new string[] { "id" }, new string[] { obj.nickname, obj.password }, "WHERE nickname = @nickname AND password = @password", out cmd);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                    else while (reader.Read())
                            idNewUser = reader.GetInt32(reader.GetOrdinal("id"));
                }

                if (idNewUser == -1)
                {
                    response.result = 0;
                    return JsonSerializer.Serialize(response);
                }



                // getting data this user
                MySqlCommand cmd1;
                QUERY.SELECT("users", new string[] { "id", "PlayerData", "idCurrentCar", "idRoom" }, new string[] { obj.nickname, obj.password }, "WHERE id = @id", out cmd1);
                using (DbDataReader reader = cmd1.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                    else while (reader.Read())
                        {
                            response.result = 1;
                            response.nickname = obj.nickname;
                            response.PlayerData = reader.GetString(reader.GetOrdinal("PlayerData"));
                            response.idCurrentCar = reader.GetInt32(reader.GetOrdinal("idCurrentCar"));
                            response.Room = 0;
                            int idRoom = reader.GetInt32(reader.GetOrdinal("idRoom"));
                            if (idRoom > 0)
                            {
                                try
                                {
                                    if (Server.rooms[idRoom] != null)
                                    {
                                        dynamic r = new ExpandoObject();
                                        r.nickname1 = Server.rooms[idRoom].players[0].nickname;
                                        r.nickname2 = Server.rooms[idRoom].players[1].nickname;

                                        response.Room = r;

                                        player.SetActiveRoom(Server.rooms[idRoom]);
                                    }
                                } catch { }
                            }

                            player.personalData = JsonSerializer.Deserialize<PlayerData>(response.PlayerData);
                            player.id_user = reader.GetInt32(reader.GetOrdinal("id"));
                            player.nickname = response.nickname;
                            player.SetCurrentCar(response.idCurrentCar);

                            return JsonSerializer.Serialize(response);
                        }
                }



                response.result = 0;
                return JsonSerializer.Serialize(response);
            }))(),
            #endregion



            "create_room" =>
            #region create_room
            ((Func<string>)(() =>
            {
                // input: 
                // output: type, result

                dynamic response = new ExpandoObject();
                response.type = "create_room";


                // failed
                if (player.GetActiveRoom() != null)
                {
                    response.result = 0;
                    return JsonSerializer.Serialize(response);
                }


                // success
                lock (locked)
                {
                    int idRoom;
                    Server.rooms.Add(new Room(idRoom = Server.rooms.Count, 2, player, 0));
                    player.SetActiveRoom(Server.rooms[idRoom]);
                }


                response.result = 1;

                return JsonSerializer.Serialize(response);
            }))(),
            #endregion



            "invite_player" =>
            #region invite_player
            ((Func<string>)(() =>
            {
                // input: nickname
                // output: type, result

                dynamic response = new ExpandoObject();
                response.type = "invite_player";


                // failed
                if (player.GetActiveRoom() == null)
                {
                    response.result = 0;
                    return JsonSerializer.Serialize(response);
                }


                MySqlCommand cmd;
                QUERY.SELECT("users", new string[1] { "id" }, new string[1] { obj.nickname }, "WHERE nickname = @nickname", out cmd);
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("id"));

                            foreach (var kv in Server.players)
                                if (kv.Value.id_user == id)
                                {
                                    if (kv.Value.GetActiveRoom() == null && kv.Value.GetActiveRace() == null)
                                    {
                                        dynamic not = new ExpandoObject();

                                        not.type = "invite";
                                        not.nickname = player.nickname;
                                        not.timeExpire = TimeManager.GetTimestamp() + 10;
                                        not.idRoom = player.GetActiveRoom()?.idRoom;


                                        kv.Value.notification = JsonSerializer.Serialize(not);


                                        response.result = 1;

                                        return JsonSerializer.Serialize(response);
                                    }
                                }
                                else break;
                        }
                    }
                    else
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                }


                response.result = 0;

                return JsonSerializer.Serialize(response);
            }))(),
            #endregion



            "accept_invite" =>
            #region accept_invite
            ((Func<string>)(() =>
            {
                // input: idRoom
                // output: type, result

                dynamic response = new ExpandoObject();
                response.type = "accept_invite";


                lock (locked)
                {
                    // failed
                    if (player.GetActiveRoom() != null || player.GetActiveRace() != null || obj.idRoom > Server.rooms.Count || Server.rooms[obj.idRoom] == null || Server.rooms[obj.idRoom].players[0] == null || Server.rooms[obj.idRoom].players[1] == null)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }


                    // succeed
                    Room r = Server.rooms[obj.idRoom];
                    player.SetActiveRoom(r);
                    r.players[1] = player;
                }


                response.result = 1;

                return JsonSerializer.Serialize(response);
            }))(),
            #endregion



            "update_ui" =>
            #region update_ui
            ((Func<string>)(() =>
            {
                // input: 
                // output: type



                // check updated notification
                dynamic? n = player.notification;
                if(n != null)
                    return JsonSerializer.Serialize(n);




                dynamic response = new ExpandoObject();
                response.type = "update_ui";

                response.GoldBolt = player.personalData.GoldBolt;
                response.SilverBolt = player.personalData.SilverBolt;
                int dif = (TimeManager.GetTimestamp(DateTime.Now) - player.personalData.LastUpdatePetrol) / 60;
                if (dif >= 1)
                {
                    player.personalData.LastUpdatePetrol = TimeManager.GetTimestamp(DateTime.Now);
                    player.personalData.Petrol += dif;
                    player.personalData.Petrol = player.personalData.Petrol > 100 ? 100 : player.personalData.Petrol;
                }
                response.Petrol = player.personalData.Petrol;


                return JsonSerializer.Serialize(response);
            }))(),
            // on client invoked every one second
            #endregion



            "get_in_queue" =>
            #region get_in_queue
            ((Func<string>)(() =>
            {
                // input: queue
                // output: type, result

                dynamic response = new ExpandoObject();
                response.type = "get_in_queue";



                int indexInQueue = (int)Math.Floor((decimal)(player.GetCurrentCar().Characteristics[0] / Server.COEFF_SEPARATION));

                if (!Server.goldenQueue.ContainsKey(indexInQueue))
                    throw new Exception($"Очереди {indexInQueue} не существует!");


                // failed
                if (player.GetActiveRoom() != null || player.GetActiveRace() != null || Distributor.LOCKED)
                {
                    response.result = 0;
                    return JsonSerializer.Serialize(response);
                }

                if(obj.queue == "GoldBolt")
                    if(player.personalData.GoldBolt < indexInQueue * Distributor.COEFF_GOLD_FUND)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }
                else
                    if(player.personalData.SilverBolt < indexInQueue * Distributor.COEFF_SILVER_FUND)
                    {
                        response.result = 0;
                        return JsonSerializer.Serialize(response);
                    }


                // succeed
                lock (locked)
                {
                    if (obj.queue == "GoldBolt")
                        Server.goldenQueue[indexInQueue].Add(player);
                    else
                        Server.silverQueue[indexInQueue].Add(player);
                }


                response.result = 1;

                return JsonSerializer.Serialize(response);
            }))(),
            #endregion



            "get_out_queue" =>
            #region get_out_queue
            ((Func<string>)(() =>
            {
                // input: queue
                // output: type, result

                dynamic response = new ExpandoObject();
                response.type = "get_out_queue";



                int indexInQueue = (int)Math.Floor((decimal)(player.GetCurrentCar().Characteristics[0] / Server.COEFF_SEPARATION));

                if (!Server.goldenQueue.ContainsKey(indexInQueue))
                    throw new Exception($"Очереди {indexInQueue} не существует!");


                // failed
                if (Distributor.LOCKED)
                {
                    response.result = 0;
                    return JsonSerializer.Serialize(response);
                }

                // succeed
                lock (locked)
                {
                    if (obj.queue == "GoldBolt")
                        Server.goldenQueue[indexInQueue].Remove(player);
                    else
                        Server.silverQueue[indexInQueue].Remove(player);
                }


                response.result = 1;

                return JsonSerializer.Serialize(response);
            }))(),
            #endregion
        };
    }

    // result: 1 - success, 0 - failed
}
