using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyChatServer
{
    class ConnectedClient
    {
        public User User { get; set; }
        public TcpClient Client { get; set; }

        public ConnectedClient(User user, TcpClient client)
        {
            User = User;
            Client = client;
        }
    }
}
