namespace All.Window
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.lblTitle = new System.Windows.Forms.Label();
            this.panLeftSpace = new System.Windows.Forms.Panel();
            this.panRightSpace = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(161, 85);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(308, 40);
            this.lblTitle.TabIndex = 46;
            this.lblTitle.Text = "海尔检测系统-信息化";
            // 
            // panLeftSpace
            // 
            this.panLeftSpace.Dock = System.Windows.Forms.DockStyle.Right;
            this.panLeftSpace.Location = new System.Drawing.Point(805, 5);
            this.panLeftSpace.Name = "panLeftSpace";
            this.panLeftSpace.Size = new System.Drawing.Size(150, 530);
            this.panLeftSpace.TabIndex = 47;
            // 
            // panRightSpace
            // 
            this.panRightSpace.Dock = System.Windows.Forms.DockStyle.Left;
            this.panRightSpace.Location = new System.Drawing.Point(5, 5);
            this.panRightSpace.Name = "panRightSpace";
            this.panRightSpace.Size = new System.Drawing.Size(150, 530);
            this.panRightSpace.TabIndex = 48;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panRightSpace);
            this.Controls.Add(this.panLeftSpace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainWindow_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panLeftSpace;
        private System.Windows.Forms.Panel panRightSpace;
    }
}