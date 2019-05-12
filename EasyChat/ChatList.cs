using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using System.Threading;

namespace EasyChat
{
    public partial class ChatList : UserControl
    {
        int buttonCount = 0;
        int lastButtonBottom = 0;
        MainForm mainForm;
        BunifuFlatButton activeButton = null;

        public ChatList()
        {
            InitializeComponent();
        }
        private void ChatList_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Parent.Parent;
            ListPanel.AutoScroll = false;
            ListPanel.VerticalScroll.Enabled = false;
            ListPanel.VerticalScroll.Visible = false;
            ListPanel.HorizontalScroll.Enabled = false;
            ListPanel.HorizontalScroll.Visible = false;
            ListPanel.HorizontalScroll.Maximum = 0;
            ListPanel.VerticalScroll.Maximum = 0;
            ListPanel.AutoScroll = true;

            List<string> list = mainForm.serverConnection.ReciveChatListFromServer();
            foreach (string s in list)
            {
                AddNewButton(s);
            }
        }
       public void AddNewButton(string buttonText)
        {
            if (!string.IsNullOrWhiteSpace(buttonText))
            {
                ScrollToBottom();
                BunifuFlatButton button = new BunifuFlatButton();
                button.Font = ExampleButton.Font;
                button.Size = ExampleButton.Size;
                button.Location = new Point(0, lastButtonBottom - 1); ;
                button.IsTab = ExampleButton.IsTab;
                button.selected = false;
                button.Text = "  " + buttonText;
                button.Activecolor = ExampleButton.Activecolor;
                button.ForeColor = ExampleButton.ForeColor;
                button.Textcolor = ExampleButton.Textcolor;
                button.TextAlign = ExampleButton.TextAlign;
                button.Normalcolor = ExampleButton.Normalcolor;
                button.BackColor = ExampleButton.BackColor;
                button.colbackground = ExampleButton.colbackground;
                button.OnHovercolor = ExampleButton.OnHovercolor;
                button.Anchor = ExampleButton.Anchor;
                button.Iconimage = ExampleButton.Iconimage;
                button.IconZoom = ExampleButton.IconZoom;
                button.Name = "button" + buttonCount;
                button.Click += button_Click;

                ListPanel.Controls.Add(button);
                ScrollToBottom();
                lastButtonBottom = button.Location.Y + button.Height;
            }
        }

        public void ScrollToBottom()
        {
            lock (this.ListPanel)
            {
                Panel p = this.ListPanel;
                using (Control c = new Control() { Parent = p, Dock = DockStyle.Bottom })
                {
                    p.ScrollControlIntoView(c);
                    c.Parent = null;
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if(activeButton == null)
            {
                //Chat selected
                activeButton = (BunifuFlatButton)sender;
                activeButton.selected = true;

                mainForm.serverConnection.SendLine("ConnectChat:" + activeButton.Text.Replace(" ", ""));
                //mainForm.serverConnection.startListen();
                mainForm.onlineStatusImage.Image = Properties.Resources.online_icon_S;
            }
            else
            {
                activeButton.selected = false;
                activeButton = (BunifuFlatButton)sender;
                activeButton.selected = true;
                mainForm.serverConnection.SendLine("ConnectChat:" + activeButton.Text.Replace(" ", ""));
                //mainForm.serverConnection.startListen();
                mainForm.onlineStatusImage.Image = Properties.Resources.online_icon_S;
            }
        }
    }
}
