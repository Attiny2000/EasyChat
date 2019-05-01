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
    public partial class Bubble : UserControl
    {
        public Bubble()
        {
            InitializeComponent();
        }
        public enum MessageType { In, Out}
        public Bubble(string message,string time,MessageType messageType)
        {
            InitializeComponent();
            MessageLabel.Text = message;
            TimeLablel.Text = time;

            if(messageType.ToString()=="In")
            {
                this.BackColor = Color.FromArgb(29, 101, 152);
            }
            else if (messageType.ToString() == "Out")
            {
                this.BackColor = Color.Gray;
            }

            Setheight();
        }

        void Setheight()
        {
            Size maxSize = new Size(500, int.MaxValue);
            Graphics g = CreateGraphics();
            SizeF size = g.MeasureString(MessageLabel.Text, MessageLabel.Font, MessageLabel.Width);
           
            MessageLabel.Height = int.Parse(Math.Round(size.Height + 2, 0).ToString());
            TimeLablel.Top = MessageLabel.Bottom + 10;
            this.Height = TimeLablel.Bottom + 10;
        }

        private void bubble_Resize(object sender, EventArgs e)
        {
            Setheight();
        }
    }
}
