using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Valhalla
{
    public static class WebSocketDefaults
    {
        /// <summary>
        /// Assigns the localhost ipaddress
        /// </summary>
        public static IPAddress host = Dns.GetHostEntry("localhost").AddressList[0];

        /// <summary>
        /// The port to use for the the communication.
        /// </summary>
        public static int port = 30159;
    }
}
