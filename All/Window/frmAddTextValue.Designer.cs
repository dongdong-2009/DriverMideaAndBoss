namespace All.Window
{
    partial class frmAddTextValue
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.btnCancel = new All.Control.Metro.PicButton();
            this.btnOk = new All.Control.Metro.PicButton();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 17);
            this.lblTitle.TabIndex = 116;
            // 
            // txtValue
            // 
            this.txtValue.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValue.Location = new System.Drawing.Point(15, 49);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(545, 33);
            this.txtValue.TabIndex = 117;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.BackImage = null;
            this.btnCancel.Border = true;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(347, 99);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PicBackColor = System.Drawing.Color.Black;
            this.btnCancel.Size = new System.Drawing.Size(89, 28);
            this.btnCancel.Style = All.Control.Metro.PicButton.Styles.Button;
            this.btnCancel.TabIndex = 119;
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
            this.btnOk.Location = new System.Drawing.Point(456, 99);
            this.btnOk.Name = "btnOk";
            this.btnOk.PicBackColor = System.Drawing.Color.Black;
            this.btnOk.Size = new System.Drawing.Size(89, 28);
            this.btnOk.Style = All.Control.Metro.PicButton.Styles.Button;
            this.btnOk.TabIndex = 118;
            this.btnOk.Text = "确认";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmAddTextValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 141);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtValue);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "frmAddTextValue";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAddTextValue";
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.Load += new System.EventHandler(this.frmAddTextValue_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label lblTitle;
        internal System.Windows.Forms.TextBox txtValue;
        private Control.Metro.PicButton btnCancel;
        private Control.Metro.PicButton btnOk;
    }
}