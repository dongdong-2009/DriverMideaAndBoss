using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace All.Window
{
    public partial class frmAddTextValue : BaseWindow
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public string Value
        { get; set; }

        public frmAddTextValue()
            : this("", "")
        { }
        public frmAddTextValue(string title)
            : this(title, "")
        { }
        public frmAddTextValue(string title, string value)
        {
            InitializeComponent();
            lblTitle.Text = title;
            txtValue.Text = value;
            this.Value = value;
        }
        private void frmAddTextValue_Load(object sender, EventArgs e)
        {
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Value = txtValue.Text;
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
