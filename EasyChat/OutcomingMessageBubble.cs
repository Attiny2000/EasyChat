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

namespace EasyChat
{
    public partial class OutcomingMessageBubble : UserControl
    {
        public OutcomingMessageBubble()
        {
            InitializeComponent();
        }
        public OutcomingMessageBubble(string message, string time, string photo)
        {
            InitializeComponent();

            MessageLabel.Text = message;
            TimeLablel.Text = time;
            pictureBox1.ImageLocation = photo;

            SetHeight();
        }
        void SetHeight()
        {
            Size maxSize = new Size(MessageLabel.Width, int.MaxValue);
            MessageLabel.MaximumSize = maxSize;
            SizeF size = TextRenderer.MeasureText(MessageLabel.Text, MessageLabel.Font, maxSize, TextFormatFlags.WordBreak);

            MessageLabel.Height = int.Parse(Math.Ceiling((double)size.Height + 5).ToString());
            TimeLablel.Top = MessageLabel.Bottom + 5;
            this.Height = TimeLablel.Bottom + 5;
        }

        private void bubble_Resize(object sender, EventArgs e)
        {
            SetHeight();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MessageLabel.Text);
        }
    }
}
