
namespace ARC
{
    partial class HotswapConsole
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HotswapConsole));
            this.txtStatusText = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.finished = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // txtStatusText
            // 
            this.txtStatusText.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtStatusText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.txtStatusText.Location = new System.Drawing.Point(12, 12);
            this.txtStatusText.Multiline = true;
            this.txtStatusText.Name = "txtStatusText";
            this.txtStatusText.ReadOnly = true;
            this.txtStatusText.Size = new System.Drawing.Size(469, 191);
            this.txtStatusText.TabIndex = 0;
            this.txtStatusText.TextChanged += new System.EventHandler(this.txtStatusText_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(12, 209);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(469, 23);
            this.pbProgress.TabIndex = 2;
            // 
            // finished
            // 
            this.finished.Enabled = true;
            this.finished.Tick += new System.EventHandler(this.finished_Tick);
            // 
            // HotswapConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(489, 242);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.txtStatusText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "HotswapConsole";
            this.Text = "Hotswap";
            this.Load += new System.EventHandler(this.HotswapConsole_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.TextBox txtStatusText;
        public System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Timer finished;
    }
}