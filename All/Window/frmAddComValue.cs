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
    public partial class frmAddComValue  : BaseWindow
    {
        /// <summary>
        /// 返回值
        /// </summary>
        public string Value
        { get; set; }

        public frmAddComValue(string title, string[] value)
        {
            InitializeComponent();
            lblTitle.Text = title;
            if (value != null)
            {
                foreach (string ss in value)
                {
                    cbbValue.Items.Add(ss);
                }
            }
        }
        private void frmAddComValue_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Value = cbbValue.Text;
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
