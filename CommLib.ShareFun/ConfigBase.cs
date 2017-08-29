using System;
using System.Globalization;

namespace CommLib.ShareFun
{
    /// <summary>
    /// <para>配置文件基类</para>
    /// <para>所有的继承类，需要首先给Section赋值，以确保配置文件写在各自的区间内</para>
    /// <para>已经实现各种类型的读写信息，直接使用就好了</para>
    /// </summary>
    public class ConfigBase
    {
        /// <summary>
        /// 配置区间
        /// </summary>
        protected string Section;

        public ConfigBase()
        {
            Section = "General";
        }

        /// <summary>
        /// 必须重写--读配置信息
        /// </summary>
        public virtual void Write() { }
        /// <summary>
        /// 必须重写--写配置信息
        /// </summary>
        public virtual void Read() { }

        #region 读写的一些封装
        /// <summary>
        /// 写配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, string value)
        {
            return ConfigFile.WriteIniData(Section, key, value);
        }

        /// <summary>
        /// 写配置信息--bool
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, bool value)
        {
            return ConfigFile.WriteIniData(Section, key, value.ToString());
        }

        /// <summary>
        /// 写配置信息--int
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, int value)
        {
            return ConfigFile.WriteIniData(Section, key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 写配置信息--UInt64
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, UInt64 value)
        {
            return ConfigFile.WriteIniData(Section, key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 写配置信息--double
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, double value)
        {
            return ConfigFile.WriteIniData(Section, key, value.ToString(CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// 写配置信息--DateTime值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, DateTime value)
        {
            return ConfigFile.WriteIniData(Section, key, value.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        /// <summary>
        /// 写配置信息--int 数组
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, int[] value)
        {
            string ret = "";
            foreach (int i in value)
            {
                ret += i + ",";
            }
            ret = ret.TrimEnd(',');
            return Write(key, ret);
        }

        /// <summary>
        /// 写配置信息--double 数组
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, double[] value)
        {
            string ret = "";
            foreach (double i in value)
            {
                ret += i + ",";
            }
            ret = ret.TrimEnd(',');
            return Write(key, ret);
        }

        /// <summary>
        /// 写配置信息--string 数组[注意，数组保存是逗号分隔的，所以string数组中的字符串不能有半角逗号]
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Write(string key, string[] value)
        {
            string ret = "";
            foreach (string i in value)
            {
                ret += i + ",";
            }
            ret = ret.TrimEnd(',');
            return Write(key, ret);
        }

        /// <summary>
        /// 读配置信息--字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        protected string Read(string key, string noText)
        {
            return ConfigFile.ReadIniData(Section, key, noText);
        }

        /// <summary>
        /// 读配置信息--bool值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        protected bool Read(string key, bool noText)
        {
            string val = ConfigFile.ReadIniData(Section, key, noText.ToString());

            try
            {
                return bool.Parse(val);
            }
            catch (Exception)
            {
                return noText;
            }
        }

        /// <summary>
        /// 读配置信息--int值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        protected int Read(string key, int noText)
        {
            string val = ConfigFile.ReadIniData(Section, key, noText.ToString());

            try
            {
                return int.Parse(val);
            }
            catch (Exception)
            {
                return noText;
            }
        }

        /// <summary>
        /// 读配置信息--UInt64值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        protected UInt64 Read(string key, UInt64 noText)
        {
            string val = ConfigFile.ReadIniData(Section, key, noText.ToString());

            try
            {
                return UInt64.Parse(val);
            }
            catch (Exception)
            {
                return noText;
            }
        }

        /// <summary>
        /// 读配置信息--double值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        protected double Read(string key, double noText)
        {
            string val = ConfigFile.ReadIniData(Section, key, noText.ToString());

            try
            {
                return double.Parse(val);
            }
            catch (Exception)
            {
                return noText;
            }
        }
        /// <summary>
        /// 读配置信息--Datetime
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        protected DateTime Read(string key, DateTime noText)
        {
            string val = ConfigFile.ReadIniData(Section, key, noText.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            try
            {
                return DateTime.ParseExact(val, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return noText;
            }
        }

        /// <summary>
        /// int数组类型，用逗号分隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        public int[] Read(string key, int[] noText)
        {
            string value = Read(key, "");
            if (value == "")
            {
                return noText;
            }
            else
            {
                try
                {
                    string[] vs = value.Split(',', ';');
                    int[] ret = new int[vs.Length];
                    for (int i = 0; i < vs.Length; i++)
                    {
                        ret[i] = int.Parse(vs[i]);
                    }
                    return ret;
                }
                catch (Exception)
                {
                    return noText;
                }
            }
        }

        /// <summary>
        /// 数组类型，用逗号分隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        public double[] Read(string key, double[] noText)
        {
            string value = Read(key, "");
            if (value == "")
            {
                return noText;
            }
            else
            {
                try
                {
                    string[] vs = value.Split(',', ';');
                    double[] ret = new double[vs.Length];
                    for (int i = 0; i < vs.Length; i++)
                    {
                        ret[i] = double.Parse(vs[i]);
                    }
                    return ret;
                }
                catch (Exception)
                {
                    return noText;
                }
            }
        }

        /// <summary>
        /// 数组类型，用逗号分隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <returns></returns>
        public string[] Read(string key, string[] noText)
        {
            string value = Read(key, "");
            if (value == "")
            {
                return noText;
            }
            else
            {
                try
                {
                    string[] vs = value.Split(',', ';');
                    return vs;
                }
                catch (Exception)
                {
                    return noText;
                }
            }
        }
        #endregion
    }
}
