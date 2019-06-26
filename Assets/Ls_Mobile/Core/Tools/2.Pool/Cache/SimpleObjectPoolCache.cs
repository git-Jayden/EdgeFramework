using System;
using System.Collections.Generic;
namespace Ls_Mobile
{
    public class SimpleObjectCache
    {
        /// <summary>
        /// 存储多个类型的对象
        /// </summary>
        private readonly Dictionary<Type, object> mObjectPools;
        public SimpleObjectCache()
        {
            mObjectPools = new Dictionary<Type, object>();
        }
        /// <summary>
        /// 根据对象类型取得对象的对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SimpleObjectPool<T> GetObjectPool<T>()where T:new ()
        {
            object objectPool;
            var type = typeof(T);
            if (!mObjectPools.TryGetValue(type, out objectPool))
            {
                objectPool = new SimpleObjectPool<T>(() => new T());
                mObjectPools.Add(type, objectPool);
            }
            return ((SimpleObjectPool<T>) objectPool);
        }
        /// <summary>
        /// 从对象池中取出一个相对类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : new()
        {
            return GetObjectPool<T>().Allocate();
        }
        /// <summary>
        /// 回收相对类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Push<T>(T obj) where T : new()
        {
            GetObjectPool<T>().Recycle(obj);
        }
        /// <summary>
        /// 添加一个对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="simpleObjectPool"></param>
        public void RegisterCustomObjectPool<T>(SimpleObjectPool<T> simpleObjectPool)
        {
            mObjectPools.Add(typeof(T), simpleObjectPool);
        }
        /// <summary>
        /// 清理所有的对象池
        /// </summary>
        public void Reset()
        {
            mObjectPools.Clear();
        }
    }

}