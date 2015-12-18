using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;


namespace infomationPublicsys
{
    class INIClass
    {


        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public INIClass(string INIPath)
        {
            inipath = INIPath;
        }

        //-----------写入 deviceCfg.ini 文件-----------
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }

        //-----------读取 deviceCfg.ini -----------
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.inipath);
            return temp.ToString();
        }

        //-----------检查文件 deviceCfg.ini 是否存在-----------
        public bool test()
        {
            return File.Exists(inipath);
        }
    }
}
