using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyChatServer
{
    class ChatRoomWorker
    {
        public ChatRoom chatRoom { get; }
        List<ConnectedClient> clients { get; }
        Regex selectChatString = new Regex(@"^ConnectChat:[a-zA-Z0-9]{1,24}$");

        public ChatRoomWorker(ChatRoom chatRoom)
        {
            this.chatRoom = chatRoom;
            clients = new List<ConnectedClient>();
        }
        public void AddNewActiveMember(ConnectedClient client, DataContext db, List<ChatRoomWorker> chatRoomWorkers)
        {
            new Thread(() =>
            {
                try
                {
                    clients.Add(client);
                    //StreamReader sr = new StreamReader(client.TcpClient.GetStream());
                    while (client.TcpClient.Connected && isClientConnected(client.TcpClient))
                    {
                        //string message = sr.ReadToEnd();

                        //Recive messagge
                        byte[] data = new byte[8192];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = client.TcpClient.GetStream().Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (client.TcpClient.GetStream().DataAvailable);

                        string message = builder.ToString();

                        //Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " in " + chatRoom.Name + ": " + message);
                        if (!string.IsNullOrEmpty(message) && selectChatString.IsMatch(message))
                        {
                            message = message.Replace("ConnectChat:", "");
                            var chat = chatRoomWorkers.Where(p => p.chatRoom.Name == message);
                            if (chat.Count() > 0)
                            {
                                if (client.currentChatRoomWorker != null)
                                    client.currentChatRoomWorker.RemoveActiveMember(client);
                                clients.Remove(client);
                                chat.First().AddNewActiveMember(client, db, chatRoomWorkers);
                                chat.First().chatRoom.MembersArray.Add(client.User);
                                db.SaveChanges();
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + message);
                            }
                            else
                            {
                                //Create chat
                                ChatRoom newChat = new ChatRoom();
                                newChat.Name = message;
                                db.ChatRooms.Add(newChat);
                                newChat.MembersArray.Add(client.User);
                                db.SaveChanges();
                                ChatRoomWorker chatRoomWorker = new ChatRoomWorker(newChat);
                                chatRoomWorkers.Add(chatRoomWorker);
                                if (client.currentChatRoomWorker != null)
                                    client.currentChatRoomWorker.RemoveActiveMember(client);
                                clients.Remove(client);
                                chatRoomWorker.AddNewActiveMember(client, db, chatRoomWorkers);
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "ChatRoom " + message + " created");
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + message);
                            }
                        }
                        else if (message == "[getServerInfo]MyChatList")
                        {
                            string result = "ChatList:";
                            foreach (ChatRoom c in client.User.ChatRooms)
                            {
                                result += c.Name + ";";
                            }
                            byte[] data1 = Encoding.Unicode.GetBytes(result);
                            client.TcpClient.GetStream().Write(data, 0, data.Length);
                        }
                        else if (message == "[getServerInfo]AllChatList")
                        {
                            string result = "ChatList:";
                            foreach (ChatRoom c in db.ChatRooms)
                            {
                                result += c.Name + ";";
                            }
                            byte[] data1 = Encoding.Unicode.GetBytes(result);
                            client.TcpClient.GetStream().Write(data, 0, data.Length);
                        }
                        else if (!string.IsNullOrWhiteSpace(message))
                        {
                            SendToAll(message, client.User.Login);
                            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " in " + chatRoom.Name + ": " + message);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }).Start();
        }
        public void RemoveActiveMember(ConnectedClient client)
        {
            clients.Remove(client);
        }
        public void SendToAll(string message, string senderNick)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (ConnectedClient c in clients)
                    {
                        if (c.TcpClient.Connected)
                        {
                            byte[] buffer = Encoding.Unicode.GetBytes(senderNick + '▶' + message);
                            c.TcpClient.GetStream().Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            clients.Remove(c);
                        }
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
            }).Start();
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
