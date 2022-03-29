using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Null.TocReborn.Pro.Server.Model
{
    internal class AppConfig
    {
        public ListeningConfig HttpListening = new ListeningConfig()
        {
            Address = "0.0.0.0",
            Port = 80,
        };

        public ListeningConfig TcpListening = new ListeningConfig()
        {
            Address = "0.0.0.0",
            Port = 80,
        };


        public class ListeningConfig
        {
            public string Address = "0.0.0.0";
            public int Port = 0;
        }

        public static AppConfig Default => new AppConfig()
        {
            HttpListening = new ListeningConfig()
            {
                Address = "0.0.0.0",
                Port = 80,
            },

            TcpListening = new ListeningConfig()
            {
                Address = "0.0.0.0",
                Port = 2022
            }
        };
    }
}
