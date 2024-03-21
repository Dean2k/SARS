namespace ARC
{
    partial class AvatarPreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AvatarPreview));
            this.AvatarImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.AvatarImage)).BeginInit();
            this.SuspendLayout();
            // 
            // AvatarImage
            // 
            this.AvatarImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AvatarImage.Location = new System.Drawing.Point(0, 0);
            this.AvatarImage.Name = "AvatarImage";
            this.AvatarImage.Size = new System.Drawing.Size(800, 450);
            this.AvatarImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.AvatarImage.TabIndex = 0;
            this.AvatarImage.TabStop = false;
            // 
            // AvatarPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.AvatarImage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AvatarPreview";
            this.Text = "Avatar Image";
            this.Load += new System.EventHandler(this.Avatar_Preview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AvatarImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox AvatarImage;
    }
}