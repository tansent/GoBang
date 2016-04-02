using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace GoBangServer
{
    class User
    {
        public User(TcpClient tc)
        {
            this.client = tc;
            this.ns = client.GetStream();
            sr = new StreamReader(ns, System.Text.Encoding.Default);
            sw = new StreamWriter(ns, System.Text.Encoding.Default);
        }

        public readonly TcpClient client;
        public readonly StreamWriter sw;
        public readonly StreamReader sr;
        public readonly NetworkStream ns;
        public string userName;

    }
}
