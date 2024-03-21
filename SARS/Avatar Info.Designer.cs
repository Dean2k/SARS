namespace ARC
{
    partial class Avatar_Info
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Avatar_Info));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAvatarInfo = new MetroFramework.Controls.MetroTextBox();
            this.cbCopy = new MetroFramework.Controls.MetroComboBox();
            this.btnCopy = new MetroFramework.Controls.MetroButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtAvatarInfo);
            this.groupBox1.Controls.Add(this.cbCopy);
            this.groupBox1.Controls.Add(this.btnCopy);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(14, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 664);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Avatar/World Info";
            // 
            // txtAvatarInfo
            // 
            this.txtAvatarInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtAvatarInfo.CustomButton.Image = null;
            this.txtAvatarInfo.CustomButton.Location = new System.Drawing.Point(-338, 2);
            this.txtAvatarInfo.CustomButton.Name = "";
            this.txtAvatarInfo.CustomButton.Size = new System.Drawing.Size(593, 593);
            this.txtAvatarInfo.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtAvatarInfo.CustomButton.TabIndex = 1;
            this.txtAvatarInfo.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtAvatarInfo.CustomButton.UseSelectable = true;
            this.txtAvatarInfo.CustomButton.Visible = false;
            this.txtAvatarInfo.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtAvatarInfo.FontWeight = MetroFramework.MetroTextBoxWeight.Bold;
            this.txtAvatarInfo.Lines = new string[0];
            this.txtAvatarInfo.Location = new System.Drawing.Point(6, 19);
            this.txtAvatarInfo.MaxLength = 99999999;
            this.txtAvatarInfo.Multiline = true;
            this.txtAvatarInfo.Name = "txtAvatarInfo";
            this.txtAvatarInfo.PasswordChar = '\0';
            this.txtAvatarInfo.ReadOnly = true;
            this.txtAvatarInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAvatarInfo.SelectedText = "";
            this.txtAvatarInfo.SelectionLength = 0;
            this.txtAvatarInfo.SelectionStart = 0;
            this.txtAvatarInfo.ShortcutsEnabled = true;
            this.txtAvatarInfo.Size = new System.Drawing.Size(258, 598);
            this.txtAvatarInfo.TabIndex = 49;
            this.txtAvatarInfo.UseSelectable = true;
            this.txtAvatarInfo.UseStyleColors = true;
            this.txtAvatarInfo.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtAvatarInfo.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // cbCopy
            // 
            this.cbCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCopy.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbCopy.FontWeight = MetroFramework.MetroComboBoxWeight.Bold;
            this.cbCopy.FormattingEnabled = true;
            this.cbCopy.ItemHeight = 29;
            this.cbCopy.Items.AddRange(new object[] {
            "Time Dectected",
            "Avatar ID",
            "Avatar Name",
            "Avatar Description",
            "Author ID",
            "Author Name",
            "PC Asset URL",
            "Quest Asset URL",
            "Image URL",
            "Thumbnail URL",
            "Unity Version",
            "Release Status",
            "Tags",
            "World ID",
            "World Name"});
            this.cbCopy.Location = new System.Drawing.Point(6, 623);
            this.cbCopy.Name = "cbCopy";
            this.cbCopy.Size = new System.Drawing.Size(167, 35);
            this.cbCopy.Style = MetroFramework.MetroColorStyle.Blue;
            this.cbCopy.TabIndex = 48;
            this.cbCopy.UseSelectable = true;
            this.cbCopy.UseStyleColors = true;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnCopy.Location = new System.Drawing.Point(179, 623);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(85, 35);
            this.btnCopy.TabIndex = 48;
            this.btnCopy.Text = "Copy Info";
            this.btnCopy.UseSelectable = true;
            this.btnCopy.UseStyleColors = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // Avatar_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 750);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Avatar_Info";
            this.Text = "Avatar Info";
            this.Load += new System.EventHandler(this.Avatar_Info_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBox1;
        public MetroFramework.Controls.MetroTextBox txtAvatarInfo;
        public MetroFramework.Controls.MetroComboBox cbCopy;
        public MetroFramework.Controls.MetroButton btnCopy;
    }
}