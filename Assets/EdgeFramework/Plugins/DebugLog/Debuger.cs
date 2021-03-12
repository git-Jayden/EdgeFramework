/****************************************************
	文件：Debuger.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:22   	
	Features：
*****************************************************/
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace EdgeFramework.Debugger
{
    /// <summary>
    /// Unity 的 Debug 的封装类
    /// </summary>
    public class Debuger
    {
        /// <summary>
        /// 是否输出打印
        /// </summary>
        public static bool EnableLog;

        /// <summary>
        /// 是否显示打印时间
        /// </summary>
        public static bool EnableTime;

        /// <summary>
        /// 是否储存打印到文本
        /// </summary>
        public static bool EnableSave;

        /// <summary>
        /// 是否显示堆栈打印信息
        /// </summary>
        public static bool EnableStack;

        /// <summary>
        /// 打印文本保存文件夹路径
        /// </summary>
        public static string LogFileDir;

        /// <summary>
        /// 打印文本名称
        /// </summary>
        public static string LogFileName;

        /// <summary>
        /// 打印前缀
        /// </summary>
        public static string Prefix;

        /// <summary>
        /// 打印文本流
        /// </summary>
        public static StreamWriter LogFileWriter;

        /// <summary>
        /// 是否使用Unity打印
        /// </summary>
        public static bool UseUnityEngine;

        static Debuger()
        {
            Debuger.EnableLog = true;
            Debuger.EnableTime = true;
            Debuger.EnableSave = false;
            Debuger.EnableStack = false;
            Debuger.LogFileDir = "";
            Debuger.LogFileName = "";
            Debuger.Prefix = "-> ";
            Debuger.LogFileWriter = null;
            Debuger.UseUnityEngine = true;
        }

        public Debuger()
        {
        }

        /// <summary>
        /// 获取打印时间
        /// </summary>
        /// <param name="tag">触发打印信息对应的类或者NAME字段名称</param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetLogTime(string tag, string message)
        {
            string result = "";
            if (Debuger.EnableTime)
            {
                DateTime now = DateTime.Now;
                result = string.Concat(now.ToString("HH:mm:ss.fff"), " ");
            }
            return string.Concat(result, tag, "::", message);
        }

        /// <summary>
        /// 获取打印时间
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetLogTime(string message)
        {
            string result = "";
            if (Debuger.EnableTime)
            {
                DateTime now = DateTime.Now;
                result = string.Concat(now.ToString("HH:mm:ss.fff"), " ");
            }
            return string.Concat(result, "::", message);
        }

        /// <summary>
        /// 获取打印时间
        /// </summary>
        /// <returns></returns>
        private static string GetLogTime()
        {
            string result = "";
            if (Debuger.EnableTime)
            {
                DateTime now = DateTime.Now;
                result = string.Concat(now.ToString("HH:mm:ss.fff"), " ");
            }
            return result;
        }

        private static void Internal_Log(string msg, object context = null)
        {
            if (!Debuger.UseUnityEngine)
            {
                Console.WriteLine(msg);
            }
            else
            {
                UnityEngine.Debug.Log(msg, (UnityEngine.Object)context);
            }
        }

        private static void Internal_LogError(string msg, object context = null)
        {
            if (!Debuger.UseUnityEngine)
            {
                Console.WriteLine(msg);
            }
            else
            {

                UnityEngine.Debug.LogError(msg, (UnityEngine.Object)context);
            }
        }

        private static void Internal_LogWarning(string msg, object context = null)
        {
            if (!Debuger.UseUnityEngine)
            {
                Console.WriteLine(msg);
            }
            else
            {
                UnityEngine.Debug.LogWarning(msg, (UnityEngine.Object)context);
            }
        }

        /// <summary>
        /// Debug.Log 对应封装函数
        /// </summary>
        /// <param name="tag">触发函数对应的类</param>
        /// <param name="message">打印信息</param>
        [Conditional("EnableLog")]
        public static void Log(string message, string tag = null)
        {
            message = string.Concat("<color=#00ff00>", message, "</color>");
            if (Debuger.EnableLog)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = Debuger.GetLogTime(message);
                }
                else
                {
                    tag = string.Concat("<color=#800080ff>", tag, "</color>");
                    message = Debuger.GetLogTime(tag, message);
                }
                Debuger.Internal_Log(string.Concat(Debuger.Prefix, message), null);
                Debuger.LogToFile(string.Concat("[I]", message), false);
            }
        }

        /// <summary>
        /// Debug.Log 对应封装函数
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("EnableLog")]
        public static void Log(string format, params object[] args)
        {
            string message = string.Concat("<color=#00ff00>", string.Format(format, args), "</color>");
            if (Debuger.EnableLog)
            {
                string logText = Debuger.GetLogTime(message);
                Debuger.Internal_Log(string.Concat(Debuger.Prefix, logText), null);
                Debuger.LogToFile(string.Concat("[I]", logText), false);
            }
        }

        /// <summary>
        /// Debug.Log 对应封装函数
        /// </summary>
        /// <param name="tag">触发函数对应的类</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("EnableLog")]
        public static void Log(string format, string tag, params object[] args)
        {
            tag = string.Concat("<color=#800080ff>", tag, "</color>");
            string message = string.Concat("<color=#00ff00>", string.Format(format, args), "</color>");
            if (Debuger.EnableLog)
            {
                string logText = Debuger.GetLogTime(tag, message);
                Debuger.Internal_Log(string.Concat(Debuger.Prefix, logText), null);
                Debuger.LogToFile(string.Concat("[I]", logText), false);
            }
        }

        /// <summary>
        /// Debug.LogError 对应封装函数
        /// </summary>
        /// <param name="message">打印信息</param>
        /// <param name="context"></param>
        [Conditional("EnableLog")]
        public static void LogError(object message, object context)
        {
            message = string.Concat("<color=#ff0000ff>", message, "</color>");
            string str = string.Concat(Debuger.GetLogTime(), message);
            Debuger.Internal_LogError(string.Concat(Debuger.Prefix, str), context);
            Debuger.LogToFile(string.Concat("[E]", str), true);
        }

        /// <summary>
        /// Debug.LogError 对应封装函数
        /// </summary>
        /// <param name="tag">触发函数对应的类</param>
        /// <param name="message">打印信息</param>
        [Conditional("EnableLog")]
        public static void LogError(string message, string tag = null)
        {
            message = string.Concat("<color=#ff0000ff>", message, "</color>");
            if (string.IsNullOrEmpty(tag))
            {
                message = Debuger.GetLogTime(message);
            }
            else
            {
                tag = string.Concat("<color=#800080ff>", tag, "</color>");
                message = Debuger.GetLogTime(tag, message);
            }
            Debuger.Internal_LogError(string.Concat(Debuger.Prefix, message), null);
            Debuger.LogToFile(string.Concat("[E]", message), true);
        }

        /// <summary>
        /// Debug.LogError 对应封装函数
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("EnableLog")]
        public static void LogError(string format, params object[] args)
        {
            string.Concat("<color=#ff0000ff>", string.Format(format, args), "</color>");
            string logText = Debuger.GetLogTime(string.Format(format, args));
            Debuger.Internal_LogError(string.Concat(Debuger.Prefix, logText), null);
            Debuger.LogToFile(string.Concat("[E]", logText), true);
        }

        /// <summary>
        /// Debug.LogError 对应封装函数
        /// </summary>
        /// <param name="tag">触发函数对应的类</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("EnableLog")]
        public static void LogError(string format, string tag, params object[] args)
        {
            tag = string.Concat(tag, "<color=#800080ff>", tag, "</color>");
            string.Concat("<color=#ff0000ff>", string.Format(format, args), "</color>");
            string logText = Debuger.GetLogTime(tag, string.Format(format, args));
            Debuger.Internal_LogError(string.Concat(Debuger.Prefix, logText), null);
            Debuger.LogToFile(string.Concat("[E]", logText), true);
        }

        /// <summary>
        /// 序列化打印信息
        /// </summary>
        /// <param name="message">打印信息</param>
        /// <param name="EnableStack">是否开启堆栈打印</param>
        private static void LogToFile(string message, bool EnableStk = false)
        {
            if (Debuger.EnableSave)
            {
                if (Debuger.LogFileWriter == null)
                {
                    DateTime now = DateTime.Now;
                    Debuger.LogFileName = now.GetDateTimeFormats('s')[0].ToString();
                    Debuger.LogFileName = Debuger.LogFileName.Replace("-", "_");
                    Debuger.LogFileName = Debuger.LogFileName.Replace(":", "_");
                    Debuger.LogFileName = Debuger.LogFileName.Replace(" ", "");
                    Debuger.LogFileName = string.Concat(Debuger.LogFileName, ".log");
                    if (string.IsNullOrEmpty(Debuger.LogFileDir))
                    {
                        try
                        {
                            if (!Debuger.UseUnityEngine)
                            {
                                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                Debuger.LogFileDir = string.Concat(baseDirectory, "/DebugerLog/");
                            }
                            else
                            {
                                Debuger.LogFileDir = string.Concat(Application.persistentDataPath, "/DebugerLog/");
                            }
                        }
                        catch (Exception exception)
                        {
                            Exception ex = exception;
                            Debuger.Internal_LogError(string.Concat(Debuger.Prefix, "获取 Application.persistentDataPath 报错！", ex.Message), null);
                            return;
                        }
                    }
                    string path = string.Concat(Debuger.LogFileDir, Debuger.LogFileName);
                    try
                    {
                        if (!Directory.Exists(Debuger.LogFileDir))
                        {
                            Directory.CreateDirectory(Debuger.LogFileDir);
                        }
                        Debuger.LogFileWriter = File.AppendText(path);
                        Debuger.LogFileWriter.AutoFlush = true;
                    }
                    catch (Exception exception1)
                    {
                        Exception ex2 = exception1;
                        Debuger.LogFileWriter = null;
                        Debuger.Internal_LogError(string.Concat("LogToCache() ", ex2.Message, ex2.StackTrace), null);
                        return;
                    }
                }
                if (Debuger.LogFileWriter != null)
                {
                    try
                    {
                        Debuger.LogFileWriter.WriteLine(message);
                        if ((EnableStk || Debuger.EnableStack ? Debuger.UseUnityEngine : false))
                        {
                            Debuger.LogFileWriter.WriteLine(StackTraceUtility.ExtractStackTrace());
                        }
                    }
                    catch (Exception exception2)
                    {
                        Debuger.Log(exception2.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Debug.LogWarning 对应封装函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        [Conditional("EnableLog")]
        public static void LogWarning(object message, object context)
        {
            message = string.Concat("<color=#ffff00ff>", message, "</color>");
            string str = string.Concat(Debuger.GetLogTime(), message);
            Debuger.Internal_LogWarning(string.Concat(Debuger.Prefix, str), context);
            Debuger.LogToFile(string.Concat("[W]", str), false);
        }

        /// <summary>
        /// Debug.LogWarning 对应封装函数
        /// </summary>
        /// <param name="tag">触发函数对应的类</param>
        /// <param name="message">打印信息</param>
        [Conditional("EnableLog")]
        public static void LogWarning(string message, string tag = null)
        {
            message = string.Concat("<color=#ffff00ff>", message, "</color>");
            if (string.IsNullOrEmpty(tag))
            {
                message = Debuger.GetLogTime(message);
            }
            else
            {
                tag = string.Concat("<color=#800080ff>", tag, "</color>");
                message = Debuger.GetLogTime(tag, message);
            }
            Debuger.Internal_LogWarning(string.Concat(Debuger.Prefix, message), null);
            Debuger.LogToFile(string.Concat("[W]", message), false);
        }

        /// <summary>
        /// Debug.LogWarning 对应封装函数
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("EnableLog")]
        public static void LogWarning(string format, params object[] args)
        {
            string.Concat("<color=#ffff00ff>", string.Format(format, args), "</color>");
            string logText = Debuger.GetLogTime(string.Format(format, args));
            Debuger.Internal_LogWarning(string.Concat(Debuger.Prefix, logText), null);
            Debuger.LogToFile(string.Concat("[W]", logText), false);
        }

        /// <summary>
        /// Debug.LogWarning 对应封装函数
        /// </summary>
        /// <param name="tag">触发函数对应的类</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("EnableLog")]
        public static void LogWarning(string format, string tag, params object[] args)
        {
            tag = string.Concat(tag, "<color=#800080ff>", tag, "</color>");
            string.Concat("<color=#ffff00ff>", string.Format(format, args), "</color>");
            string logText = Debuger.GetLogTime(tag, string.Format(format, args));
            Debuger.Internal_LogWarning(string.Concat(Debuger.Prefix, logText), null);
            Debuger.LogToFile(string.Concat("[W]", logText), false);
        }
    }
}