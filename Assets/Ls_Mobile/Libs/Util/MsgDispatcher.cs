
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ls_Mobile.Util
{
   public  class MsgDispatcher: MonoBehaviour
    {
       public  static Dictionary<string, Action<object>> mRegisteredMsgs = new Dictionary<string, Action<object>>();
        /// <summary>
        /// 注册事件到事件链
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public static void Register(string msgName, Action<object> onMsgReceived)
        {
            if (!mRegisteredMsgs.ContainsKey(msgName))
                mRegisteredMsgs.Add(msgName, _ => { });
            mRegisteredMsgs[msgName] += onMsgReceived;
        }
        /// <summary>
        /// 从事件链中移除整个事件
        /// </summary>
        /// <param name="msgName"></param>
        public static void UnRegisterAll(string msgName)
        {
            mRegisteredMsgs.Remove(msgName);
        }
        /// <summary>
        /// 从事件链中移除指定事件
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public static void UnRegister(string msgName, Action<object> onMsgReceived)
        {
            if (mRegisteredMsgs.ContainsKey(msgName))
            {
                mRegisteredMsgs[msgName] -= onMsgReceived;
            }
        }
        /// <summary>
        /// 执行事件链中的指定事件
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="data"></param>
        public static void Send(string msgName, object data)
        {
            if (mRegisteredMsgs.ContainsKey(msgName))
            {
                mRegisteredMsgs[msgName](data);
            }
        }

    }
}
