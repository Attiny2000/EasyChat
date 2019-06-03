using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyChat
{
    public partial class ActiveUsersForm : Form
    {
        public MainForm mainForm;
        public ActiveUsersForm()
        {
            InitializeComponent();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActiveUsersForm_Load(object sender, EventArgs e)
        {
            if (mainForm.chatList1.ActiveButton != null)
            {
                listBox1.DataSource = mainForm.serverConnection.GetUsersList();
            }
            else
                MessageBox.Show("You must first select a chat", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
