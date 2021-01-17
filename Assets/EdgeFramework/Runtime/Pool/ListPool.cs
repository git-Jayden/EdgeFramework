/****************************************************
	文件：ListPool.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 17:14   	
	Features：
*****************************************************/

using System.Collections.Generic;

namespace EdgeFramework
{
    /// <summary>
    /// 链表对象池：存储相关对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ListPool<T>
    {
        /// <summary>
        /// 栈对象：存储多个List
        /// </summary>
        static Stack<List<T>> mListStack = new Stack<List<T>>(8);

        /// <summary>
        /// 出栈：获取某个List对象
        /// </summary>
        /// <returns></returns>
        public static List<T> Get()
        {
            if (mListStack.Count == 0)
            {
                return new List<T>(8);
            }

            return mListStack.Pop();
        }

        /// <summary>
        /// 入栈：将List对象添加到栈中
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(List<T> toRelease)
        {
            toRelease.Clear();
            mListStack.Push(toRelease);
        }
    }

    /// <summary>
    /// 链表对象池 拓展方法类
    /// </summary>
    public static class ListPoolExtensions
    {
        /// <summary>
        /// 给List拓展 自身入栈 的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toRelease"></param>
        public static void Release2Pool<T>(this List<T> toRelease)
        {
            ListPool<T>.Release(toRelease);
        }
    }
}