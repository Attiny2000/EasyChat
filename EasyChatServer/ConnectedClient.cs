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
        public TcpClient TcpClient { get; set; }
        public ChatRoomWorker currentChatRoomWorker { get; set; }

        public ConnectedClient(User user, TcpClient client)
        {
            this.User = user;
            this.TcpClient = client;
        }
    }
}
