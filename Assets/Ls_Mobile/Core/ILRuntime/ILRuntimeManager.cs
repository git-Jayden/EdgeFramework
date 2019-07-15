using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;

namespace Ls_Mobile
{
    public class ILRuntimeManager : Singleton<ILRuntimeManager>
    {

        private const string DLLPATH = "Assets/Ls_Mobile/Example/Data/Hotfix/HotFix.dll.txt";
        private const string PDBPATH = "Assets/Ls_Mobile/Example/Data/Hotfix/HotFix.pdb.txt";

        ILRuntimeManager() { }
        ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        public ILRuntime.Runtime.Enviorment.AppDomain ILRunAppDomain
        {
            get { return appDomain; }
        }
        public void Init()
        {
            LoadHotFixAssembly();
        }
        void LoadHotFixAssembly()
        {
            //整个工程只有一个ILRuntime的AppDomain
            appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            //读取热更资源的dll
            TextAsset dllText = ResouceManager.Instance.LoadResouce<TextAsset>(DLLPATH);
            //PBD文件，调试数据可，日志报错
            TextAsset pdbText = ResouceManager.Instance.LoadResouce<TextAsset>(PDBPATH);
            using (MemoryStream ms = new MemoryStream(dllText.bytes))
            {
                using (MemoryStream p = new MemoryStream(pdbText.bytes))
                {
                    appDomain.LoadAssembly(ms, null, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
            }
            InitializeIlRuntime();
            OnHotFixLoaded();
        }
        void InitializeIlRuntime()
        {
            //默认委托注册仅仅支持系统自带的Action以及Function
            //appDomain.DelegateManager.RegisterMethodDelegate<bool>();
            //appDomain.DelegateManager.RegisterFunctionDelegate<int, string>();
            //appDomain.DelegateManager.RegisterMethodDelegate<int>();
            //appDomain.DelegateManager.RegisterMethodDelegate<string>();
            //appDomain.DelegateManager.RegisterMethodDelegate<System.String, UnityEngine.Object, System.Object, System.Object, System.Object>();
        }
        void OnHotFixLoaded()
        {
            //第一个简单方法的调用
            //appDomain.Invoke("HotFix.TestClass", "StaticFunTest", null, null);

            //先单独获取类，之后一直使用这个类来调用
           IType type =appDomain.LoadedTypes["HotFix.TestClass"];
            //根据方法名称和参数个数获取方法(学习获取函数进行调用)
           // IMethod method = type.GetMethod("StaticFunTest", 0);
           //appDomain.Invoke(method, null, null);

            //根据获取函数来调用有参的函数
            //第一种含参调用
            IMethod method = type.GetMethod("StaticFunTest2", 1);
            appDomain.Invoke(method, null, 5);

        }
    }
}