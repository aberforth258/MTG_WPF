using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.NetworkInformation;

namespace MTG_WPF
{
    public static class GlobalParameters
    {
        public static bool hasInternetConnection { get; private set; }
        public const int mtgCardHeight = 88;
        public const int mtgCardWidth = 63;
        public const double mtgCardTitleHeightRatio = 0.2; // 1/3 of image



        public static bool CheckInternetConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                hasInternetConnection = reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                hasInternetConnection = false;
            }

            return hasInternetConnection;
        }
    }
}
