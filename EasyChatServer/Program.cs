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
            List<ConnectedClient> clients = new List<ConnectedClient>();
            //TCP listener
            TcpListener listener = new TcpListener(IPAddress.Any, 5050);
            Regex regex = new Regex(@"Login:\S{1,24};Password:\S{1,24};");
            TcpClient client;
            listener.Start();
            while (true)
            {
                client = listener.AcceptTcpClient();
                StreamReader sr = new StreamReader(client.GetStream());
                while (client.Connected && isClientConnected(client))
                {
                    try
                    {
                        string line = sr.ReadLine();
                        string login = "";
                        string password = "";
                        if (!string.IsNullOrEmpty(line) && regex.IsMatch(line))
                        {
                            if (line.Contains("[Registration]"))
                            {
                                //Registration
                                line.Replace("[Registration]", "");
                                string[] lp = line.Split(';');
                                login = lp[0].Substring(lp[0].IndexOf("Login:") + 6);
                                password = lp[1].Substring(lp[1].IndexOf("Password:") + 9);
                                StreamWriter sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;
                                //Verification
                                var query = db.Users.Where(p => p.Login == login);
                                if (query.Count() == 0)
                                {
                                    User user = new User(login, password, "");
                                    db.Users.Add(user);
                                    db.SaveChanges();
                                    sw.WriteLine("Succes");
                                    clients.Add(new ConnectedClient(user, client));
                                    Console.WriteLine(login + " registred");
                                }
                                else
                                {
                                    sw.WriteLine("Refused");
                                    client.Close();
                                    clients.Remove(clients.Find(x => x.Client == client));
                                }
                            }
                            else
                            {
                                //Login
                                string[] lp = line.Split(';');
                                login = lp[0].Substring(lp[0].IndexOf("Login:") + 6);
                                password = lp[1].Substring(lp[1].IndexOf("Password:") + 9);
                                StreamWriter sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;
                                //Verification
                                try
                                {
                                    var query = db.Users.Where(p => p.Login == login);
                                    User user = query.First();
                                    if (login == user.Login && password == user.Password)
                                    {
                                        sw.WriteLine("Succes");
                                        clients.Add(new ConnectedClient(user, client));
                                        Console.WriteLine(login + " logined");
                                    }
                                    else
                                    {
                                        sw.WriteLine("Refused");
                                        client.Close();
                                        clients.Remove(clients.Find(x => x.Client == client));
                                    }
                                }
                                catch (InvalidOperationException)
                                {
                                    sw.WriteLine("Refused");
                                    client.Close();
                                    clients.Remove(clients.Find(x => x.Client == client));
                                }
                            }
                            break;
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }
                //ChatWorker connection = new ChatWorker();
                //connection.StartConnection();
            }
        }

        static bool isClientConnected(TcpClient tcpClient)
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
