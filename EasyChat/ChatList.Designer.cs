namespace EasyChat
{
    partial class ChatList
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatList));
            this.ExampleButton = new Bunifu.Framework.UI.BunifuFlatButton();
            this.ListPanel = new System.Windows.Forms.Panel();
            this.ListPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExampleButton
            // 
            this.ExampleButton.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.ExampleButton.BackColor = System.Drawing.Color.Transparent;
            this.ExampleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ExampleButton.BorderRadius = 0;
            this.ExampleButton.ButtonText = "  ChatRoomName";
            this.ExampleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExampleButton.DisabledColor = System.Drawing.Color.White;
            this.ExampleButton.Enabled = false;
            this.ExampleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.ExampleButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(101)))), ((int)(((byte)(152)))));
            this.ExampleButton.Iconcolor = System.Drawing.Color.Transparent;
            this.ExampleButton.Iconimage = ((System.Drawing.Image)(resources.GetObject("ExampleButton.Iconimage")));
            this.ExampleButton.Iconimage_right = null;
            this.ExampleButton.Iconimage_right_Selected = null;
            this.ExampleButton.Iconimage_Selected = null;
            this.ExampleButton.IconMarginLeft = 5;
            this.ExampleButton.IconMarginRight = 0;
            this.ExampleButton.IconRightVisible = false;
            this.ExampleButton.IconRightZoom = 0D;
            this.ExampleButton.IconVisible = true;
            this.ExampleButton.IconZoom = 70D;
            this.ExampleButton.IsTab = true;
            this.ExampleButton.Location = new System.Drawing.Point(0, 0);
            this.ExampleButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ExampleButton.Name = "ExampleButton";
            this.ExampleButton.Normalcolor = System.Drawing.Color.Transparent;
            this.ExampleButton.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.ExampleButton.OnHoverTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(101)))), ((int)(((byte)(152)))));
            this.ExampleButton.selected = false;
            this.ExampleButton.Size = new System.Drawing.Size(241, 58);
            this.ExampleButton.TabIndex = 1;
            this.ExampleButton.TabStop = false;
            this.ExampleButton.Text = "  ChatRoomName";
            this.ExampleButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExampleButton.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(101)))), ((int)(((byte)(152)))));
            this.ExampleButton.TextFont = new System.Drawing.Font("Century", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ExampleButton.Visible = false;
            // 
            // ListPanel
            // 
            this.ListPanel.AutoScroll = true;
            this.ListPanel.Controls.Add(this.ExampleButton);
            this.ListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListPanel.Location = new System.Drawing.Point(0, 0);
            this.ListPanel.Name = "ListPanel";
            this.ListPanel.Size = new System.Drawing.Size(241, 410);
            this.ListPanel.TabIndex = 2;
            // 
            // ChatList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.ListPanel);
            this.Name = "ChatList";
            this.Size = new System.Drawing.Size(241, 410);
            this.Load += new System.EventHandler(this.ChatList_Load);
            this.ListPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuFlatButton ExampleButton;
        private System.Windows.Forms.Panel ListPanel;
    }
}
