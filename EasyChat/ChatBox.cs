using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyChat
{
    public partial class ChatBox : UserControl
    {
        VScrollBar myScrollBar = null;
        int lastBottom = 0;

        public ChatBox()
        {
            InitializeComponent();

            //myScrollBar = new VScrollBar();
            //myScrollBar.Height = MessageHistoryPanel.Height;
            //myScrollBar.Left = MessageHistoryPanel.Width - myScrollBar.Width;
            //myScrollBar.Top = 0;
            //myScrollBar.Enabled = false;
            //MessageHistoryPanel.Controls.Add(myScrollBar);
        }

        private void ChatBox_Load(object sender, EventArgs e)
        {
            MessageHistoryPanel.AutoScroll = false;
            MessageHistoryPanel.VerticalScroll.Enabled = true;
            MessageHistoryPanel.VerticalScroll.Visible = true;
            MessageHistoryPanel.AutoScroll = true;
        }
        public void AddNewIncomingMessage(string message, string time)
        {
            IncomingMessageBubble bubble = new IncomingMessageBubble(message, time);
            bubble.Location = new Point(3, 3);
            bubble.Size = new Size(510, 68);
            bubble.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            bubble.Top = lastBottom + 10;
            MessageHistoryPanel.Controls.Add(bubble);
            ScrollToBottom();
            lastBottom = bubble.Bottom;
            //if (MessageHistoryPanel.VerticalScroll.Visible == true) { myScrollBar.Visible = false; }
        }
        public void AddNewOutcomingMessage(string message, string time)
        {
            OutcomingMessageBubble bubble = new OutcomingMessageBubble(message, time);
            bubble.Location = new Point(3, 3);
            bubble.Size = new Size(510, 68);
            bubble.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            bubble.Left += 26;
            bubble.Top = lastBottom + 10;
            MessageHistoryPanel.Controls.Add(bubble);
            ScrollToBottom();
            lastBottom = bubble.Bottom;
            //if (MessageHistoryPanel.VerticalScroll.Visible == true) { myScrollBar.Visible = false; }
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            AddNewOutcomingMessage("Hello, that's test for dynamic rendreing message bubbles.", DateTime.Now.ToString() + " ");
            AddNewIncomingMessage("Ok, here is short message and message with long text", DateTime.Now.ToString() + " ");
            AddNewIncomingMessage("Instant messaging (IM) technology is a type of online chat that offers real-time text transmission over the Internet. A LAN messenger operates in a similar way over a local area network. Short messages are typically transmitted between two parties, when each user chooses to complete a thought and select \"send\".", DateTime.Now.ToString() + " ");
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

        private void bunifuCustomTextbox1_Enter(object sender, EventArgs e)
        {
            if (bunifuCustomTextbox1.Text == "Write your message here")
            {
                bunifuCustomTextbox1.Text = "";
                bunifuCustomTextbox1.ForeColor = Color.Black;
            }
        }

        private void bunifuCustomTextbox1_Leave(object sender, EventArgs e)
        {
            if (bunifuCustomTextbox1.Text == "")
            {
                bunifuCustomTextbox1.Text = "Write your message here";
                bunifuCustomTextbox1.ForeColor = Color.FromArgb(149, 148, 150);
            }
        }

        private void bunifuCustomTextbox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.Enter && Control.ModifierKeys == Keys.Control))
                if (e.KeyCode == Keys.Enter)
                {
                    //TODO
                    bunifuCustomTextbox1.Text = "";
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
        }
    }
}
