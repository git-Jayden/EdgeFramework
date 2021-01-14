using System;

namespace EdgeFramework
{
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        /// <summary>
        /// 实例化一个简单对象池
        /// </summary>
        /// <param name="factoryMethod">创建对象工厂时候的Func方法函数并返回一个对象存入对象池中</param>
        /// <param name="resetMethod">回收对象时候的方法函数Action</param>
        /// <param name="initCount">初始化多少个对象</param>
        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;
            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">存储对象池中的对象</param>
        /// <returns></returns>
        public override bool Recycle(T obj)
        {
            mResetMethod?.Invoke(obj);
            mCacheStack.Push(obj);
            return true;
        }
    }
}
