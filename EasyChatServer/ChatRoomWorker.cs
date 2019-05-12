using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyChatServer
{
    class ChatRoomWorker
    {
        public ChatRoom chatRoom { get; }
        List<ConnectedClient> clients { get; }

        public ChatRoomWorker(ChatRoom chatRoom)
        {
            this.chatRoom = chatRoom;
            clients = new List<ConnectedClient>();
        }
        public async void AddNewActiveMember(ConnectedClient client)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    StreamReader sr = new StreamReader(client.TcpClient.GetStream());
                    while (client.TcpClient.Connected && isClientConnected(client.TcpClient))
                    {
                        string message = sr.ReadLine();
                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            SendToAll(message);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            });
        }
        public void RemoveActiveMember(ConnectedClient client)
        {
            clients.Remove(client);
        }
        public async void SendToAll(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (ConnectedClient c in clients)
                    {
                        if (c.TcpClient.Connected && isClientConnected(c.TcpClient))
                        {
                            StreamWriter sw = new StreamWriter(c.TcpClient.GetStream());
                            sw.AutoFlush = true;
                            sw.WriteLine(message);
                        }
                        else
                        {
                            clients.Remove(c);
                        }
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
            });
        }

        private bool isClientConnected(TcpClient tcpClient)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation c in tcpConnections)
            {
                TcpState stateOfConnection = c.State;

                if (c.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint) && c.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint))
                {
                    if (stateOfConnection == TcpState.Established)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
