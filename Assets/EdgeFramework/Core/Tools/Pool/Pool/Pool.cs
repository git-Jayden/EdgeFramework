
using System.Collections.Generic;
namespace EdgeFramework
{
    public abstract class Pool<T> : IPool<T>, ICountObserveAble
    {
        #region ICountObserverable
        /// <summary>
        /// 获取当前对象数量
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }
        #endregion
        /// <summary>
        /// 创建对象的工厂
        /// </summary>
        protected IObjectFactory<T> mFactory;
        /// <summary>
        /// 所有对象缓存
        /// </summary>
        protected readonly Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// 默认最大数量
        /// </summary>
        protected int mMaxCount = 12;

        /// <summary>
        /// 分配获取对象 没有则创建新对象
        /// </summary>
        /// <returns></returns>
        public virtual T Allocate()
        {
            return mCacheStack.Count == 0
                ? mFactory.Create()
                : mCacheStack.Pop();
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool Recycle(T obj);
    }
}
