namespace All.Window
{
    partial class MetroMessageBoxOld
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
            this.lblStr = new System.Windows.Forms.Label();
            this.btn2 = new All.Control.Metro.PicButton();
            this.btn1 = new All.Control.Metro.PicButton();
            this.SuspendLayout();
            // 
            // lblStr
            // 
            this.lblStr.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStr.Location = new System.Drawing.Point(29, 103);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(521, 21);
            this.lblStr.TabIndex = 3;
            this.lblStr.Text = "对不起,数据库连接失败";
            this.lblStr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn2
            // 
            this.btn2.BackColor = System.Drawing.Color.Purple;
            this.btn2.BackImage = null;
            this.btn2.Border = false;
            this.btn2.Text = "保存";
            this.btn2.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn2.ForeColor = System.Drawing.Color.White;
            this.btn2.Location = new System.Drawing.Point(398, 188);
            this.btn2.Name = "btn2";
            this.btn2.PicBackColor = System.Drawing.Color.Green;
            this.btn2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btn2.Size = new System.Drawing.Size(85, 28);
            this.btn2.TabIndex = 27;
            this.btn2.Visible = false;
            this.btn2.Click += new System.EventHandler(this.btn2_Click);
            // 
            // btn1
            // 
            this.btn1.BackColor = System.Drawing.Color.Purple;
            this.btn1.BackImage = null;
            this.btn1.Border = false;
            this.btn1.Text = "保存";
            this.btn1.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn1.ForeColor = System.Drawing.Color.White;
            this.btn1.Location = new System.Drawing.Point(283, 188);
            this.btn1.Name = "btn1";
            this.btn1.PicBackColor = System.Drawing.Color.Green;
            this.btn1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btn1.Size = new System.Drawing.Size(85, 28);
            this.btn1.TabIndex = 26;
            this.btn1.Visible = false;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // MetroMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 237);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.lblStr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MetroMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MetroMessageBox";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MetroMessageBox_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MetroMessageBox_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblStr;
        private Control.Metro.PicButton btn1;
        private Control.Metro.PicButton btn2;
    }
}