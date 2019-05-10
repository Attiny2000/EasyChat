﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EasyChat
{
    public partial class MainForm : Form
    {
        public ServerConnection chatConnection = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            //chatConnection.Disconnect();
            Application.Exit();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            //Chat settings
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            //Add chat
            chatList1.AddNewButton("ChatRoom");
        }
    }
}
