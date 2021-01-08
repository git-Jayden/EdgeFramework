using System.Collections;
using UnityEngine;
namespace EdgeFramework
{
    public static  class IActionExtension 
    {
        /// <summary>
        /// 利用协程执行该节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selfBehaviour"></param>
        /// <param name="commandNode"></param>
        /// <returns></returns>
        public static T ExecuteNode<T>(this T selfBehaviour,IAction commandNode ) where T : MonoBehaviour
        {
            selfBehaviour.StartCoroutine(commandNode.Execute());
            return selfBehaviour;
        }
        /// <summary>
        /// 延时N秒执行函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selfBehaviour"></param>
        /// <param name="seconds">N秒</param>
        /// <param name="delayEvent">N秒后执行该函数</param>
        public static void Delay<T>(this T selfBehaviour, float seconds, System.Action delayEvent) where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayAction.Allocate(seconds,delayEvent));
        }

        public static IEnumerator Execute(this IAction selfNode)
        {

            if (selfNode.Finished) selfNode.Reset();
            while (!selfNode.Execute(Time.deltaTime))
            {
                yield return null;
            }

        }
    }
}