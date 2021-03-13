using System;
using UnityEngine;

namespace Hotfix
{
    public class TestInheritance : TestClassBase
    {
        public static TestInheritance NewObject()
        {
            return new TestInheritance();
        }
        public override void TestAbstract(int a)
        {
            Debug.Log("TestInheritance TestAbstract a="+a);
        }
        public override void TestVirtual(string str)
        {
            base.TestVirtual(str);
            Debug.Log("TestInheritance TestVirtual str=" + str);
        }
     
    }
}
