using System.Collections;
using System.Collections.Generic;

namespace NetworkMaker
{
    public class NMLanInfoConnection
    {
        public string GetIPAddress
        {
            get { return ipAddress; }
        }
        public int GetPort
        {
            get { return port; }
        }

        string ipAddress;
        int port;

        public NMLanInfoConnection()
        {
            //empty ctor for instantiation;
        }
        public NMLanInfoConnection(string fromAddress, string data)
        {
            ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - (fromAddress.LastIndexOf(":") + 1));
            string recievedPort = data.Substring(data.LastIndexOf(":") + 1, data.Length - (data.LastIndexOf(":") + 1));

            port = 7777;
            int.TryParse(recievedPort, out port);
        }
    }
}
