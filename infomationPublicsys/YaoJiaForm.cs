﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace infomationPublicsys
{
    public partial class YaoJiaForm : Form
    {
        public YaoJiaForm()
        {
            InitializeComponent();
 
        }

        private void YaoJiaForm_Load(object sender, EventArgs e)
        {
            Program.WriteLog("进入药价列表界面");
            this.Size = new Size(7680, 7680);    
        }
    }
}
