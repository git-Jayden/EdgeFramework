/****************************************************
	文件：SimpleObjectPool.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 17:15   	
	Features：
*****************************************************/
using System;

namespace EdgeFramework
{
    /// <summary>
    /// Unity 游戏框架搭建 (十九) 简易对象池：http://qframework.io/post/24/ 的例子
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleObjectPool<T> : Pool<T>
    {
        readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null,int initCount = 0)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
            mResetMethod = resetMethod;

            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (mResetMethod != null)
            {
                mResetMethod.Invoke(obj);
            }
            
            mCacheStack.Push(obj);
            return true;
        }
    }
}