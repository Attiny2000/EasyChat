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
using Bunifu.Framework.UI;

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
            try
            {
                serverConnection.SendMessage("[Disconnect]");
                serverConnection.Disconnect();
            }
            catch (Exception) { }
            finally { Application.Exit(); }
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            //Chat menu
            contextMenuStrip1.Show(PointToScreen(new Point(bunifuImageButton4.Location.X + 0, bunifuImageButton4.Location.Y + 33)), ToolStripDropDownDirection.BelowLeft);
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            //Add chat
            chatBox1.Clear();
            List<string> list = serverConnection.ReciveChatListFromServer();
            if (list.Exists(s => s == bunifuMaterialTextbox1.Text))
            {
                chatList1.AddNewButton(bunifuMaterialTextbox1.Text);
            }
            else
            {
                DialogResult res = MessageBox.Show("Chat room with this name does not exist. Do you want to create new chat room?", "Create new chat room?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    chatList1.AddNewButton(bunifuMaterialTextbox1.Text);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void leaveChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Delete chat
            serverConnection.LeaveChat();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Users List
            ActiveUsersForm usersForm = new ActiveUsersForm();
            usersForm.mainForm = this;
            usersForm.StartPosition = FormStartPosition.Manual;
            usersForm.Location = new Point(620, 260);
            usersForm.Show();
        }

        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chatBox1.Clear();
        }

        private void bunifuMaterialTextbox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chatBox1.Clear();
                List<string> list = serverConnection.ReciveChatListFromServer();
                if (list.Exists(s => s == bunifuMaterialTextbox1.Text))
                {
                    chatList1.AddNewButton(bunifuMaterialTextbox1.Text);
                }
                else
                {
                    DialogResult res = MessageBox.Show("Chat room with this name does not exist. Do you want to create new chat room?", "Create new chat room?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        chatList1.AddNewButton(bunifuMaterialTextbox1.Text);
                    }
                }
            }

        }
    }
}
