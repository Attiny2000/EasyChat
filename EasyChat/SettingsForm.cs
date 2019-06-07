using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyChat
{
    public partial class SettingsForm : Form
    {
        public LoginForm LoginForm;
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            try { LoginForm.WindowState = FormWindowState.Normal; } catch { }
            this.Close();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try { LoginForm.WindowState = FormWindowState.Normal; } catch { }
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.IP = bunifuMaterialTextbox1.Text;

            Properties.Settings.Default.Photo = bunifuMaterialTextbox2.Text;

            Properties.Settings.Default.Save();

            try { LoginForm.WindowState = FormWindowState.Normal; } catch { }
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            bunifuMaterialTextbox1.Text = Properties.Settings.Default.IP;
            bunifuMaterialTextbox2.Text = Properties.Settings.Default.Photo;
        }
    }
}
