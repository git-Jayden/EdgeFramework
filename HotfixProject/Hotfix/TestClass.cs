using System;
using UnityEngine;

namespace Hotfix
{
    public class TestClass
    {
        private int mId;
        public int ID { get { return mId; } }
        public TestClass() { }
        public TestClass(int id)
        {
            mId = id;
        }

        public static void StaticFunTest()
        {
            Debug.Log("TestClass StaticFunTest!!!");
        }
        public static void StaticFunTest2(int a)
        {
            Debug.Log("TestClass StaticFunTest!!!" + a);
        }
        public static void GenericMethod<T>(T a)
        {
            Debug.Log("TestClass GenericMethod a="+a);
        }
    }
}
