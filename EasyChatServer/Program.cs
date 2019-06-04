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
            try
            {
                using (DataContext db = new DataContext())
                {
                    db.ChatRooms.Load();
                    //Chats creation;
                    foreach (ChatRoom chat in db.ChatRooms)
                    {
                        ChatRoomWorker chatRoomWorker = new ChatRoomWorker(chat);
                        chatRoomWorkers.Add(chatRoomWorker);
                    }
                }
                //TCP listener
                listener.Start();
                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    ClientListener(tcpClient);
                }
            }
            catch (Exception ex) { Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message); }
        }

        static void ClientListener(TcpClient tcpClient)
        {
            new Thread(() =>
            {
                using (DataContext db = new DataContext())
                {
                    db.Users.Include(u => u.ChatRooms).Load();
                    db.ChatRooms.Include(u => u.MembersArray).Load();
                    db.Messages.Load();
                    ConnectedClient client = null;
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
                                    line.Replace("[Login]", "");
                                    string[] lp = line.Split(';');
                                    login = lp[0].Substring(lp[0].IndexOf("Login:") + 6);
                                    password = lp[1].Substring(lp[1].IndexOf("Password:") + 9);
                                    try
                                    {
                                        var query = db.Users.Where(u => u.Login == login);
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
                                        var chat = db.ChatRooms.Where(r => r.Name == line);
                                        if (chat.Count() > 0)
                                        {
                                            if (client.currentChatRoomWorker != null)
                                                client.currentChatRoomWorker.RemoveActiveMember(client);

                                            chat.First().MembersArray.Add(client.User);
                                            db.SaveChanges();
                                            var chatWorker = chatRoomWorkers.Where(p => p.ChatRoom.Name == line);
                                            chatWorker.First().AddNewActiveMember(client, chatRoomWorkers);
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
                                            chatRoomWorker.AddNewActiveMember(client, chatRoomWorkers);
                                            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "ChatRoom " + line + " created");
                                            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " connected to " + line);
                                            break;
                                        }
                                    }
                                }
                                else if (line == "[GetMyChatList]")
                                {
                                    db.Users.Include(u => u.ChatRooms).Load();
                                    string result = "ChatList:";
                                    foreach (ChatRoom c in client.User.ChatRooms)
                                    {
                                        result += c.Name + ";";
                                    }
                                    byte[] data2 = Encoding.Unicode.GetBytes(result);
                                    client.TcpClient.GetStream().Write(data2, 0, data2.Length);
                                }
                                else if (line == "[GetAllChatList]")
                                {
                                    db.ChatRooms.Include(u => u.MembersArray).Load();
                                    string result = "ChatList:";
                                    foreach (ChatRoom c in db.ChatRooms)
                                    {
                                        result += c.Name + ";";
                                    }
                                    byte[] data2 = Encoding.Unicode.GetBytes(result);
                                    client.TcpClient.GetStream().Write(data2, 0, data2.Length);
                                }
                                else if (line.Contains("[GetUserList]"))
                                {
                                    db.ChatRooms.Include(u => u.MembersArray).Load();
                                    string result = "UserList:";
                                    List<ConnectedClient> Clients = chatRoomWorkers.Find((ChatRoomWorker w) => { return w.ChatRoom.Name == line.Replace("[GetUserList]", ""); }).Clients;

                                    chatRoomWorkers.Find((ChatRoomWorker w) => { return w.ChatRoom.Name == line.Replace("[GetUserList]", ""); }).AddNewActiveMember(client, chatRoomWorkers);
                                    foreach (ConnectedClient user in Clients)
                                    {
                                        result += user.User.Login + "(online)" + ";";
                                    }
                                    foreach (ConnectedClient user in Clients)
                                    {
                                        if (!Clients.Exists((ConnectedClient p) => { return p.User.UserId == user.User.UserId; }))
                                            result += user.User.Login + "(offline)" + ";";
                                    }
                                    byte[] data0 = Encoding.Unicode.GetBytes(result);
                                    client.TcpClient.GetStream().Write(data0, 0, data0.Length);
                                }
                                else if (line == "[Disconnect]")
                                {
                                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + client.User.Login + " disconnected");
                                    clients.Remove(client);
                                    if (client.TcpClient.Connected)
                                        client.TcpClient.GetStream().Dispose();
                                    client.TcpClient.Dispose();
                                    client.TcpClient.Close();
                                }
                            }
                        }
                        catch (Exception ex) { Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message); }
                    }
                }
            }).Start();
        }
    }
}
