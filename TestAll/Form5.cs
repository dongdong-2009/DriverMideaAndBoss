using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace TestAll
{
    public partial class Form5 : All.Window.BaseWindow
    {
        public Form5()
        {
            InitializeComponent();
        }
        private void listBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.DrawString(listBox2.Items[e.Index].ToString(), listBox2.Font, new SolidBrush(listBox2.ForeColor), new PointF(0, e.Bounds.Top));
        }

        private void Form5_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据");
            listBox2.Items.Add("这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据这里添加数据");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBox2.SelectedIndex = listBox2.Items.Count - 1;
        }
    }
}
