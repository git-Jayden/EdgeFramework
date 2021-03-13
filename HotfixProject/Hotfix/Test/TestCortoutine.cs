using System;
using UnityEngine;

namespace Hotfix
{
   public  class TestCortoutine
    {
        public static void RunTest()
        {
            GameRoot.Instance.StartCoroutine(Coroutine());
        }

        static System.Collections.IEnumerator Coroutine()
        {
            Debug.Log("开始协成,t=" + Time.time);
            yield return new WaitForSeconds(3);
            Debug.Log("开始完成,t=" + Time.time);

        }
    }
}
