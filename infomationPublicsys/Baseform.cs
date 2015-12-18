using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace infomationPublicsys
{
    public partial class Baseform : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowA")]
        public static extern IntPtr FindWindowA(string lp1, string lp2);
        //[DllImport("user32.dll", EntryPoint = "ShowWindow")]
        //public static extern IntPtr ShowWindow(IntPtr hWnd, int _value);

        IntPtr hTray = Baseform.FindWindowA("Shell_TrayWnd", String.Empty);

        public Baseform()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point( 11, 15);
            InitializeComponent();
            //Baseform.ShowWindow(hTray, 9);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            new YaoJiaForm().Show();
        }

        private void Baseform_Load(object sender, EventArgs e)
        {
            try
            {
                Program.WriteLog("=======>>进入Baseform页面");
               //启动监听
                Program.Service();
             
              //  string sendStr = Program.g_LEDReceiveText;
                
                //得到D:\javafzxt\Logs"文件夹下所有 
                DirectoryInfo di = new DirectoryInfo(@"D:\javafzxt\Logs");
                FileInfo[] fi = di.GetFiles("*.log");

                String shumu = fi.Length.ToString();// 文件的个数
                Program.WriteLog("过期的文件数目：" + shumu);
                DateTime dtNow = DateTime.Now;   

                foreach (FileInfo tmpfi in fi)
                {
                    if (tmpfi.Name != "1.log")
                    {
                        //tmpfi.CreationTime;//创建时间
                        TimeSpan ts = dtNow.Subtract(tmpfi.LastWriteTime);
                        if (ts.TotalDays > 7)//距现在30分钟以上    TotalMinutes
                        {
                            tmpfi.Delete();//删除文件
                        }

                    }
                }
                Program.WriteLog("成功删除7天之前的日志文件文件");

            }
            catch (Exception Error)
            {
                Program.WriteLog("系统异常" + Error.ToString());
                
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        
    }
}
