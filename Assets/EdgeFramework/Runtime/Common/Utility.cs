/****************************************************
	文件：Utility.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:52   	
	Features：
*****************************************************/
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace EdgeFramework
{
    public static class Utility
    {
       
        public class TimerHelper
        {
            /// <summary>
            /// 将秒换算称 时分秒
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static string MathfTime(float time)
            {
                string value = "";
                TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(time));
                if (ts.Hours > 0)
                {
                    value = ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds + "秒";
                }
                if (ts.Hours == 0 && ts.Minutes > 0)
                {
                    value = ts.Minutes.ToString() + "分钟" + ts.Seconds + "秒";
                }
                if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds > 0)
                {
                    value = ts.Seconds + "秒";
                }
                return value;
            }
            /// <summary>
            /// 将秒换算称 00:00
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static string TimeConvert(double time)
            {
                string value = "";
                TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(time));
                string h = "";
                if (ts.Hours.ToString().ToCharArray().Length <= 1)
                    h += "0";
                h += ts.Hours.ToString();
                string m = "";
                if (ts.Minutes.ToString().ToCharArray().Length <= 1)
                    m += "0";
                m += ts.Minutes.ToString();

                value = h + ":" + m;
                return value;
            }
        }

        /// <summary>
        /// 字符相关的实用函数。
        /// </summary>
        public class TextHelper
        {
            [ThreadStatic]
            private static StringBuilder s_CachedStringBuilder;

            private static void CheckCachedStringBuilder()
            {
                if (Utility.TextHelper.s_CachedStringBuilder == null)
                {
                    Utility.TextHelper.s_CachedStringBuilder = new StringBuilder(1024);
                }
            }
            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0)
            {
                if (format == null)
                {
                    throw new EdgeFrameworkException("Format is invalid.");
                }
                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.AppendFormat(format, arg0);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }
            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <param name="arg1">字符串参数 1。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0, object arg1)
            {
                if (format == null)
                {
                    throw new EdgeFrameworkException("Format is invalid.");
                }
                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.AppendFormat(format, arg0, arg1);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }
            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <param name="arg1">字符串参数 1。</param>
            /// <param name="arg2">字符串参数 2。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0, object arg1, object arg2)
            {
                if (format == null)
                {
                    throw new EdgeFrameworkException("Format is invalid.");
                }
                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }
            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="args">字符串参数。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, params object[] args)
            {
                if (format == null)
                {
                    throw new EdgeFrameworkException("Format is invalid.");
                }
                if (args == null)
                {
                    throw new EdgeFrameworkException("Args is invalid.");
                }
                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.AppendFormat(format, args);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }

            public static string Concat(string param1, string param2)
            {
                if (param1 == null)
                {
                    throw new EdgeFrameworkException("param1 is invalid.");
                }
                if (param2 == null)
                {
                    throw new EdgeFrameworkException("param2 is invalid.");
                }
                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.Append(param1);
                Utility.TextHelper.s_CachedStringBuilder.Append(param2);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }
            public static string Concat(string param1, string param2, string param3)
            {
                if (param1 == null)
                {
                    throw new EdgeFrameworkException("param1 is invalid.");
                }
                if (param2 == null)
                {
                    throw new EdgeFrameworkException("param2 is invalid.");
                }
                if (param3 == null)
                {
                    throw new EdgeFrameworkException("param3 is invalid.");
                }
                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.Append(param1);
                Utility.TextHelper.s_CachedStringBuilder.Append(param2);
                Utility.TextHelper.s_CachedStringBuilder.Append(param3);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }

            public static string PathConcat(string path1, string path2)
            {
                if (path1 == null)
                {
                    throw new EdgeFrameworkException("path1 is invalid.");
                }
                if (path2 == null)
                {
                    throw new EdgeFrameworkException("path2 is invalid.");
                }

                Utility.TextHelper.CheckCachedStringBuilder();
                Utility.TextHelper.s_CachedStringBuilder.Length = 0;
                Utility.TextHelper.s_CachedStringBuilder.Append(path1);
                Utility.TextHelper.s_CachedStringBuilder.Append("/");
                Utility.TextHelper.s_CachedStringBuilder.Append(path2);
                return Utility.TextHelper.s_CachedStringBuilder.ToString();
            }
            /// <summary>
            /// 数字转大写
            /// </summary>
            /// <param name="Num">数字</param>
            /// <returns></returns>
            public static string NumtoChinese(decimal s)
            {
                s = Math.Round(s, 2);//四舍五入到两位小数，即分
                string[] n = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
                //数字转大写
                string[] d = { "", "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };
                //不同位置的数字要加单位
                List<string> needReplace = new List<string> { "零拾", "零佰", "零仟", "零万", "零亿", "亿万", "零元", "零零", "零角", "零分" };
                List<string> afterReplace = new List<string> { "零", "零", "零", "万", "亿", "亿", "元", "零", "", "" };//特殊情况用replace剔除
                string e = s % 1 == 0 ? "整" : "";//金额是整数要加一个“整”结尾
                string re = "";
                Int64 a = (Int64)(s * 100);
                int k = 1;
                while (a != 0)
                {//初步转换为大写+单位
                    re = n[a % 10] + d[k] + re;
                    a = a / 10;
                    k = k < 11 ? k + 1 : 4;
                }
                string need = needReplace.Where(tb => re.Contains(tb)).FirstOrDefault<string>();
                while (need != null)
                {
                    int i = needReplace.IndexOf(need);
                    re = re.Replace(needReplace[i], afterReplace[i]);
                    need = needReplace.Where(tb => re.Contains(tb)).FirstOrDefault<string>();
                }//循环排除特殊情况
                re = re == "" ? "" : re + e;
                return re;
            }

        }

        public class ProtobufHelper
        {
            /// <summary>
            /// 序列化pb数据
            /// </summary>
            public static byte[] NSerialize<T>(T t)
            {
                byte[] buffer = null;
                using (MemoryStream m = new MemoryStream())
                {
                    Serializer.Serialize<T>(m, t);
                    m.Position = 0;
                    int length = (int)m.Length;
                    buffer = new byte[length];
                    m.Read(buffer, 0, length);
                }
                return buffer;
            }

            /// <summary>
            /// 反序列化pb数据
            /// </summary>
            public static T NDeserialize<T>(byte[] buffer)
            {
                T t = default(T);
                using (MemoryStream m = new MemoryStream(buffer))
                {
                    t = Serializer.Deserialize<T>(m);
                }
                return t;
            }

            /// <summary>
            /// 反序列化pb数据
            /// </summary>
            public static object NDeserialize(Type type, byte[] buffer, int index)
            {
                using (MemoryStream m = new MemoryStream(buffer, index, buffer.Length - index))
                {
                    return Serializer.Deserialize(type, m);
                }
            }
        }

        public class FileHelper
        {
            /// <summary>
            /// 获取文件的MD5值
            /// </summary>
            /// <param name="filePath">文件绝对路径</param>
            /// <returns></returns>
            public static string GetMD5HashFromFile(string filePath)
            {
                try
                {
                    var file = new FileStream(filePath, FileMode.Open);
                    var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    var retVal = md5.ComputeHash(file);
                    file.Close();
                    var sb = new StringBuilder();
                    for (var i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }

                catch (System.Exception e)
                {
                    throw new System.Exception("GetMD5HashFromFile failed! err = " + e.Message);
                }
            }

   

            /// <summary>
            /// 将字符串写入到路径下的文件中
            /// </summary>
            public static bool WriteAllText(string outFile, string outText)
            {
                try
                {
                    
                    if (!outFile.CheckFileAndCreateDirWhenNeeded()) return false;
                    if (File.Exists(outFile)) File.SetAttributes(outFile, FileAttributes.Normal);
                    File.WriteAllText(outFile, outText);
                    return true;
                }
                catch (System.Exception e)
                {
                    throw new EdgeFrameworkException(("WriteAllText failed! path = {0} with err = {1}", outFile) + e.Message);
                }
            }

            /// <summary>
            /// 将bytes写入到路径下的文件中
            /// </summary>
            public static bool WriteAllBytes(string outFile, byte[] outBytes)
            {
                try
                {
                    if (!outFile.CheckFileAndCreateDirWhenNeeded()) return false;
                    if (File.Exists(outFile)) File.SetAttributes(outFile, FileAttributes.Normal);
                    File.WriteAllBytes(outFile, outBytes);
                    return true;
                }
                catch (System.Exception e)
                {
                    throw new EdgeFrameworkException(("WriteAllBytes failed! path = {0} with err = {1}", outFile) + e.Message);
                }
            }

            /// <summary>
            /// 读取文件中所有字符串
            /// </summary>
            public static string ReadAllText(string inFile)
            {
                try
                {
                    if (string.IsNullOrEmpty(inFile)) return null;
                    if (!File.Exists(inFile)) return null;
                    File.SetAttributes(inFile, FileAttributes.Normal);
                    return File.ReadAllText(inFile);
                }
                catch (System.Exception e)
                {
                    throw new EdgeFrameworkException(("ReadAllText failed! path = {0} with err = {1}", inFile) + e.Message);
                }
            }

            /// <summary>
            /// 读取文件中所有bytes
            /// </summary>
            public static byte[] ReadAllBytes(string inFile)
            {
                try
                {
                    if (string.IsNullOrEmpty(inFile)) return null;
                    if (!File.Exists(inFile)) return null;
                    File.SetAttributes(inFile, FileAttributes.Normal);
                    return File.ReadAllBytes(inFile);
                }
                catch (System.Exception e)
                {
                    throw new EdgeFrameworkException(("ReadAllBytes failed! path = {0} with err = {1}", inFile) + e.Message);
                }
            }



            /// <summary>
            /// 获取倒数第N级目录
            /// </summary>
            /// <param name="dir"></param>
            /// <param name="floor"></param>
            /// <returns></returns>
            public static string GetParentDir(string dir, int floor = 1)
            {
                string subDir = dir;

                for (int i = 0; i < floor; ++i)
                {
                    int last = subDir.LastIndexOf('/');
                    subDir = subDir.Substring(0, last);
                }

                return subDir;
            }

            /// <summary>
            /// 根据文件名字获取文件夹中的文件 包括所有子目录
            /// </summary>
            /// <param name="dirName">目录</param>
            /// <param name="fileName">文件名</param>
            /// <param name="outResult">返回文件的FullName</param>
            public static void GetFileInFolder(string dirName, string fileName, List<string> outResult)
            {
                if (outResult == null)
                {
                    return;
                }

                DirectoryInfo dir = new DirectoryInfo(dirName);

                if (null != dir.Parent && dir.Attributes.ToString().IndexOf("System") > -1)
                {
                    return;
                }

                FileInfo[] finfo = dir.GetFiles();
                string fname = string.Empty;
                for (int i = 0; i < finfo.Length; i++)
                {
                    fname = finfo[i].Name;

                    if (fname == fileName)
                    {
                        outResult.Add(finfo[i].FullName);
                    }
                }

                DirectoryInfo[] dinfo = dir.GetDirectories();
                for (int i = 0; i < dinfo.Length; i++)
                {
                    GetFileInFolder(dinfo[i].FullName, fileName, outResult);
                }
            }

            /// <summary>
            /// 将scrPath文件夹下文件Copy到targetPath文件夹中去
            /// </summary>
            /// <param name="scrPath"></param>
            /// <param name="targetPath"></param>
            public  static void CopyFileTo(string scrPath, string targetPath)
            {
                try
                {
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                    string scrdir = Path.Combine(targetPath, Path.GetFileName(scrPath));
                    if (Directory.Exists(scrPath))
                        scrdir += Path.DirectorySeparatorChar;
                    if (!Directory.Exists(scrdir))
                        Directory.CreateDirectory(scrdir);
                    string[] files = Directory.GetFileSystemEntries(scrPath);
                    foreach (var file in files)
                    {
                        if (Directory.Exists(file))
                            CopyFileTo(file, scrdir);
                        else
                        {
                            File.Copy(file, scrdir + Path.GetFileName(file), true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("无法复制:" + scrPath + "  到" + targetPath + e);
                }
            }
            /// <summary>
            /// 移除文件夹下所有文件
            /// </summary>
            /// <param name="scrPath"></param>
            public  static void DeleteDir(string scrPath)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(scrPath);
                    FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
                    foreach (FileSystemInfo info in fileInfo)
                    {
                        if (info is DirectoryInfo)
                        {
                            DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                            subdir.Delete(true);
                        }
                        else
                        {
                            File.Delete(info.FullName);
                        }
                    }
                }
                catch (Exception e)
                {

                    Debug.LogError(e);
                }
            }
        }

        public class PathHelper
        {
            /// <summary>
            /// 根据路径获取文件名
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static string GetFileName(string path)
            {
                string[] fs = path.Split('/');
                string file = fs[fs.Length - 1];
                string name = file.Split('.')[0];
                return name;
            }
            /// <summary>
            /// 根据路径获取后缀名
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static string GetFileSuffix(string path)
            {
                return path.Split('.')[1];
            }
        }

        public class SerializeHelper
        {
            /// <summary>
            /// 类转换成二进制
            /// </summary>
            /// <param name="path"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static bool SerializeBinary(string path, object obj)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new EdgeFrameworkException("SerializeBinary Without Valid Path.");

                }

                if (obj == null)
                {
                    throw new EdgeFrameworkException("SerializeBinary obj is Null.");
                }

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bf.Serialize(fs, obj);
                    return true;
                }
            }
            /// <summary>
            /// 读取二进制
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public static object DeserializeBinary(Stream stream)
            {
                if (stream == null)
                {

                    throw new EdgeFrameworkException("DeserializeBinary Failed!");
                }

                using (stream)
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    var data = bf.Deserialize(stream);

                    // TODO:这里没风险嘛?
                    return data;
                }
            }

            public static object DeserializeBinary(string path)
            {
                if (string.IsNullOrEmpty(path))
                {


                    throw new EdgeFrameworkException("DeserializeBinary Without Valid Path.");
                }

                FileInfo fileInfo = new FileInfo(path);

                if (!fileInfo.Exists)
                {

                    throw new EdgeFrameworkException("DeserializeBinary File Not Exit.");
                }

                using (FileStream fs = fileInfo.OpenRead())
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    object data = bf.Deserialize(fs);

                    if (data != null)
                    {
                        return data;
                    }
                }


                throw new EdgeFrameworkException("DeserializeBinary Failed:" + path);
            }
            /// <summary>
            /// 类序列化成xml
            /// </summary>
            /// <param name="path"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static bool SerializeXML(string path, object obj)
            {
                if (string.IsNullOrEmpty(path))
                {

                    throw new EdgeFrameworkException("SerializeBinary Without Valid Path.");
                }

                if (obj == null)
                {

                    throw new EdgeFrameworkException("SerializeBinary obj is Null.");
                }

                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    var xmlserializer = new XmlSerializer(obj.GetType());
                    xmlserializer.Serialize(fs, obj);
                    return true;
                }
            }
            /// <summary>
            /// Xml的反序列化
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="path"></param>
            /// <returns></returns>
            public static object DeserializeXML<T>(string path)
            {
                if (string.IsNullOrEmpty(path))
                {

                    throw new EdgeFrameworkException("DeserializeBinary Without Valid Path.");
                }

                FileInfo fileInfo = new FileInfo(path);

                using (FileStream fs = fileInfo.OpenRead())
                {
                    XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                    object data = xmlserializer.Deserialize(fs);

                    if (data != null)
                    {
                        return data;
                    }
                }



                throw new EdgeFrameworkException("DeserializeBinary Failed:" + path);
            }
            /// <summary>
            /// Xml的反序列化
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="path"></param>
            /// <returns></returns>
            public static object DeserializeXML(string path, System.Type type)
            {
                if (string.IsNullOrEmpty(path))
                {

                    throw new EdgeFrameworkException("DeserializeBinary Without Valid Path.");
                }

                FileInfo fileInfo = new FileInfo(path);

                using (FileStream fs = fileInfo.OpenRead())
                {
                    XmlSerializer xmlserializer = new XmlSerializer(type);
                    object data = xmlserializer.Deserialize(fs);

                    if (data != null)
                    {
                        return data;
                    }
                }


                throw new EdgeFrameworkException("DeserializeBinary Failed:" + path);
            }

            public static string ToJson<T>(T obj) where T : class
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }

            public static T FromJson<T>(string json) where T : class
            {
                return JsonConvert.DeserializeObject<T>(json);
            }

            public static string SaveJson<T>(T obj, string path) where T : class
            {
                var jsonContent = ToJson(obj);
                File.WriteAllText(path, jsonContent);
                return jsonContent;
            }

            public static T LoadJson<T>(string path) where T : class
            {
                return FromJson<T>(File.ReadAllText(path));
            }

            public static byte[] ToProtoBuff<T>(T obj) where T : class
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize<T>(ms, obj);
                    return ms.ToArray();
                }
            }
            public static T FromProtoBuff<T>(byte[] bytes) where T : class
            {
                if (bytes == null || bytes.Length == 0)
                {
                    throw new System.ArgumentNullException("bytes");
                }
                T t = ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(bytes));
                return t;
            }

            public static void SaveProtoBuff<T>(T obj, string path) where T : class
            {
                File.WriteAllBytes(path, ToProtoBuff<T>(obj));
            }

            public static T LoadProtoBuff<T>(string path) where T : class
            {
                return FromProtoBuff<T>(File.ReadAllBytes(path));
            }
        }

        public class TransformHelper
        {
            /// <summary>
            /// 查找父物体下所有子物体
            /// </summary>
            /// <param name="goParent"></param>
            /// <param name="childName"></param>
            /// <returns></returns>
            public static Transform FindTheChild(GameObject goParent, string childName)
            {
                Transform searchTrans = goParent.transform.Find(childName);
                if (searchTrans == null)
                {
                    foreach (Transform trans in goParent.transform)
                    {
                        searchTrans = FindTheChild(trans.gameObject, childName);
                        if (searchTrans != null)
                        {
                            return searchTrans;
                        }
                    }
                }
                return searchTrans;
            }
            /// <summary>
            /// 获取父物体下所有激活的子物体
            /// </summary>
            /// <param name="goParent"></param>
            /// <returns></returns>
            public static List<Transform> GetChildList(GameObject goParent)
            {
                List<Transform> trans = new List<Transform>();
                foreach (Transform tran in goParent.transform)
                {
                    if (tran.gameObject.activeSelf)
                    {
                        if (tran.gameObject.name != goParent.name)
                        {
                            trans.Add(tran);
                        }
                    }
                }
                if (trans.Count == 0)
                    return null;
                return trans;
            }
        }

        public class NumberHelper
        {
            //获取指定范围内随机数
            public static int GetRandomInt(int num1, int num2)
            {
                if (num1 < num2)
                {
                    return UnityEngine.Random.Range(num1, num2);
                }
                else
                {
                    return UnityEngine.Random.Range(num2, num1);
                }
            }
            /// <summary>
            /// 概率百分比
            /// </summary>
            /// <param name="percent"> 0 ~ 100 </param>
            /// <returns></returns>
            public static bool PercentProbability(int percent)
            {
                return UnityEngine.Random.Range(0, 1000) * 0.001f < 50 * 0.01f;
            }
        }

        /// <summary>
        /// 程序集工具
        /// </summary>
        public class AssemblyUtil
        {
            private  void Example()
            {
                // var selfType = ReflectionExtension.GetAssemblyCSharp().GetType("QFramework.ReflectionExtension");
                // selfType.LogInfo();
            }

            /// <summary>
            /// 获取 Assembly-CSharp 程序集
            /// </summary>
            public static Assembly DefaultCSharpAssembly
            {
                get
                {
                    return AppDomain.CurrentDomain.GetAssemblies()
                        .SingleOrDefault(a => a.GetName().Name == "Assembly-CSharp");
                }
            }
            public static Assembly GetAssemblyCSharp()
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in assemblies)
                {
                    if (a.FullName.StartsWith("Assembly-CSharp,"))
                    {
                        return a;
                    }
                }

                //            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
                return null;
            }
            public static Assembly GetAssemblyCSharpEditor()
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in assemblies)
                {
                    if (a.FullName.StartsWith("Assembly-CSharp-Editor,"))
                    {
                        return a;
                    }
                }

                //            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp-Editor.dll");
                return null;
            }

            /// <summary>
            /// 获取默认的程序集中的类型
            /// </summary>
            /// <param name="typeName"></param>
            /// <returns></returns>
            public static Type GetDefaultAssemblyType(string typeName)
            {
                return DefaultCSharpAssembly.GetType(typeName);
            }
        }

    }
}