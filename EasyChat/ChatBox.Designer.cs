namespace EasyChat
{
    partial class ChatBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatBox));
            this.MessageHistoryPanel = new System.Windows.Forms.Panel();
            this.messageTextBox = new WindowsFormsControlLibrary1.BunifuCustomTextbox();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.InputMessagePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            this.InputMessagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessageHistoryPanel
            // 
            this.MessageHistoryPanel.AutoScroll = true;
            this.MessageHistoryPanel.BackColor = System.Drawing.Color.LightGray;
            this.MessageHistoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageHistoryPanel.Location = new System.Drawing.Point(0, 0);
            this.MessageHistoryPanel.Name = "MessageHistoryPanel";
            this.MessageHistoryPanel.Size = new System.Drawing.Size(559, 360);
            this.MessageHistoryPanel.TabIndex = 1;
            // 
            // messageTextBox
            // 
            this.messageTextBox.BackColor = System.Drawing.Color.White;
            this.messageTextBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(130)))), ((int)(((byte)(182)))));
            this.messageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.messageTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(148)))), ((int)(((byte)(150)))));
            this.messageTextBox.Location = new System.Drawing.Point(3, 3);
            this.messageTextBox.MaxLength = 8168;
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageTextBox.Size = new System.Drawing.Size(506, 44);
            this.messageTextBox.TabIndex = 4;
            this.messageTextBox.TabStop = false;
            this.messageTextBox.Text = "Write your message here";
            this.messageTextBox.Enter += new System.EventHandler(this.bunifuCustomTextbox1_Enter);
            this.messageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bunifuCustomTextbox1_KeyDown);
            this.messageTextBox.Leave += new System.EventHandler(this.bunifuCustomTextbox1_Leave);
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(512, 3);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(44, 44);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 3;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            this.bunifuImageButton1.Click += new System.EventHandler(this.bunifuImageButton1_Click);
            // 
            // InputMessagePanel
            // 
            this.InputMessagePanel.BackColor = System.Drawing.Color.LightGray;
            this.InputMessagePanel.Controls.Add(this.messageTextBox);
            this.InputMessagePanel.Controls.Add(this.bunifuImageButton1);
            this.InputMessagePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InputMessagePanel.Location = new System.Drawing.Point(0, 360);
            this.InputMessagePanel.Name = "InputMessagePanel";
            this.InputMessagePanel.Size = new System.Drawing.Size(559, 50);
            this.InputMessagePanel.TabIndex = 2;
            // 
            // ChatBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MessageHistoryPanel);
            this.Controls.Add(this.InputMessagePanel);
            this.Name = "ChatBox";
            this.Size = new System.Drawing.Size(559, 410);
            this.Load += new System.EventHandler(this.ChatBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            this.InputMessagePanel.ResumeLayout(false);
            this.InputMessagePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel InputMessagePanel;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private WindowsFormsControlLibrary1.BunifuCustomTextbox messageTextBox;
        private System.Windows.Forms.Panel MessageHistoryPanel;
    }
}
