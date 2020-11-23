using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace EdgeFramework
{
    public class FileEx
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
        /// <summary>
        /// 将内容保存到指定路径
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="Path">路径</param>
        public static void SaveFile(string content, string Path)
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            File.WriteAllBytes(Path, Encoding.UTF8.GetBytes(content));
        }
    }
}