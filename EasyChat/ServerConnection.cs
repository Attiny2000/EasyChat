using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyChat
{
    public class ServerConnection
    {
        TcpClient client;
        StreamReader sr;
        StreamWriter sw;
        public string IP;
        public int Port;
        public string Nick;
        public string Password;
        private MainForm mainForm;

        public ServerConnection(string serverIp, int serverPort, string nick, string password, MainForm form)
        {
            IP = serverIp;
            Port = serverPort;
            Nick = nick;
            Password = password;
            mainForm = form;
        }

        public bool Connect(bool isRegistration)
        {
            //Connect
                return Task<bool>.Factory.StartNew(() =>
                {
                    try
                    {
                        client = new TcpClient();
                        client.Connect(IP, Port);
                        sw = new StreamWriter(client.GetStream());
                        sr = new StreamReader(client.GetStream());
                        sw.AutoFlush = true;
                        if (isRegistration) { sw.WriteLine($"[Registration]Login:{Nick};Password:{Password};"); }
                        else { sw.WriteLine($"Login:{Nick};Password:{Password};"); }

                        //mainForm.onlineStatusImage.Image = Properties.Resources.online_icon_S;
                        while (true)
                        {
                            if (client != null && client.Connected && isClientConnected())
                            {
                                try
                                {
                                    string line = sr.ReadLine();
                                    if (line != null && line == "Succes") return true;
                                    if (line != null && line == "Refused") return false;
                                }
                                catch (IOException) { return false; }
                            }
                            else
                            {
                                return false;
                            }
                            Task.Delay(100).Wait();
                        }
                    }
                    catch (Exception) { MessageBox.Show("Server does't respond", "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                }).Result;
        }


        public async void startListen()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //try
                    //{
                    if (client != null && client.Connected && isClientConnected())
                    {
                        try
                        {
                            string line = sr.ReadLine();
                            if (line != null)
                                mainForm.chatBox1.AddNewIncomingMessage(line, DateTime.Now.ToString());
                        }
                        catch (IOException ex) { }
                    }
                    Task.Delay(100).Wait();
                    //}
                    //catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            });
        }
        public void Disconnect()
        {
            //Disconnect
            //try
            //{
                //sw.WriteLine("/close connection");
                client.Close();
                sr.Close();
                sw.Close();
                //mainForm.onlineStatusImage.Image = Properties.Resources.offline_icon_S;
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public bool isClientConnected()
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation c in tcpConnections)
            {
                TcpState stateOfConnection = c.State;

                if (c.LocalEndPoint.Equals(client.Client.LocalEndPoint) && c.RemoteEndPoint.Equals(client.Client.RemoteEndPoint))
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

        public async void SendMessage(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                //try
                //{
                    if (client != null && client.Connected && !string.IsNullOrWhiteSpace(message))
                    {
                        sw.WriteLine(message);
                        mainForm.chatBox1.AddNewOutcomingMessage(message, DateTime.Now.ToString());
                    }
                //}
                //catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            });
        }
    }
}
