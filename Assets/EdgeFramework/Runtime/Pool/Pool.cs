
/****************************************************
	文件：Pool.cs
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
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Pool<T> : IPool<T>
    {
        #region ICountObserverable
        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }
        #endregion
        
        protected IObjectFactory<T> mFactory;

        /// <summary>
        /// 存储相关数据的栈
        /// </summary>
        protected readonly Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// default is 5
        /// </summary>
        protected int mMaxCount = 12;

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0
                ? mFactory.Create()
                : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }
}