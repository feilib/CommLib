using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;

namespace CommLib.ShareFun
{
    public static class Funs
    {
        /// <summary>
        /// 十进制转换为bcd编码
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int IntToBcd(int a)
        {
            int ret = 0, shl = 0;
            while (a > 0)
            {
                ret |= (a % 10) << shl;
                a /= 10;
                shl += 4;
            }
            return ret;
        }

        /// <summary>
        /// 将BCD码专为int十进制
        /// </summary>
        /// <param name="body">byte数据</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="len">BCD长度（包含字节书）</param>
        /// <returns>返回转换完成的十进制结果</returns>
        public static int Bcd2Int(byte[] body, int offset, int len)
        {
            int ret = 0;
            for (int i = 0; i < len; i++)
            {
                int tmp = ((body[offset + len - 1 - i] >> 4) & 0x0F) * 10 + ((body[offset + len - 1 - i]) & 0x0F);
                ret = ret * 100 + tmp;
            }
            return ret;
        }

        /// <summary>
        /// 获取本机的IP地址，第一个IPV4地址，失败的话返回默认"192.168.0.1";
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                string hostName = Dns.GetHostName(); //得到主机名
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                foreach (IPAddress ip in ipEntry.AddressList)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "192.168.0.1";
            }
            catch (Exception)
            {
                //MessageBox.Show("获取本机IP出错:" + ex.Message);
                return "192.168.0.1";
            }
        }

        /// <summary>
        /// 去掉冠字号码识别错误的内容，用_替代 
        /// </summary>
        /// <param name="code">待处理的冠字号</param>
        /// <returns>处理后的冠字号</returns>
        public static string ReplaceBadCharOfRmbCode(string code)
        {
            byte[] str = Encoding.ASCII.GetBytes(code);
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] < '0' || str[i] > '9') && (str[i] < 'a' || str[i] > 'z') && (str[i] < 'A' || str[i] > 'Z'))
                {
                    str[i] = (byte)('_');
                }
            }
            return Encoding.ASCII.GetString(str);
        }

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>true表示正在使用,false没有使用</returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;

            FileStream fs = null;
            try
            {

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,

                FileShare.None);

                inUse = false;
            }
            catch
            {
                // ignored
            }
            finally
            {
                if (fs != null)

                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用
        }


        private static List<DateTime> _dtShowed = new List<DateTime>();

        /// <summary>
        /// 在文件名上用，获取一个不重复的时间字符串
        /// </summary>
        /// <param name="formatString">格式化字符串，务必输入正确</param>
        /// <returns></returns>
        public static string GetNoneRepeadTimeString(string formatString)
        {
            DateTime dtnow = DateTime.Now;

            lock (_dtShowed)
            {
                do
                {
                    bool hasRepeat = false;
                    foreach (DateTime dt in _dtShowed)
                    {
                        if (dtnow.ToString(formatString) == dt.ToString(formatString))
                        {
                            hasRepeat = true;
                        }
                    }
                    if (hasRepeat)
                    {
                        dtnow = dtnow.AddMilliseconds(50);
                    }
                    else
                    {
                        break;
                    }
                } while (true);


                _dtShowed.Add(dtnow);

                while (_dtShowed.Count > 30)
                {
                    _dtShowed.RemoveAt(0);
                }
            }

            return dtnow.ToString(formatString);
        }

        /// <summary>
        /// 获取随机的冠字号码：2位大写字母+8位数字
        /// </summary>
        /// <returns></returns>
        public static string GetRandRmbCode()
        {
            return GetRndString(2, false, false, true, false, "") + GetRndString(8, true, false, false, false, "");
        }

        ///<summary>
        ///生成随机字符串
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRndString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum) { str += "0123456789"; }
            if (useLow) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 根据传入的冠字号，生成一个图片内存流
        /// </summary>
        /// <param name="rmbCode"></param>
        /// <returns></returns>
        public static MemoryStream GenImgByRmbCode(string rmbCode)
        {
            string checkcode = rmbCode;
            Bitmap image = new Bitmap(272, 40);//可以控制显示验证码的区域 
            Graphics g = Graphics.FromImage(image);//创建Graphics对象
            g.Clear(Color.White);//清空图片背景色
            Font font = new Font("Arial", 20, (FontStyle.Bold));//指定字体大小和样式
            g.DrawString(checkcode, font, new SolidBrush(Color.Black), 25, 5);//绘制文本字符串
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            return ms;
        }

        /// <summary>
        /// 根据传入的冠字号，生成文件
        /// </summary>
        /// <param name="rmbCode"></param>
        /// <param name="saveName"></param>
        public static void GenerateImgByRmbCode(string rmbCode, string saveName)
        {
            if (!File.Exists(saveName))
            {
                MemoryStream ms = GenImgByRmbCode(rmbCode);
                FileStream fs = new FileStream(saveName, FileMode.OpenOrCreate);
                ms.WriteTo(fs);
                fs.Flush();
                fs.Close();
            }


        }

        /// <summary>
        /// 判断给定的服务名是否存在
        /// </summary>
        /// <param name="serviceName">服务名（显示名称）</param>
        /// <returns></returns>
        public static bool IsServiceExist(string serviceName)
        {
            ServiceController[] scs = ServiceController.GetServices();
            foreach (ServiceController sc in scs)
            {
                if (sc.ServiceName == serviceName)
                { return true;}
            }
            return false;
        }


        //常用正则表达式
        /*
        "^\d+$" //非负整数（正整数 + 0） 
        "^[0-9]*[1-9][0-9]*$" //正整数 
        "^((-\d+)|(0+))$" //非正整数（负整数 + 0） 
        "^-[0-9]*[1-9][0-9]*$" //负整数 
        "^-?\d+$" //整数 
        "^\d+(\.\d+)?$" //非负浮点数（正浮点数 + 0） 
        "^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$" //正浮点数 
        "^((-\d+(\.\d+)?)|(0+(\.0+)?))$" //非正浮点数（负浮点数 + 0） 
        "^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$" //负浮点数 
        "^(-?\d+)(\.\d+)?$" //浮点数 
        "^[A-Za-z]+$" //由26个英文字母组成的字符串 
        "^[A-Z]+$" //由26个英文字母的大写组成的字符串 
        "^[a-z]+$" //由26个英文字母的小写组成的字符串 
        "^[A-Za-z0-9]+$" //由数字和26个英文字母组成的字符串 
        "^\w+$" //由数字、26个英文字母或者下划线组成的字符串 
        "^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$" //email地址 
        "^[a-zA-z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$" //url 
        /^(d{2}|d{4})-((0([1-9]{1}))|(1[1|2]))-(([0-2]([1-9]{1}))|(3[0|1]))$/ // 年-月-日 
        /^((0([1-9]{1}))|(1[1|2]))/(([0-2]([1-9]{1}))|(3[0|1]))/(d{2}|d{4})$/ // 月/日/年 
        "^([w-.]+)@(([[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.)|(([w-]+.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(]?)$" //Emil 
        "(d+-)?(d{4}-?d{7}|d{3}-?d{8}|^d{7,8})(-d+)?" //电话号码 
        "^(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5])$" //IP地址 

        YYYY-MM-DD基本上把闰年和2月等的情况都考虑进去了 
        ^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$
        */


        /// <summary>
        /// 判断输入的字符串是否仅为字母
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool CheckStringOnlyHaveLetter(string val)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(val, @"^[A-Za-z]+$");
        }

        /// <summary>
        /// 判断输入的字符串是否仅为数字
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool CheckStringOnlyHaveNumber(string val)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(val, @"^-?\d+$");
        }

        /// <summary>
        /// 判断输入的字符串是否为数字和字母的组合
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool CheckStringOnlyHaveLetterAndNumber(string val)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(val, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 根据传入文件名（全路径），返回可以使用的文件名，如果有重名，则自动加入_1、_2之类的后缀，直至没有重名
        /// </summary>
        /// <param name="fileName">待检查文件名</param>
        /// <returns></returns>
        public static string GetAvailableFileName(string fileName)
        {
            string retName = fileName;
            string extend = Path.GetExtension(fileName);
            int suffixNo = 0;
            while (File.Exists(fileName))
            {
                string oldSuffix = string.Format("_{0}.{1}", suffixNo, extend);
                suffixNo++;
                string newSuffix = string.Format("_{0}.{1}", suffixNo, extend);
                if (fileName.Contains(oldSuffix))
                {
                    retName = fileName.Replace(oldSuffix, newSuffix);
                }
                else
                {
                    retName = fileName.Replace("." + extend, newSuffix);
                }

            }

            return retName;
        }
    }
}
