using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EdgeFramework
{
    public static class Log
    {
        private static LogLevel mLogLevel;

        public static LogLevel Level
        {
            get
            {
                return Log.mLogLevel;
            }
            set
            {
                Log.mLogLevel = value;
            }
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        public static bool Visible
        {
            get
            {
                return DebugerGUI.visible;
            }
        }

        static Log()
        {
            Log.mLogLevel = LogLevel.Normal;
        }

        public static void E(Exception e)
        {
            if (Log.mLogLevel >= LogLevel.Exception)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 打印错误信息 不需打印tag值 需在Unity编辑器添加宏命令EnableLog
        /// </summary>
        /// <param name="msg">需要打印的信息</param>
        /// <param name="args">Format参数</param>
        public static void E(object msg, params object[] args)
        {
            if (Log.mLogLevel >= LogLevel.Error)
            {
                if ((args == null ? true : args.Length == 0))
                {
                    Debug.LogError(msg);
                }
                else
                {
                    Debuger.LogError(msg.ToString(), args);
                }
            }
        }

        /// <summary>
        /// 打印错误信息+tag值 需在Unity编辑器添加宏命令EnableLog
        /// </summary>
        /// <param name="obj">需要打印的信息</param>
        /// <param name="msg">this 打印脚本tag值</param>
        /// <param name="args">Format参数</param>
        public static void E(object obj, object msg, params object[] args)
        {
            if (Log.mLogLevel >= LogLevel.Error)
            {
                if ((args == null ? true : args.Length == 0))
                {
                    Debuger.LogError(msg.ToString(), Log.GetLogTag(obj));
                }
                else
                {
                    Debuger.LogError(msg.ToString(), Log.GetLogTag(obj), args);
                }
            }
        }

        /// <summary>
        /// 获取调用打印的类名称或者标记有NAME的字段 
        /// 有NAME字段的，触发类名称用NAME字段对应的赋值
        /// 没有用类的名称代替
        /// </summary>
        /// <param name="obj">触发Log对应的类</param>
        /// <returns></returns>
        private static string GetLogTag(object obj)
        {
            string result;
            FieldInfo field = obj.GetType().GetField("NAME");
            result = (field == null ? obj.GetType().Name : (string)field.GetValue(obj));
            return result;
        }

        /// <summary>
        /// 打印信息 不需打印tag值 需在Unity编辑器添加宏命令EnableLog
        /// </summary>
        /// <param name="msg">需要打印的信息</param>
        /// <param name="args">Format参数</param>
        public static void I(object msg, params object[] args)
        {
            if (Log.mLogLevel >= LogLevel.Normal)
            {
                if ((args == null ? true : args.Length == 0))
                {
                    Debuger.Log(msg.ToString(), (string)null);
                }
                else
                {
                    Debuger.Log(msg.ToString(), args);
                }
            }
        }

        /// <summary>
        /// 打印信息+tag值 需在Unity编辑器添加宏命令EnableLog
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void I(object obj, object msg, params object[] args)
        {
            if (Log.mLogLevel >= LogLevel.Normal)
            {
                if ((args == null ? true : args.Length == 0))
                {
                    Debuger.Log(msg.ToString(), Log.GetLogTag(obj));
                }
                else
                {
                    Debuger.Log(msg.ToString(), Log.GetLogTag(obj), args);
                }
            }
        }

        public static void LogError(this object selfMsg)
        {
            Log.E(selfMsg, new object[0]);
        }

        public static void LogException(this Exception selfExp)
        {
            Log.E(selfExp);
        }

        public static void LogInfo(this object selfMsg)
        {
            Log.I(selfMsg, new object[0]);
        }

        public static void LogWarning(this object selfMsg)
        {
            Log.W(selfMsg, Array.Empty<object>());
        }

        /// <summary>
        ///  打印警告 不需打印tag值 需在Unity编辑器添加宏命令EnableLog
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void W(object msg, params object[] args)
        {
            if (Log.mLogLevel >= LogLevel.Warning)
            {
                if ((args == null ? true : args.Length == 0))
                {
                    Debuger.LogWarning(msg.ToString(), (string)null);
                }
                else
                {
                    Debuger.LogWarning(string.Format(msg.ToString(), Array.Empty<object>()), args);
                }
            }
        }

        /// <summary>
        /// 打印警告+tag值 需在Unity编辑器添加宏命令EnableLog
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void W(object obj, object msg, params object[] args)
        {
            if (Log.mLogLevel >= LogLevel.Warning)
            {
                if ((args == null ? true : args.Length == 0))
                {
                    Debuger.LogWarning(msg.ToString(), Log.GetLogTag(obj));
                }
                else
                {
                    Debuger.LogWarning(string.Format(msg.ToString(), Array.Empty<object>()), Log.GetLogTag(obj), args);
                }
            }
        }
    }
}