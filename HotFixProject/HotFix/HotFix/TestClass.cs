using System;
using UnityEngine;

namespace HotFix
{
    public class TestClass
    {
        public static void StaticFunTest()
        {
            Debug.Log("TestClass StaticFunTest!!!!!!");
        }
        public static void StaticFunTest2(int a)
        {
            Debug.Log("TestClass StaticFunTest2 a="+a);
        }
    }
}
