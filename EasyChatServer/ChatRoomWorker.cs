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
        Regex selectChatString = new Regex(@"^\[ConnectChat\]:[a-zA-Zа-яА-Я0-9 ]{1,24}$");

        public ChatRoomWorker(ChatRoom chatRoom)
        {
            this.ChatRoom = chatRoom;
            Clients = new List<ConnectedClient>();
        }
        public void AddNewActiveMember(ConnectedClient client, DataContext db, List<ChatRoomWorker> chatRoomWorkers)
        {
            if(client.currentChatRoomWorker != null)
                client.currentChatRoomWorker.RemoveActiveMember(client);

            new Thread(() =>
            {
                try
                {
                    lock (db)
                    {
                        db.Users.Include(u => u.ChatRooms).Load();
                        db.ChatRooms.Include(u => u.MembersArray).Load();
                        db.Messages.Load();
                    }
                    if (!Clients.Exists((ConnectedClient c) => { return c.User.Login == client.User.Login; }))
                    {
                        Clients.Add(client);
                    }
                    else
                    {
                        Clients.Find((ConnectedClient c) => { return c.User.Login == client.User.Login; }).TcpClient = client.TcpClient;
                        Clients.Find((ConnectedClient c) => { return c.User.Login == client.User.Login; }).currentChatRoomWorker = client.currentChatRoomWorker;
                    }
                    //Send message history
                    //foreach (Message m in db.Messages)
                    //{
                    //    if (m.ChatRoom.ChatRoomId == ChatRoom.ChatRoomId)
                    //    {
                    //        byte[] buffer = Encoding.Unicode.GetBytes(m.Sender.Login + '▶' + m.MessageTime + '▶' + m.MessageText);
                    //        client.TcpClient.GetStream().Write(buffer, 0, buffer.Length);
                    //        Thread.Sleep(100);
                    //    }
                    //}
                    while (client.TcpClient.Connected)
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
                            message = message.Replace("[ConnectChat]:", "");
                            lock (db)
                            {
                                var chat = db.ChatRooms.Where(ch => ch.Name == message);
                                if (chat.Count() > 0)
                                {
                                    //Connect to chat
                                    if (client.currentChatRoomWorker != null)
                                        client.currentChatRoomWorker.RemoveActiveMember(client);

                                    if (!chat.First().MembersArray.Contains(db.Users.Where(u => u.UserId == client.User.UserId).First()))
                                    {
                                        try
                                        {
                                            chat.First().MembersArray.Add(client.User);
                                            lock (db) { db.SaveChanges(); }
                                        }
                                        catch (Exception) { }
                                    }
                                    chatRoomWorkers.Find(p => p.ChatRoom.Name == message).AddNewActiveMember(client, db, chatRoomWorkers);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + message);
                                    Thread.CurrentThread.Abort();
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
                                    Clients.Remove(client);
                                    chatRoomWorker.AddNewActiveMember(client, db, chatRoomWorkers);
                                }
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "ChatRoom " + message + " created");
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + message);
                                Thread.CurrentThread.Abort();
                            }
                        }
                        else if (message == "[GetMyChatList]")
                        {
                            lock (db)
                            {
                                db.ChatRooms.Include(u => u.MembersArray).Load();
                                db.Users.Include(u => u.ChatRooms).Load(); db.Users.Include(u => u.ChatRooms).Load();
                            }
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
                            string result = "ChatList:";
                            lock (db)
                            {
                                db.ChatRooms.Include(u => u.MembersArray).Load();
                                db.Users.Include(u => u.ChatRooms).Load();
                                foreach (ChatRoom c in db.ChatRooms)
                                {
                                    result += c.Name + ";";
                                }
                            }
                            byte[] data1 = Encoding.Unicode.GetBytes(result);
                            client.TcpClient.GetStream().Write(data, 0, data.Length);
                        }
                        else if (message.Contains("[GetUserList]"))
                        {
                            lock (db)
                            {
                                db.ChatRooms.Include(u => u.MembersArray).Load();
                            }
                            string result = "UserList:";

                            foreach (ConnectedClient user in Clients)
                            {
                                result += user.User.Login + "(online)" + ";";
                            }
                            foreach (User user in ChatRoom.MembersArray)
                            {
                                if (!Clients.Exists((ConnectedClient p) => { return p.User.UserId == user.UserId; }))
                                    result += user.Login + "(offline)" + ";";
                            }
                            byte[] data0 = Encoding.Unicode.GetBytes(result);
                            client.TcpClient.GetStream().Write(data0, 0, data0.Length);
                        }
                        else if (message.Contains("[LeaveChat]"))
                        {
                            message = message.Replace("[LeaveChat]", "");
                            lock (db)
                            {
                                var query = db.Users.Where(u => u.Login == client.User.Login);
                                User user = query.First();
                                user.ChatRooms.Remove(db.ChatRooms.Where(r => r.Name == message).First());
                            }
                            lock (db) { db.SaveChanges(); }
                            chatRoomWorkers.Where(p => p.ChatRoom.Name == message).First().RemoveActiveMember(client);
                            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " left " + db.ChatRooms.Where(r => r.Name == message).First().Name);
                            lock (db)
                            {
                                db.Users.Include(u => u.ChatRooms).Load();
                                db.ChatRooms.Include(u => u.MembersArray).Load();
                            }
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
                                SendToAll(message, client, db);
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " in " + ChatRoom.Name + ": " + message);
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
                catch (ThreadAbortException) { Clients.Remove(client); }
                catch (Exception ex) { Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message); Clients.Remove(client); }
            }).Start();
        }
        public void RemoveActiveMember(ConnectedClient client)
        {
            Clients.Remove(client);
            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " disconnected from " + ChatRoom.Name);
        }
        public void SendToAll(string messageText, ConnectedClient sender, DataContext db)
        {
            new Thread(() =>
            {
                try
                {
                    foreach (ConnectedClient c in Clients)
                    {
                        if (c.TcpClient.Connected)
                        {
                            byte[] buffer = Encoding.Unicode.GetBytes(sender.User.Login + '▶' + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + '▶' + messageText + '▶' + sender.User.Photo);
                            c.TcpClient.GetStream().Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            Clients.Remove(c);
                        }
                    }
                    //Write message to db
                    lock (db)
                    {
                        Message message = new Message();
                        message.MessageText = messageText;
                        message.Sender = db.Users.Where(p => p.Login == sender.User.Login).First();
                        message.ChatRoom = db.ChatRooms.Where(ch => ch.ChatRoomId == ChatRoom.ChatRoomId).First(); ;
                        message.MessageTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        db.Messages.Add(message);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }).Start();
        }
    }
}