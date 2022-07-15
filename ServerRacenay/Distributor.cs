using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    internal static class Distributor
    {
        public static bool LOCKED; // if true, it is forbidden to enter and exit from waiting queue
        public const int COEFF_GOLD_FUND = 1;
        public const int COEFF_SILVER_FUND = 25;

        public static void Spread()
        {
            while (true)
            {
                Console.WriteLine("Distributor.Spread()");

                LOCKED = true;


                try
                {
                    List<Action> postActions = new List<Action>();



                    // at first qolden queue, hah
                    int coeffBet = 1;
                    foreach (var kv in Server.goldenQueue)
                    {
                        int sumBet = coeffBet * COEFF_GOLD_FUND;

                        for (int i = 0; i < kv.Value.Count; i++)
                        {
                            if (i + 1 < kv.Value.Count && kv.Value[i].id_user != -1 && kv.Value[i + 1].id_user != -1)
                            {
                                new Race(new object[] { "GoldBolt", sumBet * 2 }).Init(kv.Value[i], kv.Value[i + 1]);
                                kv.Value[i].personalData.GoldBolt -= sumBet;
                                kv.Value[i + 1].personalData.GoldBolt -= sumBet;
                                i++;
                                continue;
                            }

                            if (kv.Value[i].id_user == -1)
                                postActions.Add(() => kv.Value.Remove(kv.Value[i]));

                            if (kv.Value[i + 1].id_user == -1)
                                postActions.Add(() => kv.Value.Remove(kv.Value[i + 1]));
                        }

                        coeffBet++;
                    }

                    foreach (Action a in postActions)
                        a();

                    postActions.Clear();



                    // then silver queue
                    coeffBet = 1;
                    foreach (var kv in Server.silverQueue)
                    {
                        int sumBet = coeffBet * COEFF_SILVER_FUND;

                        for (int i = 0; i < kv.Value.Count; i++)
                        {
                            if (i + 1 < kv.Value.Count && kv.Value[i].id_user != -1 && kv.Value[i + 1].id_user != -1)
                            {
                                new Race(new object[] { "SilverBolt", sumBet * 2 }).Init(kv.Value[i], kv.Value[i + 1]);
                                kv.Value[i].personalData.SilverBolt -= sumBet;
                                kv.Value[i + 1].personalData.SilverBolt -= sumBet;
                                i++;
                                continue;
                            }

                            if (kv.Value[i].id_user == -1)
                                postActions.Add(() => kv.Value.Remove(kv.Value[i]));

                            if (kv.Value[i + 1].id_user == -1)
                                postActions.Add(() => kv.Value.Remove(kv.Value[i + 1]));
                        }

                        coeffBet++;
                    }

                    foreach (Action a in postActions)
                        a();

                    postActions.Clear();

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                LOCKED = false;
                Thread.Sleep(3_000);
            }
        }
    }
}
