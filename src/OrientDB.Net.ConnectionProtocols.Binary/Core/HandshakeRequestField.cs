using System;
using System.Collections.Generic;
using System.Text;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    class HandshakeRequestField
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public int PoolSize { get; set; } = 1;
    }
}