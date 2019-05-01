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
        public ChatBox()
        {
            if (!this.DesignMode) { InitializeComponent(); }

            oldBubble.Top = 0 - oldBubble.Height + 10;
        }

        Bubble oldBubble = new Bubble();

        public void AddNewIncomingMessage(string message, string time)
        {
            Bubble bubble = new Bubble(message, time, Bubble.MessageType.In);
            bubble.Location = bubble1.Location;
            bubble.Size = bubble1.Size;
            bubble.Anchor = bubble1.Anchor;
            bubble.Top = oldBubble.Bottom + 10;
            MessageHistoryPanel.Controls.Add(bubble);
            oldBubble = bubble;
        }
        public void AddNewOutcomingMessage(string message, string time)
        {
            Bubble bubble = new Bubble(message, time, Bubble.MessageType.Out);
            bubble.Location = bubble1.Location;
            bubble.Left += 20;
            bubble.Size = bubble1.Size;
            bubble.Anchor = bubble1.Anchor;
            bubble.Top = oldBubble.Bottom + 10;
            MessageHistoryPanel.Controls.Add(bubble);
            oldBubble = bubble;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            AddNewOutcomingMessage("Hello, that's test for dynamic rendreing message bubbles.", DateTime.Now.ToString() + " ");
            AddNewIncomingMessage("Ok, here is short message and message with long text", DateTime.Now.ToString() + " ");
            AddNewIncomingMessage("Instant messaging (IM) technology is a type of online chat that offers real-time text transmission over the Internet. A LAN messenger operates in a similar way over a local area network. Short messages are typically transmitted between two parties, when each user chooses to complete a thought and select \"send\".", DateTime.Now.ToString() + " ");
            MessageHistoryPanel.VerticalScroll.Value = MessageHistoryPanel.VerticalScroll.Maximum;
        }
    }
}
