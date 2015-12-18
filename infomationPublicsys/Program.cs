using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml;
using infomationPublicsys;
using System.Diagnostics;
using infomationPublicsys;
 

namespace infomationPublicsys
{
    static class Program
    {
        public static String serviceIp = ComputerInfo.GetLocalIpv4();  //"127.0.0.1";   
        public static int g_location_x = 1360;  //后期从配置文件读取
        public static int g_location_y = 0;
        public static String g_computerIp = "";
        public static int myProt = 9093;   //端口

        //web服务相关变量
        public static string g_fzxtWebServiceURL = "";
        //public static LED.fzxtWebService.FzxtWebService g_FzxtService = null;
 
        //静态构造函数  读配置文件
        static Program()
        {
            INIClass ini = new INIClass(@"D:\deviceCfg.ini");   //------文件 deviceCfg.ini 的路径
            bool exist = ini.test(); //检查文件是否存在
            if (exist)
            {
                g_location_x =  100; // int.Parse(ini.IniReadValue("ComputerInfo", "left"));//读取 
                g_location_y = 100;  //int.Parse(ini.IniReadValue("ComputerInfo", "top"));      

                g_fzxtWebServiceURL = ini.IniReadValue("WebService", "fzxtService");
                if (g_fzxtWebServiceURL.Equals("") || g_fzxtWebServiceURL.Equals("null"))
                {
                    //ini.IniWriteValue("WebService", "fzxtService", "http://sort.tjh.com:8080/fzxt_tj/services/FzxtWebService?wsdl");
                    //g_fzxtWebServiceURL = ini.IniReadValue("WebService", "fzxtService");

                }
                //g_FzxtService = new LED.fzxtWebService.FzxtWebService();// 
                //g_FzxtService.Url = g_fzxtWebServiceURL;
            }
            else
            {
                //创建配置文件，并初始化配置文件               
                try
                {
                    ini.IniWriteValue("WebService", "fzxtService", "http://sort.tjh.com:8080/fzxt_tj/services/FzxtSocketService?wsdl");
                  
                }
                catch
                {
                    throw (new ApplicationException("Ini文件不存在"));

                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //Cursor.Hide();         

            Application.Run(new Baseform());
            //Application.Run(new TestForm()); 
            //  Application.Run(new Form_fyk()); 

        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            WriteLog("CurrentDomain_UnhandledException: " + sender.GetType().FullName + " " + (e.ExceptionObject as System.Exception).ToString());
            //new StaticErorMessage("系统异常,请联系管理人员").Show();
            Program.WriteLog("系统异常,请联系管理人员");
            return;
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            WriteLog("Application_ThreadException: " + sender.GetType().FullName + " " + e.Exception.ToString());
            //new StaticErorMessage("系统异常,请联系管理人员").Show();
            Program.WriteLog("系统异常,请联系管理人员");
            return;
        }
        private static byte[] result = new byte[1024];
        static Socket serverSocket;

        public static void Service()
        {
            try
            {
                //服务器IP地址  
                IPAddress ip = IPAddress.Parse(serviceIp);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
                serverSocket.Listen(10);    //设定最多10个排队连接请求  
                Program.WriteLog("启动监听成功" + serverSocket.LocalEndPoint.ToString());
            }
            catch (Exception socketEerror)
            {
                WriteLog("启动socket通信服务端发生异常！" + socketEerror.Message);
                //new StaticErorMessage("系统异常,请重新启动").Show();
                Program.WriteLog("系统异常,请联系管理人员");
                return;

            }
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();

        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>
        private static void ListenClientConnect()
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    Program.WriteLog("建立连接");
                    // clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));  //链接成功向客户端发送消息
                    Thread receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start(clientSocket);
                }
            }
            catch (Exception error)
            {
                Program.WriteLog("socket通信服务端 监听启动失败" + error.ToString());
                //new StaticErorMessage("系统异常,请重新启动").Show();
                Program.WriteLog("系统异常,请联系管理人员");
                return;
            }
        }

        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                        string recvStr = "";
                        //通过clientSocket接收数据  
                        int receiveNumber = myClientSocket.Receive(result);
                        //Encoding encoding = Encoding.GetEncoding("GBK");
                        //  recvStr += Encoding.ASCII.GetString(result, 0, receiveNumber);  
                        recvStr += UTF8Encoding.UTF8.GetString(result, 0, receiveNumber);
                        Program.WriteLog("接收客户端" + myClientSocket.RemoteEndPoint.ToString() + "消息:" + recvStr);
                        //给Client端返回信息
                        string sendStr = "close\n";                       
                        byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                        myClientSocket.Send(bs, bs.Length, 0);
                        myClientSocket.Close();
                        break;
                    
                }
                catch (Exception ex)
                {
                    Program.WriteLog(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }

        }



        // 文件日志
        public static void WriteLog(string strLogInfo)
        {
            DateTime dt = DateTime.Now;

            string strLogName = "fzxt_tj_" + dt.ToString("yyyy-MM-dd") + ".log";

            try
            {
                if (!Directory.Exists("D:\\javafzxt\\Logs"))
                    Directory.CreateDirectory("D:\\javafzxt\\Logs");

                FileStream fs = new FileStream("D:\\javafzxt\\Logs\\" + strLogName, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                if (strLogInfo != "")
                {
                    Console.WriteLine(dt.ToString("HH:mm:ss:fff") + "   " + strLogInfo);
                    sw.WriteLine(dt.ToString("HH:mm:ss:fff") + "   " + strLogInfo);
                }

                sw.Flush();
                sw.Close();

                fs.Close();
            }
            catch (System.Exception error)
            {
                WriteLog(error.Message);
                return;
            }
        }


         

        public static String Week()
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            return week;
        }


        public static String SpaceStrFormat(String str, int n_space)
        {
            int len = Encoding.GetEncoding("gb2312").GetBytes(str).Length;
            if (len > n_space)
            {
                if (str.Length > 4)
                {
                    str = str.Substring(0, 4);
                }
                //if (str.Length = 5)
                //{
                //    str = str.Substring(0, 4);
                //}
                len = Encoding.GetEncoding("gb2312").GetBytes(str).Length;
            }
            str = str + new string(' ', n_space - len);
            return str;
        }



    }
}
