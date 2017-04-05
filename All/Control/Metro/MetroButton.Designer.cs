namespace All.Control.Metro
{
    partial class MetroButton
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetroButton));
            this.lblUser = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.Pic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Pic)).BeginInit();
            this.SuspendLayout();
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(68, 75);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(66, 17);
            this.lblUser.TabIndex = 56;
            this.lblUser.Text = "LoginOut";
            this.lblUser.Click += new System.EventHandler(this.Pic_Click);
            this.lblUser.MouseEnter += new System.EventHandler(this.Pic_MouseEnter);
            this.lblUser.MouseLeave += new System.EventHandler(this.Pic_MouseLeave);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(68, 52);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(42, 17);
            this.lblTitle.TabIndex = 55;
            this.lblTitle.Text = "登出";
            this.lblTitle.Click += new System.EventHandler(this.Pic_Click);
            this.lblTitle.MouseEnter += new System.EventHandler(this.Pic_MouseEnter);
            this.lblTitle.MouseLeave += new System.EventHandler(this.Pic_MouseLeave);
            // 
            // Pic
            // 
            this.Pic.Image = ((System.Drawing.Image)(resources.GetObject("Pic.Image")));
            this.Pic.Location = new System.Drawing.Point(20, 52);
            this.Pic.Name = "Pic";
            this.Pic.Size = new System.Drawing.Size(45, 45);
            this.Pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Pic.TabIndex = 54;
            this.Pic.TabStop = false;
            this.Pic.Click += new System.EventHandler(this.Pic_Click);
            this.Pic.MouseEnter += new System.EventHandler(this.Pic_MouseEnter);
            this.Pic.MouseLeave += new System.EventHandler(this.Pic_MouseLeave);
            // 
            // MetroButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.Pic);
            this.Name = "MetroButton";
            ((System.ComponentModel.ISupportInitialize)(this.Pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox Pic;
    }
}
