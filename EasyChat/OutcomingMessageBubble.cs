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
    public partial class OutcomingMessageBubble : UserControl
    {
        public OutcomingMessageBubble()
        {
            InitializeComponent();

            //Circle photo
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox1.Width - 3, pictureBox1.Height - 3);
            Region rg = new Region(gp);
            pictureBox1.Region = rg;
        }
        public OutcomingMessageBubble(string message,string time)
        {
            InitializeComponent();

            //Circle photo
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pictureBox1.Width - 3, pictureBox1.Height - 3);
            Region rg = new Region(gp);
            pictureBox1.Region = rg;

            MessageLabel.Text = message;
            TimeLablel.Text = time;

            Setheight();
        }

        void Setheight()
        {
            Size maxSize = new Size(500, int.MaxValue);
            Graphics g = CreateGraphics();
            SizeF size = g.MeasureString(MessageLabel.Text, MessageLabel.Font, MessageLabel.Width);
           
            MessageLabel.Height = int.Parse(Math.Round(size.Height + 10, 0).ToString());
            TimeLablel.Top = MessageLabel.Bottom + 5;
            this.Height = TimeLablel.Bottom + 5;
        }

        private void bubble_Resize(object sender, EventArgs e)
        {
            Setheight();
        }
    }
}
