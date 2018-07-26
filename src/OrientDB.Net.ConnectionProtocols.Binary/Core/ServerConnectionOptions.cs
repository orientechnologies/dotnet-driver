using System.Net;

namespace OrientDB.Net.ConnectionProtocols.Binary.Core
{
    public class ServerConnectionOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string  Type { get; set; }
        public int Port { get; set; }
        public int PoolSize { get; set; } = 1;//Should be 10, changed only for functional purpose

    }
}