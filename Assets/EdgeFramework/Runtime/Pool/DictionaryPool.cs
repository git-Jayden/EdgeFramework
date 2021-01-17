
/****************************************************
	文件：DictionaryPool.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 17:13   	
	Features：
*****************************************************/
using System.Collections.Generic;

namespace EdgeFramework
{
    /// <summary>
    /// 字典对象池：用于存储相关对象
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryPool<TKey, TValue>
    {
        /// <summary>
        /// 栈对象：存储多个字典
        /// </summary>
        static Stack<Dictionary<TKey, TValue>> mListStack = new Stack<Dictionary<TKey, TValue>>(8);

        /// <summary>
        /// 出栈：从栈中获取某个字典数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Get()
        {
            if (mListStack.Count == 0)
            {
                return new Dictionary<TKey, TValue>(8);
            }

            return mListStack.Pop();
        }

        /// <summary>
        /// 入栈：将字典数据存储到栈中 
        /// </summary>
        /// <param name="toRelease"></param>
        public static void Release(Dictionary<TKey, TValue> toRelease)
        {
            toRelease.Clear();
            mListStack.Push(toRelease);
        }
    }
    
    /// <summary>
    /// 对象池字典 拓展方法类
    /// </summary>
    public static class DictionaryPoolExtensions
    {
        /// <summary>
        /// 对字典拓展 自身入栈 的方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="toRelease"></param>
        public static void Release2Pool<TKey,TValue>(this Dictionary<TKey, TValue> toRelease)
        {
            DictionaryPool<TKey,TValue>.Release(toRelease);
        }
    }
}