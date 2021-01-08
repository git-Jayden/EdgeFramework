
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    #region 事件接口

    public delegate void OnEvent(int key, params object[] param);

    #endregion

    public class LEventSystem : Singleton<LEventSystem>,IPoolable
    {
        /// <summary>
        /// 缓存所有的事件 
        /// </summary>
        private readonly Dictionary<int, ListenerWrap> mAllListenerMap = new Dictionary<int, ListenerWrap>();

        public bool IsRecycled { get; set; }
        private LEventSystem(){}

        #region 内部结构 事件链
        private class ListenerWrap
        {
            /// <summary>
            /// 缓存同一类型的事件
            /// </summary>
            private LinkedList<OnEvent> mEventList;
            public bool Fire(int key,params object[]param)
            {
                if (mEventList.IsNull()) return false;

                var next = mEventList.First;
                OnEvent call = null;
                LinkedListNode<OnEvent> nextCache = null;
                while (next!=null)
                {
                    call = next.Value;
                    nextCache = next.Next;
                    call(key,param);

                    next = next.Next ?? nextCache;
                }
                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="listener"></param>
            /// <returns></returns>
            public bool Add(OnEvent listener)
            {
                if (mEventList.IsNull())
                    mEventList = new LinkedList<OnEvent>();
                if (mEventList.Contains(listener))
                    return false;
                mEventList.AddLast(listener);
                return true;
            }
            public void Remove(OnEvent listener)
            {
                if (mEventList.IsNull()) return;
                if (mEventList.Contains(listener))
                    mEventList.Remove(listener);
            }
            public void RemoveAll()
            {
                if (mEventList.IsNull()) return;
                mEventList.Clear();
            }
        }
        #endregion
        #region 功能函数
        public bool Register<T>(T key, OnEvent fun) where T:IConvertible
        {
            var kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (!mAllListenerMap.TryGetValue(kv, out wrap))
            {
                wrap = new ListenerWrap();
                mAllListenerMap.Add(kv, wrap);
            }
            if (wrap.Add(fun))
                return true;

            Debug.LogWarning("Already Register Same Event:"+key);
            return false;
        }

        public void UnRegister<T>(T key, OnEvent fun)where T: IConvertible
        {
            var kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (mAllListenerMap.TryGetValue(kv, out wrap))
            {
                wrap.Remove(fun);
            }
        }
        public void UnRegister<T>(T key) where T : IConvertible
        {
            var kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (mAllListenerMap.TryGetValue(kv, out wrap))
            {
                wrap.RemoveAll();
                wrap = null;
                mAllListenerMap.Remove(kv);
            }
        }
        public bool Send<T>(T key, params object[] para)where T: IConvertible
        {
            int kv = key.ToInt32(null);
            ListenerWrap wrap;
            if (mAllListenerMap.TryGetValue(kv, out wrap))
            {
                return wrap.Fire(kv,wrap);
            }
            return false;
        }
        public void OnRecycled()
        {
            mAllListenerMap.Clear();
        }
        #endregion
        #region 高频率使用的API
        /// <summary>
        /// 执行事件链中事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool SendEvent<T>(T key, params object[] param) where T : IConvertible
        {
            return Instance.Send(key, param);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public static bool RegisterEvent<T>(T key, OnEvent fun) where T : IConvertible
        {
            return Instance.Register(key, fun);
        }
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fun"></param>
        public static void UnRegisterEvent<T>(T key, OnEvent fun) where T : IConvertible
        {
            Instance.UnRegister(key, fun);
        }

        #endregion
    }

}