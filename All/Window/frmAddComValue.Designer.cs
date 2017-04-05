namespace All.Window
{
    partial class frmAddComValue
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
            this.btnCancel = new All.Control.Metro.PicButton();
            this.btnOk = new All.Control.Metro.PicButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cbbValue = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.BackImage = null;
            this.btnCancel.Border = true;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(354, 98);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PicBackColor = System.Drawing.Color.Black;
            this.btnCancel.Size = new System.Drawing.Size(89, 28);
            this.btnCancel.Style = All.Control.Metro.PicButton.Styles.Button;
            this.btnCancel.TabIndex = 123;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.BackImage = null;
            this.btnOk.Border = true;
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Location = new System.Drawing.Point(463, 98);
            this.btnOk.Name = "btnOk";
            this.btnOk.PicBackColor = System.Drawing.Color.Black;
            this.btnOk.Size = new System.Drawing.Size(89, 28);
            this.btnOk.Style = All.Control.Metro.PicButton.Styles.Button;
            this.btnOk.TabIndex = 122;
            this.btnOk.Text = "确认";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 17);
            this.lblTitle.TabIndex = 120;
            // 
            // cbbValue
            // 
            this.cbbValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbValue.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbValue.FormattingEnabled = true;
            this.cbbValue.Location = new System.Drawing.Point(23, 48);
            this.cbbValue.Name = "cbbValue";
            this.cbbValue.Size = new System.Drawing.Size(538, 29);
            this.cbbValue.TabIndex = 124;
            // 
            // frmAddComValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 140);
            this.Controls.Add(this.cbbValue);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblTitle);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "frmAddComValue";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmAddComValue";
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.Load += new System.EventHandler(this.frmAddComValue_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Control.Metro.PicButton btnCancel;
        private Control.Metro.PicButton btnOk;
        internal System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox cbbValue;
    }
}