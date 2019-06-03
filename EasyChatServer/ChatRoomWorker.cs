﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public ChatRoom ChatRoom { get; }
        public List<ConnectedClient> Clients { get; }
        Regex selectChatString = new Regex(@"^ConnectChat:[a-zA-Z0-9]{1,24}$");

        public ChatRoomWorker(ChatRoom chatRoom)
        {
            this.ChatRoom = chatRoom;
            Clients = new List<ConnectedClient>();
        }
        public void AddNewActiveMember(ConnectedClient client, List<ChatRoomWorker> chatRoomWorkers)
        {
            new Thread(() =>
            {
                try
                {
                    using (DataContext db = new DataContext())
                    {
                        db.Users.Include(u => u.ChatRooms).Load();
                        db.ChatRooms.Include(u => u.MembersArray).Load();
                        db.Messages.Load();
                        Clients.Add(client);
                        while (client.TcpClient.Connected && isClientConnected(client.TcpClient))
                        {
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
                                var chat = chatRoomWorkers.Where(p => p.ChatRoom.Name == message);
                                if (chat.Count() == 0)
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
                                    Clients.Remove(client);
                                    chatRoomWorker.AddNewActiveMember(client, chatRoomWorkers);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "ChatRoom " + message + " created");
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + message);
                                    Thread.CurrentThread.Abort();
                                }
                                else
                                {
                                    if (client.currentChatRoomWorker != null)
                                        client.currentChatRoomWorker.RemoveActiveMember(client);
                                    Clients.Remove(client);
                                    chat.First().ChatRoom.MembersArray.Add(client.User);
                                    chat.First().AddNewActiveMember(client, chatRoomWorkers);
                                    db.SaveChanges();
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + message);
                                    Thread.CurrentThread.Abort();
                                }
                            }
                            else if (message == "[GetMyChatList]")
                            {
                                db.Users.Include(u => u.ChatRooms).Load();
                                string result = "ChatList:";
                                foreach (ChatRoom c in client.User.ChatRooms)
                                {
                                    result += c.Name + ";";
                                }
                                byte[] data1 = Encoding.Unicode.GetBytes(result);
                                client.TcpClient.GetStream().Write(data, 0, data.Length);
                            }
                            else if (message == "[GetAllChatList]")
                            {
                                db.ChatRooms.Include(u => u.MembersArray).Load();
                                string result = "ChatList:";
                                foreach (ChatRoom c in db.ChatRooms)
                                {
                                    result += c.Name + ";";
                                }
                                byte[] data1 = Encoding.Unicode.GetBytes(result);
                                client.TcpClient.GetStream().Write(data, 0, data.Length);
                            }
                            else if (message == "[LeaveChat]")
                            {
                                var query = db.Users.Where(u => u.Login == client.User.Login);
                                User user = query.First();
                                user.ChatRooms.Remove(db.ChatRooms.Find(ChatRoom.ChatRoomId));
                                db.SaveChanges();
                                RemoveActiveMember(client);
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " left " + ChatRoom.Name);
                            }
                            else if (!string.IsNullOrWhiteSpace(message))
                            {
                                if (message == "[Disconnect]")
                                {
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " disconnected");
                                    Clients.Remove(client);
                                    if (client.TcpClient.Connected)
                                        client.TcpClient.GetStream().Dispose();
                                    client.TcpClient.Dispose();
                                    client.TcpClient.Close();
                                    Thread.CurrentThread.Abort();
                                }
                                else
                                {
                                    SendToAll(message, client.User.Login);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " in " + ChatRoom.Name + ": " + message);
                                }
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    //Disconnected from server
                    //Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " disconnected");
                    try
                    {
                        Clients.Remove(client);
                        if (client.TcpClient.Connected)
                            client.TcpClient.GetStream().Dispose();
                        client.TcpClient.Dispose();
                        client.TcpClient.Close();
                        Thread.CurrentThread.Abort();
                    }
                    catch { }
                }
                catch(ThreadAbortException){ Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " disconnected from " + ChatRoom.Name); }
                catch (Exception ex) { Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message); }
            }).Start();
        }
        public void RemoveActiveMember(ConnectedClient client)
        {
            Clients.Remove(client);
        }
        public void SendToAll(string message, string senderNick)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (ConnectedClient c in Clients)
                    {
                        if (c.TcpClient.Connected)
                        {
                            byte[] buffer = Encoding.Unicode.GetBytes(senderNick + '▶' + message);
                            c.TcpClient.GetStream().Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            Clients.Remove(c);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
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
