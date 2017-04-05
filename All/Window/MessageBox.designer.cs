namespace All.Window
{
    partial class MessageBox
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
            this.tlpBody = new System.Windows.Forms.TableLayoutPanel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.metroButton3 = new All.Control.Metro.PicButton();
            this.metroButton2 = new All.Control.Metro.PicButton();
            this.metroButton1 = new All.Control.Metro.PicButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tlpBody.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpBody
            // 
            this.tlpBody.ColumnCount = 3;
            this.tlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.36813F));
            this.tlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.1044F));
            this.tlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.66483F));
            this.tlpBody.Controls.Add(this.lblMessage, 1, 2);
            this.tlpBody.Controls.Add(this.pnlBottom, 1, 3);
            this.tlpBody.Controls.Add(this.lblTitle, 1, 1);
            this.tlpBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBody.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tlpBody.Location = new System.Drawing.Point(0, 0);
            this.tlpBody.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlpBody.Name = "tlpBody";
            this.tlpBody.RowCount = 4;
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpBody.Size = new System.Drawing.Size(1214, 334);
            this.tlpBody.TabIndex = 7;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(238, 83);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(700, 191);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "message here";
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.Transparent;
            this.pnlBottom.Controls.Add(this.metroButton3);
            this.pnlBottom.Controls.Add(this.metroButton2);
            this.pnlBottom.Controls.Add(this.metroButton1);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.Location = new System.Drawing.Point(234, 274);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(704, 60);
            this.pnlBottom.TabIndex = 2;
            // 
            // metroButton3
            // 
            this.metroButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metroButton3.BackColor = System.Drawing.SystemColors.Control;
            this.metroButton3.BackImage = null;
            this.metroButton3.Border = true;
            this.metroButton3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.metroButton3.Location = new System.Drawing.Point(537, 0);
            this.metroButton3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.metroButton3.Name = "metroButton3";
            this.metroButton3.PicBackColor = System.Drawing.Color.Black;
            this.metroButton3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.metroButton3.Size = new System.Drawing.Size(130, 45);
            this.metroButton3.Style = All.Control.Metro.PicButton.Styles.Button;
            this.metroButton3.TabIndex = 2;
            this.metroButton3.Text = "取消";
            // 
            // metroButton2
            // 
            this.metroButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metroButton2.BackColor = System.Drawing.SystemColors.Control;
            this.metroButton2.BackImage = null;
            this.metroButton2.Border = true;
            this.metroButton2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.metroButton2.Location = new System.Drawing.Point(357, 0);
            this.metroButton2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.metroButton2.Name = "metroButton2";
            this.metroButton2.PicBackColor = System.Drawing.Color.Black;
            this.metroButton2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.metroButton2.Size = new System.Drawing.Size(130, 45);
            this.metroButton2.Style = All.Control.Metro.PicButton.Styles.Button;
            this.metroButton2.TabIndex = 1;
            this.metroButton2.Text = "否";
            // 
            // metroButton1
            // 
            this.metroButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metroButton1.BackColor = System.Drawing.SystemColors.Control;
            this.metroButton1.BackImage = null;
            this.metroButton1.Border = true;
            this.metroButton1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.metroButton1.Location = new System.Drawing.Point(188, 0);
            this.metroButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.PicBackColor = System.Drawing.Color.Black;
            this.metroButton1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.metroButton1.Size = new System.Drawing.Size(130, 45);
            this.metroButton1.Style = All.Control.Metro.PicButton.Styles.Button;
            this.metroButton1.TabIndex = 0;
            this.metroButton1.Text = "是";
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTitle.Location = new System.Drawing.Point(234, 8);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(231, 42);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "message title";
            // 
            // MetroMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(1214, 334);
            this.Controls.Add(this.tlpBody);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MetroMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MetroMessageBox";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MetroMessageBox_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MetroMessageBox_KeyUp);
            this.tlpBody.ResumeLayout(false);
            this.tlpBody.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpBody;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Panel pnlBottom;
        private Control.Metro.PicButton metroButton3;
        private Control.Metro.PicButton metroButton2;
        private Control.Metro.PicButton metroButton1;
        private System.Windows.Forms.Label lblTitle;
    }
}