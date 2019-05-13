using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyChat
{
    public class ServerConnection
    {
        TcpClient client;
        //StreamReader sr;
        //StreamWriter sw;
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
                        //sw = new StreamWriter(client.GetStream());
                        //sr = new StreamReader(client.GetStream());
                        //sw.AutoFlush = true;
                        if (isRegistration)
                        {
                            byte[] data = Encoding.Unicode.GetBytes($"[Registration]Login:{Nick};Password:{Password};");
                            client.GetStream().Write(data, 0, data.Length);
                        }
                        else
                        {
                            byte[] data = Encoding.Unicode.GetBytes($"Login:{Nick};Password:{Password};");
                            client.GetStream().Write(data, 0, data.Length);
                        }

                        //mainForm.onlineStatusImage.Image = Properties.Resources.online_icon_S;
                        while (true)
                        {
                            if (client != null && client.Connected && isClientConnected())
                            {
                                try
                                {
                                    byte[] data1 = new byte[8192];
                                    StringBuilder builder = new StringBuilder();
                                    int bytes = 0;
                                    do
                                    {
                                        bytes = client.GetStream().Read(data1, 0, data1.Length);
                                        builder.Append(Encoding.Unicode.GetString(data1, 0, bytes));
                                    }
                                    while (client.GetStream().DataAvailable);

                                    string line = builder.ToString();
                                    if (line != null && line == "Succes") return true;
                                    if (line != null && line == "Refused") return false;
                                }
                                catch (Exception) { return false; }
                            }
                            else
                            {
                                return false;
                            }
                            //Task.Delay(100).Wait();
                        }
                    }
                    catch (Exception) { MessageBox.Show("Server does't respond", "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                }).Result;
        }
        public void SendMessage(string message)
        {
            try
            {
                if (client != null && client.Connected && !string.IsNullOrWhiteSpace(message))
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    client.GetStream().Write(data, 0, data.Length);
                    //sw.Write(message);
                    //Debug.WriteLine(message);
                    //mainForm.chatBox1.AddNewOutcomingMessage(message, DateTime.Now.ToString());
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        public void SendLine(string line)
        {
            try
            {
                if (client != null && client.Connected && !string.IsNullOrWhiteSpace(line))
                {
                    byte[] data = Encoding.Unicode.GetBytes(line);
                    client.GetStream().Write(data, 0, data.Length);
                    //mainForm.chatBox1.AddNewOutcomingMessage(line, DateTime.Now.ToString());
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        public async void startListen()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (client != null && client.Connected && isClientConnected())
                        {
                            byte[] data = new byte[8192];
                            StringBuilder builder = new StringBuilder();
                            int bytes = 0;
                            do
                            {
                                bytes = client.GetStream().Read(data, 0, data.Length);
                                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            }
                            while (client.GetStream().DataAvailable);

                            string message = builder.ToString();

                            if (message != null)
                            {
                                if (message.Contains('▶'))
                                {
                                    //TODO
                                    string[] s = message.Split('▶');
                                    if (s[0] == Nick)
                                    {
                                        mainForm.chatBox1.AddNewOutcomingMessage(s[1], DateTime.Now.ToString());
                                    }
                                    else
                                    {
                                        mainForm.chatBox1.AddNewIncomingMessage(s[1], DateTime.Now.ToString(), s[0]);
                                    }
                                }
                            }
                        }
                        Task.Delay(100).Wait();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            });
        }

        public List<string> ReciveMyChatListFromServer()
        {
            //Regex regex = new Regex(@"^ChatList:([a-zA-Z0-9]{1,24};)+");
            List<string> list = new List<string>();
            while (client.Connected)
            {
                try
                {
                    if (client != null && client.Connected && isClientConnected())
                    {
                        mainForm.serverConnection.SendLine("[ServerInfo]MyChatList");
                        byte[] data = new byte[8192];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = client.GetStream().Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (client.GetStream().DataAvailable);

                        string line = builder.ToString();
                        if (line != null && line.Contains("ChatList:"))
                        {
                            line = line.Replace("ChatList:", "");
                            list = line.Split(';').ToList();
                            return list;
                        }
                    }
                    else
                        return list;
                }
                catch (Exception) { return new List<string>(); }
            }
            return list;
        }
        public List<string> ReciveChatListFromServer()
        {
            //Regex regex = new Regex(@"^ChatList:([a-zA-Z0-9]{1,24};)+");
            List<string> list = new List<string>();
            while (client.Connected)
            {
                try
                {
                    if (client != null && client.Connected && isClientConnected())
                    {
                        mainForm.serverConnection.SendLine("[ServerInfo]AllChatList");
                        byte[] data = new byte[8192];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = client.GetStream().Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (client.GetStream().DataAvailable);

                        string line = builder.ToString();
                        if (line != null && line.Contains("ChatList:"))
                        {
                            line = line.Replace("ChatList:", "");
                            list = line.Split(';').ToList();
                            return list;
                        }
                    }
                    else
                        return list;
                }
                catch (Exception) { return new List<string>(); }
            }
            return list;
        }
        public void Disconnect()
        {
            //Disconnect
            try
            {
                client.Close();
                //sr.Close();
                mainForm.onlineStatusImage.Image = Properties.Resources.offline_icon_S;
            }
            catch (Exception) {}
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
    }
}
