using System;
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
        public ServerConnection serverConnection = null;
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
            List<string> list = serverConnection.ReciveChatListFromServer();
            if(list.Exists(s => s == bunifuMaterialTextbox1.Text))
            {
                chatList1.AddNewButton(bunifuMaterialTextbox1.Text);
            }
            else
            {
                DialogResult res = MessageBox.Show("Chat room with this name does not exist. Do you want to create new chat room?", "Create new chat room?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(res == DialogResult.Yes)
                {
                    chatList1.AddNewButton(bunifuMaterialTextbox1.Text);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
