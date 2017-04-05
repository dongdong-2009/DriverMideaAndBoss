namespace All.Control
{
    partial class MediaPlayDirect
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
            this.rspMediaPlayer1 = new RSPMediaPlayer.RSPMediaPlayer();
            this.SuspendLayout();
            // 
            // rspMediaPlayer1
            // 
            this.rspMediaPlayer1.BackColor = System.Drawing.Color.Black;
            this.rspMediaPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rspMediaPlayer1.Location = new System.Drawing.Point(0, 0);
            this.rspMediaPlayer1.Name = "rspMediaPlayer1";
            this.rspMediaPlayer1.Size = new System.Drawing.Size(565, 372);
            this.rspMediaPlayer1.TabIndex = 0;
            // 
            // MediaPlayDirect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rspMediaPlayer1);
            this.Name = "MediaPlayDirect";
            this.Size = new System.Drawing.Size(565, 372);
            this.ResumeLayout(false);

        }

        #endregion

        private RSPMediaPlayer.RSPMediaPlayer rspMediaPlayer1;
    }
}
