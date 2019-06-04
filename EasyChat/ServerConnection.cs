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
using System.Windows.Forms;

namespace EasyChat
{
    public class ServerConnection
    {
        TcpClient client;
        public string IP;
        public int Port;
        public string Nick;
        public string Password;
        private bool isListening = false;
        private Task listeningTask;
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
                        if (isRegistration)
                        {
                            byte[] data = Encoding.Unicode.GetBytes($"[Registration]Login:{Nick};Password:{Password};");
                            client.GetStream().Write(data, 0, data.Length);
                        }
                        else
                        {
                            byte[] data = Encoding.Unicode.GetBytes($"[Login]Login:{Nick};Password:{Password};");
                            client.GetStream().Write(data, 0, data.Length);
                        }

                        while (true)
                        {
                            if (client != null && client.Connected)
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
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        public void startListen()
        {
            if (listeningTask != null)
                listeningTask.Dispose();

            isListening = true;
            Task.Factory.StartNew(() =>
            {
                while (isListening)
                {
                    try
                    {
                        if (client != null && client.Connected)
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
                                    string[] s = message.Split('▶');
                                    if (s[0] == Nick)
                                    {
                                        mainForm.chatBox1.AddNewOutcomingMessage(s[2], s[1]);
                                    }
                                    else
                                    {
                                        mainForm.chatBox1.AddNewIncomingMessage(s[2], s[1], s[0]);
                                    }
                                }
                            }
                        }
                        Thread.Sleep(50);
                    }
                    catch (Exception ex) { Debug.WriteLine(ex.Message); }
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
                    if (client != null && client.Connected)
                    {
                        mainForm.serverConnection.SendMessage("[GetMyChatList]");

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
                    {
                        return list;
                    }
                }
                catch (Exception) { return new List<string>(); }
            }
            return list;
        }
        public void LeaveChat()
        {
            if (client.Connected)
            {
                try
                {
                    if (mainForm.chatList1.ActiveButton != null)
                    {
                        mainForm.serverConnection.SendMessage("[LeaveChat]");
                        mainForm.chatBox1.Clear();
                        mainForm.chatList1.Clear();
                        if (isListening)
                        {
                            Disconnect();
                            Connect(false);
                        }
                        foreach (string s in mainForm.serverConnection.ReciveMyChatListFromServer())
                        {
                            mainForm.chatList1.AddNewButton(s);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You must first select a chat", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        public List<string> GetUsersList()
        {
            List<string> list = new List<string>();
            while (client.Connected)
            {
                try
                {
                    if (isListening)
                    {
                        Disconnect();
                        Connect(false);
                    }
                    mainForm.serverConnection.SendMessage("[GetUserList]" + mainForm.chatList1.ActiveButton.Text.Remove(0, 2));
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
                    if (line != null && line.Contains("UserList:"))
                    {
                        line = line.Replace("UserList:", "");
                        list = line.Split(';').ToList();
                        return list;
                    }
                    return list;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return list; }
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
                    if (client != null && client.Connected)
                    {
                        if (isListening)
                        {
                            mainForm.chatList1.ActiveButton.selected = false;
                            mainForm.chatList1.ActiveButton = null;
                            Disconnect();
                            Connect(false);
                        }

                        mainForm.serverConnection.SendMessage("[GetAllChatList]");
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
                    {
                        return list;
                    }
                }
                catch (Exception) { return new List<string>(); }
            }
            return list;
        }
        public void Disconnect()
        {
            try
            {
                isListening = false;
                listeningTask.Dispose();
                mainForm.serverConnection.SendMessage("[Disconnect]");
                if (client.Connected)
                    client.GetStream().Dispose();
                client.Dispose();
                client.Close();
                mainForm.onlineStatusImage.Image = Properties.Resources.offline_icon_S;
            }
            catch (Exception) {}
        }
    }
}
