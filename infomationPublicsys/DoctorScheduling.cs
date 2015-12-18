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
    public partial class DoctorScheduling : Form
    {
        public DoctorScheduling()
        {
            InitializeComponent();
        }

        private void DoctorScheduling_Load(object sender, EventArgs e)
        {
            Program.WriteLog("进入医生排班界面");
        }
    }
}
