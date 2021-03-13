using System;
using UnityEngine;

namespace Hotfix
{
   public  class TestDelegate
    {
        static TestDelegatMeth delegateMethod;
        static TestDelegatFunction delegateFunc;
        static Action<string> delegateAction;

        public static void Initialize()
        {
            delegateMethod = Method;
            delegateFunc = Function;
            delegateAction = Action;
        }
        public static void RunTest(int a,string str)
        {
            delegateMethod?.Invoke(a);
           string returnFunction=delegateFunc?.Invoke(a);
            Debug.Log("ReturnA:"+ returnFunction);
            delegateAction?.Invoke(str);
        }
         static void Method(int a)
        {
            Debug.Log("TestDelegate Method a=" + a);
        }
        static string Function(int a)
        {
            Debug.Log("TestDelegat Function  a=" + a);
            return a.ToString();
        }
        static void Action(string str)
        {
            Debug.Log("TestDelegate Action str=" + str);
        }
        //委托跨域
        public static void Initialize2()
        {
           ILRuntimeManager.Instance.DelegateMethod = Method;
            ILRuntimeManager.Instance.DelegateFunc = Function;
            ILRuntimeManager.Instance.DelegateAction = Action;
        }
        public static void RunTest2(int a, string str)
        {
            ILRuntimeManager.Instance.DelegateMethod?.Invoke(a);
            string returnFunction = ILRuntimeManager.Instance.DelegateFunc?.Invoke(a);
            Debug.Log("ReturnA:" + returnFunction);
            ILRuntimeManager.Instance.DelegateAction?.Invoke(str);
        }
    }
}
