using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EasyChat
{
    public partial class ChatBox : UserControl
    {
        MainForm mainForm;
        int lastBubbleBottom = 0;

        public ChatBox()
        {
            InitializeComponent();
        }

        private void ChatBox_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Parent.Parent;
            MessageHistoryPanel.AutoScroll = false;
            MessageHistoryPanel.VerticalScroll.Enabled = true;
            MessageHistoryPanel.VerticalScroll.Visible = true;
            MessageHistoryPanel.AutoScroll = true;
        }
        public void AddNewIncomingMessage(string message, string time, string nick)
        {
            this.Invoke((MethodInvoker)delegate {
                lock ((object)lastBubbleBottom) {
                    ScrollToBottom();
                    IncomingMessageBubble bubble = new IncomingMessageBubble(stringNormalize(message), time, nick);
                    bubble.Location = new Point(3, lastBubbleBottom + 10);
                    bubble.Size = new Size(510, 68);
                    bubble.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                    MessageHistoryPanel.Controls.Add(bubble);
                    ScrollToBottom();
                    lastBubbleBottom = bubble.Location.Y + bubble.Height;
                }
            });
        }
        public void AddNewOutcomingMessage(string message, string time)
        {
            this.Invoke((MethodInvoker)delegate {
                lock ((object)lastBubbleBottom)
                {
                    ScrollToBottom();
                    OutcomingMessageBubble bubble = new OutcomingMessageBubble(stringNormalize(message), time);
                    bubble.Location = new Point(3, lastBubbleBottom + 10);
                    bubble.Size = new Size(510, 68);
                    bubble.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                    bubble.Left += 26;
                    MessageHistoryPanel.Controls.Add(bubble);
                    ScrollToBottom();
                    lastBubbleBottom = bubble.Location.Y + bubble.Height;
                }
            });
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            if (messageTextBox.Text != "" && messageTextBox.Text != "Write your message here")
            {
                mainForm.serverConnection.SendMessage(messageTextBox.Text);
                messageTextBox.Text = "";
            }
        }

        public void ScrollToBottom()
        {
                Panel p = this.MessageHistoryPanel;
                using (Control c = new Control() { Parent = p, Dock = DockStyle.Bottom })
                {
                    p.ScrollControlIntoView(c);
                    c.Parent = null;
                }
        }
        public void Clear()
        {
            MessageHistoryPanel.Controls.Clear();
            lastBubbleBottom = 0;
        }

            private void bunifuCustomTextbox1_Enter(object sender, EventArgs e)
        {
            if (messageTextBox.Text == "Write your message here")
            {
                messageTextBox.Text = "";
                messageTextBox.ForeColor = Color.Black;
            }
        }

        private void bunifuCustomTextbox1_Leave(object sender, EventArgs e)
        {
            if (messageTextBox.Text == "")
            {
                messageTextBox.Text = "Write your message here";
                messageTextBox.ForeColor = Color.FromArgb(149, 148, 150);
            }
        }

        private void bunifuCustomTextbox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.Enter && Control.ModifierKeys == Keys.Control))
                if (e.KeyCode == Keys.Enter)
                {
                    if(messageTextBox.Text != "")
                    mainForm.serverConnection.SendMessage(messageTextBox.Text);
                    messageTextBox.Text = "";
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
        }

        private string stringNormalize(string str)
        {
            Regex regex = new Regex(@"\S{24}");
            str = str.Replace('▶', '⮞');
            int i = 0;
            while(regex.IsMatch(str, i))
            {
                str = str.Insert(str.IndexOf(regex.Match(str, i).ToString(), i) + regex.Match(str, i).ToString().Length, " ");
                i = str.IndexOf(regex.Match(str, i).ToString(), i) + regex.Match(str, i).ToString().Length;
            }
            return str;
        }
    }
}
