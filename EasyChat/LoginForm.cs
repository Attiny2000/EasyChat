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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.IP))
            {
                Properties.Settings.Default.IP = "176.67.27.69";
                Properties.Settings.Default.Save();
            }
            if (!(string.IsNullOrWhiteSpace(Properties.Settings.Default.Login)) && !(string.IsNullOrWhiteSpace(Properties.Settings.Default.Password)))
            {
                bunifuMaterialTextbox1.TabStop = false;
                bunifuMaterialTextbox2.TabStop = false;
                bunifuMaterialTextbox1.Text = Properties.Settings.Default.Login;
                bunifuMaterialTextbox2.Text = Properties.Settings.Default.Password;
            }
        }
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]{3,24}$");
            //Login
            if (bunifuMaterialTextbox1.Text != "" && bunifuMaterialTextbox2.Text != "" && regex.IsMatch(bunifuMaterialTextbox1.Text) && regex.IsMatch(bunifuMaterialTextbox2.Text))
            {
                MainForm mainForm = new MainForm();

                mainForm.serverConnection = new ServerConnection(Properties.Settings.Default.IP, 5050, bunifuMaterialTextbox1.Text, bunifuMaterialTextbox2.Text, mainForm);
                if(mainForm.serverConnection.Connect(false))
                {
                    Properties.Settings.Default.Login = bunifuMaterialTextbox1.Text;
                    Properties.Settings.Default.Password = bunifuMaterialTextbox2.Text;
                    Properties.Settings.Default.Save();
                    this.Hide();
                    mainForm.Activate();
                    mainForm.Show();
                }
                else { MessageBox.Show("Wrong login or password", "You were refused by server", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
            {
                MessageBox.Show("Login and password can only include letters of latin alphabet or numbers. Login and password length must be between 3 and 24 charesters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]{3,24}$");
            //Registration
            if (bunifuMaterialTextbox1.Text != "" && bunifuMaterialTextbox2.Text != "" && regex.IsMatch(bunifuMaterialTextbox1.Text) && regex.IsMatch(bunifuMaterialTextbox2.Text))
            {
                MainForm mainForm = new MainForm();
                mainForm.serverConnection = new ServerConnection(Properties.Settings.Default.IP, 5050, bunifuMaterialTextbox1.Text, bunifuMaterialTextbox2.Text, mainForm);
                if (mainForm.serverConnection.Connect(true))
                {
                    MessageBox.Show("You have been succesfuly registred", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Properties.Settings.Default.Login = bunifuMaterialTextbox1.Text;
                    Properties.Settings.Default.Password = bunifuMaterialTextbox2.Text;
                    Properties.Settings.Default.Save();
                    this.Hide();
                    mainForm.Activate();
                    mainForm.Show();
                }
                else { MessageBox.Show("User with the same name alredy exist", "You were refused by server", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
            {
                MessageBox.Show("Login and password can only include letters of latin alphabet or numbers. Login and password length must be between 3 and 24 charesters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void bunifuMaterialTextbox2_OnValueChanged(object sender, EventArgs e)
        {
            bunifuMaterialTextbox2.isPassword = true;
        }
    }
}
