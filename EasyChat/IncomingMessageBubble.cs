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
    public partial class IncomingMessageBubble : UserControl
    {
        public IncomingMessageBubble()
        {
            InitializeComponent();

            //Circle photo
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox1.Width - 3, pictureBox1.Height - 3);
            Region rg = new Region(gp);
            pictureBox1.Region = rg;
        }
        public IncomingMessageBubble(string message,string time)
        {
            InitializeComponent();

            //Circle photo
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox1.Width - 3, pictureBox1.Height - 3);
            Region rg = new Region(gp);
            pictureBox1.Region = rg;

            MessageLabel.Text = message;
            TimeLablel.Text = time;

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
    }
}
