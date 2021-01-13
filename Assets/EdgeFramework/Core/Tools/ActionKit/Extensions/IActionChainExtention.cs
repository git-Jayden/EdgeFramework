using System;
using UnityEngine;

namespace EdgeFramework
{
    public static partial class IActionChainExtention
    {
        /// <summary>
        /// 创建一个重复执行的节点链并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selfbehaviour"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IActionChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count) { Executer = selfbehaviour };

            retNodeChain.Dispose();
            //retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }
        /// <summary>
        /// 创建一个节点链并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selfbehaviour"></param>
        /// <returns></returns>
        public static IActionChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain { Executer = selfbehaviour };
            retNodeChain.Dispose();
            //retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain OnlyBegin(this IActionChain selfChain, Action<OnlyBeginAction> onBegin)
        {
            return selfChain.Append(OnlyBeginAction.Allocate(onBegin));
        }
        /// <summary>
        /// 将延时节点添加到节点链中
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IActionChain Delay(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        /// <summary>
        /// Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IActionChain Wait(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }
        /// <summary>
        /// 将事件节点添加到节点链中
        /// </summary>
        /// <param name="selfChain"></param>
        /// <param name="onEvents"></param>
        /// <returns></returns>
        public static IActionChain Event(this IActionChain selfChain, params System.Action[] onEvents)
        {
            return selfChain.Append(EventAction.Allocate(onEvents));
        }


        public static IActionChain Until(this IActionChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilAction.Allocate(condition));
        }
    }
}