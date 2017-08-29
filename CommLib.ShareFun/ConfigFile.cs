using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CommLib.ShareFun
{
    /// <summary>
    /// 读写INI文件的类
    /// </summary>
    public class ConfigFile
    {
        #region dll 引用
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        #endregion

        private static object _wrLock = new object();

        #region 读Ini文件

        public static string ReadIniData(string section, string key, string noText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                lock (_wrLock)
                {
                    GetPrivateProfileString(section, key, noText, temp, 1024, iniFilePath);
                }
                return temp.ToString();
            }
            else
            {
                return noText;
            }

        }
        public static string ReadIniData(string section, string key, string noText)
        {
            string iniFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config.ini");
            return ReadIniData(section, key, noText, iniFilePath);
        }

        #endregion

        #region 写Ini文件

        public static bool WriteIniData(string section, string key, string value, string iniFilePath)
        {
            lock (_wrLock)
            {
                if (!File.Exists(iniFilePath))
                {
                    File.Create(iniFilePath).Close();
                }
                else
                {
                    long opStation = WritePrivateProfileString(section, key, value, iniFilePath);
                    switch (opStation)
                    {
                        case 0:
                            return false;
                        default:
                            return true;
                    }
                }
            }

            return false;
        }
        public static bool WriteIniData(string section, string key, string value)
        {
            string iniFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config.ini");
            return WriteIniData(section, key, value, iniFilePath);
        }
        #endregion
    }
}
