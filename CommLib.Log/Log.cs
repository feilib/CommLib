using System.Diagnostics;
using log4net;
using System.IO;

namespace CommLib.Log
{
    public class Log
    {
        /// <summary>
        /// Web环境下，由于config文件被放到了bin文件下，需要在Global.asax 的Application_Start中调用这个，初始化一下。。
        /// </summary>
        public static void WebInit()
        {
            string webLog4net = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin/Log4Net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(webLog4net));
        }
        /// <summary>
        /// 记录[DEBUG]日志  级别排序（从大到小）：FATAL（致命错误）、ERROR（一般错误）、WARN（警告）、INFO（一般信息）、DEBUG（调试 信息）
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void Debug( string logContent)
        {
            #region 使用stackTrace获取写日志的函数名
            StackTrace trace = new StackTrace();
            ILog log = LogManager.GetLogger(trace.GetFrame(1).GetMethod().DeclaringType);
            #endregion

            log.Debug(logContent);

        }

        /// <summary>
        /// 记录[DEBUG]日志  级别排序（从大到小）：FATAL（致命错误）、ERROR（一般错误）、WARN（警告）、INFO（一般信息）、DEBUG（调试 信息）
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void Error(string logContent)
        {
            #region 使用stackTrace获取写日志的函数名
            StackTrace trace = new StackTrace();
            ILog log = LogManager.GetLogger(trace.GetFrame(1).GetMethod().DeclaringType);
            #endregion

            log.Error(logContent);
        }

        /// <summary>
        /// 记录[FATAL]日志  级别排序（从大到小）：FATAL（致命错误）、ERROR（一般错误）、WARN（警告）、INFO（一般信息）、DEBUG（调试 信息）
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void Fatal(string logContent)
        {
            #region 使用stackTrace获取写日志的函数名
            StackTrace trace = new StackTrace();
            ILog log = LogManager.GetLogger(trace.GetFrame(1).GetMethod().DeclaringType);
            #endregion

            log.Fatal(logContent);

        }

        /// <summary>
        /// 记录[WARN]日志  级别排序（从大到小）：FATAL（致命错误）、ERROR（一般错误）、WARN（警告）、INFO（一般信息）、DEBUG（调试 信息）
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void Warn(string logContent)
        {
            #region 使用stackTrace获取写日志的函数名
            StackTrace trace = new StackTrace();
            ILog log = LogManager.GetLogger(trace.GetFrame(1).GetMethod().DeclaringType);
            #endregion


            log.Warn(logContent);
        }

        /// <summary>
        /// 记录[INFO]日志  级别排序（从大到小）：FATAL（致命错误）、ERROR（一般错误）、WARN（警告）、INFO（一般信息）、DEBUG（调试 信息）
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void Info(string logContent)
        {
            #region 使用stackTrace获取写日志的函数名
            StackTrace trace = new StackTrace();
            ILog log = LogManager.GetLogger(trace.GetFrame(1).GetMethod().DeclaringType);
            #endregion

            log.Info(logContent);

        }

        /// <summary>
        /// 记录报文日志
        /// </summary>
        /// <param name="logContent"></param>
        public static void InfoMsg(string logContent)
        {
 
            ILog log = LogManager.GetLogger("MESSAGE");
            log.Info(logContent);

        }

    }
}
