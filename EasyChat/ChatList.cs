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

namespace EasyChat
{
    public partial class ChatList : UserControl
    {
        int buttonCount = 0;
        int lastButtonBottom = 0;
        BunifuFlatButton activeButton = null;

        public ChatList()
        {
            InitializeComponent();
        }
        private void ChatList_Load(object sender, EventArgs e)
        {
            AddNewButton("TestButton1");
            AddNewButton("TestButton2");
            AddNewButton("TestButton3");
            AddNewButton("TestButton4");
            AddNewButton("TestButton5");
            AddNewButton("TestButton6");
            AddNewButton("TestButton7");
            AddNewButton("TestButton8");
            AddNewButton("TestButton9");
            AddNewButton("TestButton10");
            AddNewButton("TestButton11");
            AddNewButton("TestButton12");
            AddNewButton("TestButton13");
            AddNewButton("TestButton14");
            AddNewButton("TestButton15");
            AddNewButton("TestButton16");
            AddNewButton("TestButton17");
            AddNewButton("TestButton18");
            AddNewButton("TestButton19");
            AddNewButton("TestButton20");
            AddNewButton("TestButton21");
            AddNewButton("TestButton23");
            AddNewButton("TestButton24");
            AddNewButton("TestButton25");

            ListPanel.AutoScroll = false;
            ListPanel.VerticalScroll.Enabled = false;
            ListPanel.VerticalScroll.Visible = false;
            ListPanel.HorizontalScroll.Enabled = false;
            ListPanel.HorizontalScroll.Visible = false;
            ListPanel.HorizontalScroll.Maximum = 0;
            ListPanel.VerticalScroll.Maximum = 0;
            ListPanel.AutoScroll = true;
        }
        private void AddNewButton(string buttonText)
        {
            BunifuFlatButton button = new BunifuFlatButton();
            button.Font = ExampleButton.Font;
            button.Size = ExampleButton.Size;
            button.Location = new Point(0, 0);
            button.Top += lastButtonBottom;
            lastButtonBottom = button.Bottom - 1;
            button.IsTab = ExampleButton.IsTab;
            button.selected = false;
            button.Text = "  " + buttonText;
            button.Activecolor = ExampleButton.Activecolor;
            button.ForeColor = ExampleButton.ForeColor;
            button.Textcolor = ExampleButton.Textcolor;
            button.TextAlign = ExampleButton.TextAlign;
            button.Normalcolor = ExampleButton.Normalcolor;
            button.colbackground = ExampleButton.colbackground;
            button.OnHovercolor = ExampleButton.OnHovercolor;
            button.Anchor = ExampleButton.Anchor;
            button.Iconimage = ExampleButton.Iconimage;
            button.IconZoom = ExampleButton.IconZoom;
            button.Name = "button" + buttonCount;
            button.Click += button_Click;

            ListPanel.Controls.Add(button);
        }

        private void button_Click(object sender, EventArgs e)
        {
            if(activeButton == null)
            {
                activeButton = (BunifuFlatButton)sender;
                activeButton.selected = true;
            }
            else
            {
                activeButton.selected = false;
                activeButton = (BunifuFlatButton)sender;
                activeButton.selected = true;
            }
        }
    }
}
