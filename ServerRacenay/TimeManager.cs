using System;

namespace ServerRacenay
{
    class TimeManager
    {
        public static int GetTimestamp()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int) (DateTime.Now - origin).TotalSeconds;
        }

        public static int GetTimestamp(DateTime _time)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int) (_time - origin).TotalSeconds;
        }

        public static string GetStringTime(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp).ToString();
        }
        public static DateTime GetDateTime(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
