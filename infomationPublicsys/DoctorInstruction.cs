using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace infomationPublicsys
{
    public partial class DoctorInstruction : Form
    {
        public DoctorInstruction()
        {
            InitializeComponent();
        }

        private void DoctorInstruction_Load(object sender, EventArgs e)
        {
            Program.WriteLog("进入医生说明界面");
           // this.pictureBox1.ImageLocation = "@D:\sch\doctor\100043.jpg";
            this.pictureBox1.Image=Image.FromFile("D:\\sch\\doctor\\100043.jpg");

            this.pictureBox2.Image = Image.FromFile("D:\\sch\\doctor\\100232.jpg");


        }
    }
}
