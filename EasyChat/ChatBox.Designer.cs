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
            this.InputMessagePanel = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.MessageHistoryPanel = new System.Windows.Forms.Panel();
            this.bubble1 = new EasyChat.Bubble();
            this.InputMessagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            this.MessageHistoryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputMessagePanel
            // 
            this.InputMessagePanel.BackColor = System.Drawing.Color.LightGray;
            this.InputMessagePanel.Controls.Add(this.bunifuImageButton1);
            this.InputMessagePanel.Controls.Add(this.metroTextBox1);
            this.InputMessagePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InputMessagePanel.Location = new System.Drawing.Point(0, 360);
            this.InputMessagePanel.Name = "InputMessagePanel";
            this.InputMessagePanel.Size = new System.Drawing.Size(559, 50);
            this.InputMessagePanel.TabIndex = 0;
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
            // metroTextBox1
            // 
            this.metroTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroTextBox1.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = null;
            this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(464, 2);
            this.metroTextBox1.CustomButton.Name = "";
            this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(39, 39);
            this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.CustomButton.TabIndex = 1;
            this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.CustomButton.UseSelectable = true;
            this.metroTextBox1.CustomButton.Visible = false;
            this.metroTextBox1.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.metroTextBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.metroTextBox1.Icon = ((System.Drawing.Image)(resources.GetObject("metroTextBox1.Icon")));
            this.metroTextBox1.IconRight = true;
            this.metroTextBox1.Lines = new string[0];
            this.metroTextBox1.Location = new System.Drawing.Point(3, 3);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Multiline = true;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.PromptText = "Write your message here";
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.SelectionLength = 0;
            this.metroTextBox1.SelectionStart = 0;
            this.metroTextBox1.ShortcutsEnabled = true;
            this.metroTextBox1.Size = new System.Drawing.Size(506, 44);
            this.metroTextBox1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.TabIndex = 2;
            this.metroTextBox1.TabStop = false;
            this.metroTextBox1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.UseSelectable = true;
            this.metroTextBox1.WaterMark = "Write your message here";
            this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Century", 11F);
            // 
            // MessageHistoryPanel
            // 
            this.MessageHistoryPanel.AutoScroll = true;
            this.MessageHistoryPanel.BackColor = System.Drawing.Color.LightGray;
            this.MessageHistoryPanel.Controls.Add(this.bubble1);
            this.MessageHistoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageHistoryPanel.Location = new System.Drawing.Point(0, 0);
            this.MessageHistoryPanel.Name = "MessageHistoryPanel";
            this.MessageHistoryPanel.Size = new System.Drawing.Size(559, 360);
            this.MessageHistoryPanel.TabIndex = 1;
            // 
            // bubble1
            // 
            this.bubble1.BackColor = System.Drawing.Color.DimGray;
            this.bubble1.Location = new System.Drawing.Point(8, 3);
            this.bubble1.Name = "bubble1";
            this.bubble1.Size = new System.Drawing.Size(510, 71);
            this.bubble1.TabIndex = 0;
            this.bubble1.Visible = false;
            // 
            // ChatBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MessageHistoryPanel);
            this.Controls.Add(this.InputMessagePanel);
            this.Name = "ChatBox";
            this.Size = new System.Drawing.Size(559, 410);
            this.InputMessagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            this.MessageHistoryPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel InputMessagePanel;
        private System.Windows.Forms.Panel MessageHistoryPanel;
        private MetroFramework.Controls.MetroTextBox metroTextBox1;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bubble bubble1;
    }
}
