using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CommLib.ShareFun
{
    /// <summary>
    /// 代码模板类，这里的代码不能直接调用，提供外部参考，有必要的话直接拷贝。。。
    /// </summary>
    public class CodeTemplate
    {
        /// <summary>
        /// 获取程序版本号的功能
        /// </summary>
        /// <returns></returns>
        /// 注：这里直接调用就拿到了CommLib.ShareFun的版本号了。这个代码要考走。。。
        public static string GetVerson()
        {
            string Verson = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Verson;
        }

    }
}
