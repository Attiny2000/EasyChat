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
using System.Text;
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

        static void ClientListener(TcpClient tcpClient, DataContext db)
        {
            new Thread(() =>
            {
                ConnectedClient client = null;
                //StreamReader sr = new StreamReader(tcpClient.GetStream());
                while (tcpClient.Connected)
                {
                    try
                    {
                        byte[] data = new byte[8192];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = tcpClient.GetStream().Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (tcpClient.GetStream().DataAvailable);

                        string line = builder.ToString();
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
                                //StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                                //sw.AutoFlush = true;
                                var query = db.Users.Where(p => p.Login == login);
                                if (query.Count() == 0)
                                {
                                    User user = new User(login, password, "");
                                    client = new ConnectedClient(user, tcpClient);
                                    db.Users.Add(user);
                                    db.SaveChanges();
                                    byte[] data1 = Encoding.Unicode.GetBytes("Succes");
                                    tcpClient.GetStream().Write(data1, 0, data1.Length);
                                    clients.Add(client);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + login + " registred");
                                }
                                else
                                {
                                    byte[] data1 = Encoding.Unicode.GetBytes("Refused");
                                    tcpClient.GetStream().Write(data1, 0, data1.Length);
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
                                //StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                                //sw.AutoFlush = true;
                                //Verification
                                try
                                {
                                    var query = db.Users.Where(p => p.Login == login);
                                    User user = query.First();
                                    if (login == user.Login && password == user.Password)
                                    {
                                        byte[] data1 = Encoding.Unicode.GetBytes("Succes");
                                        tcpClient.GetStream().Write(data1, 0, data1.Length);   
                                        client = new ConnectedClient(user, tcpClient);
                                        clients.Add(client);
                                        Console.WriteLine("[" + DateTime.Now.ToString() + "] " + login + " logined");
                                    }
                                    else
                                    {
                                        byte[] data1 = Encoding.Unicode.GetBytes("Refused");
                                        tcpClient.GetStream().Write(data1, 0, data1.Length);
                                        clients.Remove(clients.Find(x => x.TcpClient == tcpClient));
                                        tcpClient.Close();
                                    }
                                }
                                catch (InvalidOperationException)
                                {
                                    byte[] data1 = Encoding.Unicode.GetBytes("Refused");
                                    tcpClient.GetStream().Write(data1, 0, data1.Length);
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
                            if (client.currentChatRoomWorker == null)
                            {
                                //Connect chat
                                line = line.Replace("ConnectChat:", "");
                                var chat = chatRoomWorkers.Where(p => p.chatRoom.Name == line);
                                if (chat.Count() > 0)
                                {
                                    if (client.currentChatRoomWorker != null)
                                        client.currentChatRoomWorker.RemoveActiveMember(client);
                                    chat.First().AddNewActiveMember(client, db, chatRoomWorkers);
                                    chat.First().chatRoom.MembersArray.Add(client.User);
                                    db.SaveChanges();
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + line);
                                    break;
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
                                    chatRoomWorker.AddNewActiveMember(client, db, chatRoomWorkers);
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "ChatRoom " + line + " created");
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + line);
                                    break;
                                }
                            }
                        }
                        else if (line == "[ServerInfo]MyChatList")
                        {
                            string result = "ChatList:";
                            foreach (ChatRoom c in client.User.ChatRooms)
                            {
                                result += c.Name + ";";
                            }
                            byte[] data2 = Encoding.Unicode.GetBytes(result);
                            client.TcpClient.GetStream().Write(data2, 0, data2.Length);
                        }
                        else
                        {
                            if (line == "[ServerInfo]AllChatList")
                            {
                                string result = "ChatList:";
                                foreach (ChatRoom c in db.ChatRooms)
                                {
                                    result += c.Name + ";";
                                }
                                byte[] data2 = Encoding.Unicode.GetBytes(result);
                                client.TcpClient.GetStream().Write(data2, 0, data2.Length);
                            }
                        }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message); }
                }
            }).Start();
        }
    }
}
