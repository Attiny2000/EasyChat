using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyChatServer
{
    //old project code
    //class ChatWorker
    //{
    //    List<ConnectedClient> clients = new List<ConnectedClient>();
    //    TcpListener listener = new TcpListener(IPAddress.Any, 5050);
    //    TcpClient client;
    //    public void StartConnection()
    //    {
    //        listener.Start();

    //        while (true)
    //        {
    //            client = listener.AcceptTcpClient();
    //            StreamReader sr = new StreamReader(client.GetStream());
    //            Task.Factory.StartNew(() =>
    //            {
    //                while (client.Connected)
    //                {
    //                    try
    //                    {
    //                        string line = sr.ReadLine();
    //                        if (line.Contains("Login: ") && !string.IsNullOrWhiteSpace(line.Replace("Login: ", "")))
    //                        {
    //                            string nick = line.Replace("Login: ", "");
    //                            if (!clients.Exists(x => x.Nick == nick))
    //                            {
    //                                clients.Add(new ConnectedClient(nick, client));
    //                                StreamWriter sw = new StreamWriter(client.GetStream());
    //                                sw.AutoFlush = true;
    //                                SendToAll($"Server: {nick} connected");
    //                                Debug.WriteLine($"{nick} connected");
    //                                break;
    //                            }
    //                            else
    //                            {
    //                                StreamWriter sw = new StreamWriter(client.GetStream());
    //                                sw.AutoFlush = true;

    //                                sw.WriteLine("Server: User with this nickname already exists");
    //                                client.Client.Disconnect(false);
    //                            }
    //                        }
    //                    }
    //                    catch (Exception ex) { Debug.WriteLine(ex.Message); }
    //                }

    //                while (client.Connected)
    //                {

    //                    string nick = clients.Find(x => x.Client == client).Nick;
    //                    try
    //                    {
    //                        string line = sr.ReadLine();
    //                        if (!string.IsNullOrWhiteSpace(line))
    //                        {
    //                            if (line == "/close connection")
    //                            {
    //                                SendToAll($"Server: {clients.Find(x => x.Client == client).Nick} disconnected");
    //                                Debug.WriteLine($"{clients.Find(x => x.Client == client).Nick} disconnected");
    //                                clients.Remove(clients.Find(x => x.Client == client));
    //                                break;
    //                            }
    //                            else
    //                            {
    //                                SendToAll($"{nick}: {line}");
    //                                Debug.WriteLine($"{nick}: {line}");
    //                            }
    //                        }
    //                    }
    //                    catch (IOException)
    //                    {
    //                        clients.Remove(clients.Find(x => x.Nick == nick));
    //                        SendToAll($"Server: {nick} disconnected");
    //                        Debug.WriteLine($"{nick} disconnected");
    //                    }
    //                    catch (Exception ex) { Debug.WriteLine(ex.Message); }
    //                }
    //            });
    //        }
    //    }

    //    public async void SendToAll(string message)
    //    {
    //        await Task.Factory.StartNew(() =>
    //        {
    //            try
    //            {
    //                foreach (ConnectedClient c in clients)
    //                {
    //                    if (c.Client.Connected)
    //                    {
    //                        StreamWriter sw = new StreamWriter(c.Client.GetStream());
    //                        sw.AutoFlush = true;
    //                        sw.WriteLine(message);
    //                    }
    //                    else
    //                    {
    //                        clients.Remove(c);
    //                    }
    //                }
    //            }
    //            catch (Exception ex) { Debug.WriteLine(ex.Message); }
    //        });
    //    }
    }
}
