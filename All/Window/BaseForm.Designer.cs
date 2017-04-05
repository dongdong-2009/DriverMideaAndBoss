namespace All.Window
{
    partial class BaseForm
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
            this.PicMax = new System.Windows.Forms.PictureBox();
            this.PicMin = new System.Windows.Forms.PictureBox();
            this.PicExit = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicExit)).BeginInit();
            this.SuspendLayout();
            // 
            // PicMax
            // 
            this.PicMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PicMax.BackColor = System.Drawing.Color.Blue;
            this.PicMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PicMax.Location = new System.Drawing.Point(466, 5);
            this.PicMax.Name = "PicMax";
            this.PicMax.Size = new System.Drawing.Size(45, 27);
            this.PicMax.TabIndex = 15;
            this.PicMax.TabStop = false;
            this.PicMax.Click += new System.EventHandler(this.PicMax_Click);
            this.PicMax.MouseLeave += new System.EventHandler(this.PicMax_MouseLeave);
            this.PicMax.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PicMax_MouseMove);
            // 
            // PicMin
            // 
            this.PicMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PicMin.BackColor = System.Drawing.Color.Blue;
            this.PicMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PicMin.Location = new System.Drawing.Point(420, 5);
            this.PicMin.Name = "PicMin";
            this.PicMin.Size = new System.Drawing.Size(45, 27);
            this.PicMin.TabIndex = 14;
            this.PicMin.TabStop = false;
            this.PicMin.Click += new System.EventHandler(this.PicMin_Click);
            this.PicMin.MouseLeave += new System.EventHandler(this.PicMin_MouseLeave);
            this.PicMin.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PicMin_MouseMove);
            // 
            // PicExit
            // 
            this.PicExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PicExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.PicExit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PicExit.Location = new System.Drawing.Point(512, 5);
            this.PicExit.Name = "PicExit";
            this.PicExit.Size = new System.Drawing.Size(45, 27);
            this.PicExit.TabIndex = 13;
            this.PicExit.TabStop = false;
            this.PicExit.Click += new System.EventHandler(this.PicExit_Click);
            this.PicExit.MouseLeave += new System.EventHandler(this.PicExit_MouseLeave);
            this.PicExit.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PicExit_MouseMove);
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(562, 314);
            this.Controls.Add(this.PicMax);
            this.Controls.Add(this.PicMin);
            this.Controls.Add(this.PicExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BaseForm";
            ((System.ComponentModel.ISupportInitialize)(this.PicMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicExit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PicMax;
        private System.Windows.Forms.PictureBox PicMin;
        private System.Windows.Forms.PictureBox PicExit;
    }
}