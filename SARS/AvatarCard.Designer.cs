namespace SARS
{
    partial class AvatarCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imgAvatar = new System.Windows.Forms.PictureBox();
            this.lblTitle = new MaterialSkin.Controls.MaterialLabel();
            this.lblBody = new MaterialSkin.Controls.MaterialLabel();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.lblAvatarId = new MaterialSkin.Controls.MaterialLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblPc = new System.Windows.Forms.Label();
            this.lblQuest = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // imgAvatar
            // 
            this.imgAvatar.Image = global::SARS.Properties.Resources.No_Image;
            this.imgAvatar.Location = new System.Drawing.Point(15, 19);
            this.imgAvatar.Name = "imgAvatar";
            this.imgAvatar.Size = new System.Drawing.Size(118, 115);
            this.imgAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgAvatar.TabIndex = 0;
            this.imgAvatar.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.Depth = 0;
            this.lblTitle.Font = new System.Drawing.Font("Roboto", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblTitle.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            this.lblTitle.Location = new System.Drawing.Point(139, 19);
            this.lblTitle.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(173, 26);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Title";
            // 
            // lblBody
            // 
            this.lblBody.AutoEllipsis = true;
            this.lblBody.Depth = 0;
            this.lblBody.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblBody.FontType = MaterialSkin.MaterialSkinManager.fontType.Caption;
            this.lblBody.Location = new System.Drawing.Point(141, 45);
            this.lblBody.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(163, 88);
            this.lblBody.TabIndex = 2;
            this.lblBody.Text = "Description";
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSize = false;
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(288, 6);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(24, 13);
            this.materialButton1.TabIndex = 3;
            this.materialButton1.Text = ">>>";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            // 
            // lblAvatarId
            // 
            this.lblAvatarId.AutoEllipsis = true;
            this.lblAvatarId.Depth = 0;
            this.lblAvatarId.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAvatarId.FontType = MaterialSkin.MaterialSkinManager.fontType.Caption;
            this.lblAvatarId.Location = new System.Drawing.Point(3, 133);
            this.lblAvatarId.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblAvatarId.Name = "lblAvatarId";
            this.lblAvatarId.Size = new System.Drawing.Size(300, 20);
            this.lblAvatarId.TabIndex = 4;
            this.lblAvatarId.Text = "AvatarID";
            this.lblAvatarId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPc
            // 
            this.lblPc.AutoSize = true;
            this.lblPc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPc.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.lblPc.Location = new System.Drawing.Point(12, 6);
            this.lblPc.Name = "lblPc";
            this.lblPc.Size = new System.Drawing.Size(23, 13);
            this.lblPc.TabIndex = 5;
            this.lblPc.Text = "PC";
            // 
            // lblQuest
            // 
            this.lblQuest.AutoSize = true;
            this.lblQuest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuest.ForeColor = System.Drawing.Color.Lime;
            this.lblQuest.Location = new System.Drawing.Point(98, 6);
            this.lblQuest.Name = "lblQuest";
            this.lblQuest.Size = new System.Drawing.Size(40, 13);
            this.lblQuest.TabIndex = 6;
            this.lblQuest.Text = "Quest";
            // 
            // AvatarCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblQuest);
            this.Controls.Add(this.lblPc);
            this.Controls.Add(this.lblAvatarId);
            this.Controls.Add(this.materialButton1);
            this.Controls.Add(this.lblBody);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.imgAvatar);
            this.Name = "AvatarCard";
            this.Size = new System.Drawing.Size(317, 152);
            this.Load += new System.EventHandler(this.AvatarCard_Load);
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.AControlRemoved);
            ((System.ComponentModel.ISupportInitialize)(this.imgAvatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox imgAvatar;
        public MaterialSkin.Controls.MaterialLabel lblTitle;
        public MaterialSkin.Controls.MaterialLabel lblBody;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        public MaterialSkin.Controls.MaterialLabel lblAvatarId;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblPc;
        private System.Windows.Forms.Label lblQuest;
    }
}
