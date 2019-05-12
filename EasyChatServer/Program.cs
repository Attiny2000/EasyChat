using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyChatServer
{
    class Program
    {
        static List<ConnectedClient> clients = new List<ConnectedClient>();
        static List<ChatRoomWorker> chatRoomWorkers = new List<ChatRoomWorker>();
        static TcpListener listener = new TcpListener(IPAddress.Any, 5050);
        static Regex loginString = new Regex(@"Login:[a-zA-Z0-9]{1,24};Password:[a-zA-Z0-9]{1,24};");
        static Regex selectChatString = new Regex(@"^ConnectChat:[a-zA-Z0-9]{1,24}$");
        public static void Main(string[] args)
        {
            //Data base load
            DataContext db = new DataContext();
            db.Users.Include(u => u.ChatRooms).Load();
            db.ChatRooms.Include(u => u.MembersArray).Load();
            db.Messages.Load();
            db.Users.Include(u => u.ChatRooms).Load();
            db.ChatRooms.Include(u => u.MembersArray).Load();
            db.Messages.Load();
            db.Users.Include(u => u.ChatRooms).Load();
            db.ChatRooms.Include(u => u.MembersArray).Load();
            db.Messages.Load();
            //Chats creation;
            foreach(ChatRoom chat in db.ChatRooms)
            {
                ChatRoomWorker chatRoomWorker = new ChatRoomWorker(chat);
                chatRoomWorkers.Add(chatRoomWorker);
            }
            //TCP listener
            listener.Start();
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                ClientListener(tcpClient, db);
            }
        }

        static async void ClientListener(TcpClient tcpClient, DataContext db)
        {
            await Task.Factory.StartNew(() =>
            {
                ConnectedClient client = null;
                StreamReader sr = new StreamReader(tcpClient.GetStream());
                while (tcpClient.Connected)
                {
                    try
                    {
                        string line = sr.ReadLine();
                        Console.WriteLine(line);
                        string login = "";
                        string password = "";
                        if (!string.IsNullOrEmpty(line) && loginString.IsMatch(line))
                        {
                            if (line.Contains("[Registration]"))
                            {
                                //Registration
                                line.Replace("[Registration]", "");
                                string[] lp = line.Split(';');
                                login = lp[0].Substring(lp[0].IndexOf("Login:") + 6);
                                password = lp[1].Substring(lp[1].IndexOf("Password:") + 9);
                                StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                                sw.AutoFlush = true;
                                var query = db.Users.Where(p => p.Login == login);
                                if (query.Count() == 0)
                                {
                                    User user = new User(login, password, "");
                                    client = new ConnectedClient(user, tcpClient);
                                    db.Users.Add(user);
                                    db.SaveChanges();
                                    sw.WriteLine("Succes");
                                    clients.Add(client);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + login + " registred");
                                }
                                else
                                {
                                    sw.WriteLine("Refused");
                                    clients.Remove(clients.Find(x => x.TcpClient == tcpClient));
                                    tcpClient.Close();
                                }
                            }
                            else
                            {
                                //Login
                                string[] lp = line.Split(';');
                                login = lp[0].Substring(lp[0].IndexOf("Login:") + 6);
                                password = lp[1].Substring(lp[1].IndexOf("Password:") + 9);
                                StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                                sw.AutoFlush = true;
                                //Verification
                                try
                                {
                                    var query = db.Users.Where(p => p.Login == login);
                                    User user = query.First();
                                    if (login == user.Login && password == user.Password)
                                    {
                                        sw.WriteLine("Succes");
                                        client = new ConnectedClient(user, tcpClient);
                                        clients.Add(client);
                                        Console.WriteLine("[" + DateTime.Now.ToString() + "] " + login + " logined");
                                    }
                                    else
                                    {
                                        sw.WriteLine("Refused");
                                        clients.Remove(clients.Find(x => x.TcpClient == tcpClient));
                                        tcpClient.Close();
                                    }
                                }
                                catch (InvalidOperationException)
                                {
                                    sw.WriteLine("Refused");
                                    clients.Remove(clients.Find(x => x.TcpClient == tcpClient));
                                    tcpClient.Close();
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(line) && client != null)
                        {
                            //SelectChat
                            if (!string.IsNullOrEmpty(line) && selectChatString.IsMatch(line))
                            {
                                line = line.Replace("ConnectChat:", "");
                                var chat = chatRoomWorkers.Where(p => p.chatRoom.Name == line);
                                if (chat.Count() > 0)
                                {
                                    if (client.currentChatRoomWorker != null)
                                        client.currentChatRoomWorker.RemoveActiveMember(client);
                                    chat.First().AddNewActiveMember(client);
                                    chat.First().chatRoom.MembersArray.Add(client.User);
                                    db.SaveChanges();
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + line);
                                }
                                else
                                {
                                    //Create chat
                                    ChatRoom newChat = new ChatRoom();
                                    newChat.Name = line;
                                    db.ChatRooms.Add(newChat);
                                    newChat.MembersArray.Add(client.User);
                                    db.SaveChanges();
                                    ChatRoomWorker chatRoomWorker = new ChatRoomWorker(newChat);
                                    chatRoomWorkers.Add(chatRoomWorker);
                                    if (client.currentChatRoomWorker != null)
                                        client.currentChatRoomWorker.RemoveActiveMember(client);
                                    chatRoomWorker.AddNewActiveMember(client);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "ChatRoom " + line + " created");
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + line);
                                }
                            }
                            else if (line == "[getServerInfo]MyChatList")
                            {
                                string result = "ChatList:";
                                foreach (ChatRoom c in client.User.ChatRooms)
                                {
                                    result += c.Name + ";";
                                }
                                StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                                sw.AutoFlush = true;
                                sw.WriteLine(result);
                            }
                            else
                            {
                                if (line == "[getServerInfo]AllChatList")
                                {
                                    string result = "ChatList:";
                                    foreach (ChatRoom c in db.ChatRooms)
                                    {
                                        result += c.Name + ";";
                                    }
                                    StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                                    sw.AutoFlush = true;
                                    sw.WriteLine(result);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message); }
                }
            });
        }
    }
}
