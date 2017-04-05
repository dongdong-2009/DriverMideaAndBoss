using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OFFICECORE = Microsoft.Office.Core;

namespace TestAll
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }
        private void Form4_Load(object sender, EventArgs e)
        {

            System.IO.FileInfo fi = new System.IO.FileInfo(".\\致青春.avi");
            mediaPlayer1.SetFile(fi.FullName);
            mediaPlayer1.Volumn = 20;
            mediaPlayer1.Position = 120;
            mediaPlayer1.Play();

            //this.BackgroundImage = switchPictureBox1.Images[0].Value;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //da.AllCommunite[0].Sons[0].WriteInvoke<string>("1", 0);
            //index = (int)All.Class.Num.GetRandom(0, 100);
            //dt.QueueUserWorkItem(new System.Threading.WaitCallback(AddValue), index);
        }
    }
}
