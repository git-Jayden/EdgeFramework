using System;
using UnityEngine;

namespace Hotfix
{
   public class TestMono
    {
        public static void RunTest(GameObject go)
        {
            go.AddComponent<MonoTest>();
        }
        public static void RunTest1(GameObject go)
        {
            go.AddComponent<MonoTest>();
            MonoTest mono = go.GetComponent<MonoTest>();
            mono.Test();
        }
    }
    public class MonoTest : MonoBehaviour
    {
        private float mCurTime = 0;
        void Awake()
        {
            Debug.Log("MonoTest Awake");
        }
        void Start()
        {
            Debug.Log("MonoTest Start!");
        }
        void Update()
        {
            if (mCurTime < 0.2f)
            {
                mCurTime += Time.deltaTime;
                Debug.Log("MonoTest Update!");
            }
        }
        public  void Test()
        {
            Debug.Log("MonoTest");
        }
    }
}
