/****************************************************
	文件：ILRuntimeManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:30   	
	Features：  ILRUNTIME测试使用
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EdgeFramework;
using EdgeFramework.Res;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;




#region 测试代码

public abstract class TestClassBase
{
    public virtual int value
    {
        get { return 0; }
    }
    public virtual void TestVirtual(string str)
    {
        Debug.Log("TestClassBase TestVirtual str=" + str);
    }
    public abstract void TestAbstract(int a);
}
//跨域继承需要适配器的类
public class InheritanceAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType
    {
        get
        {
            //想继承的类
            return typeof(TestClassBase);
        }
    }


    public override System.Type AdaptorType
    {
        get
        {
            //实际的适配器类
            return typeof(Adapter);
        }
    }

    public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adapter(appdomain, instance);
    }
    class Adapter : TestClassBase, CrossBindingAdaptorType
    {
        private AppDomain mAppDomain;
        private ILTypeInstance mInstance;
        private IMethod mTestAbstract;
        private IMethod mTestVirtual;
        private IMethod mGetValue;
        private IMethod mTostring;
        object[] mParam = new object[1];
        private bool mTestVirtualInvoking = false;
        private bool mGetValueInvoking = false;
        public Adapter() { }
        public Adapter(AppDomain appdomain, ILTypeInstance instance)
        {
            mAppDomain = appdomain;
            mInstance = instance;
        }

        public ILTypeInstance ILInstance { get { return mInstance; } }

        //下面将所有虚函数都重载一遍，并中转到热更内
        //在适配器中重写所有需要在热更脚本重写的方法，并将控制权转移到脚本里去
        public override int value
        {
            get
            {
                if (mGetValue == null)
                    mGetValue = mInstance.Type.GetMethod("get_value", 1);
                if (mGetValue != null && !mGetValueInvoking)
                {
                    mGetValueInvoking = true;
                    int res = (int)mAppDomain.Invoke(mGetValue, mInstance, null);
                    mGetValueInvoking = false;
                    return res;
                }
                else
                    return base.value;
            }
        }
        public override void TestAbstract(int a)
        {
            if (mTestAbstract == null)
                mTestAbstract = mInstance.Type.GetMethod("TestAbstract", 1);
            if (mTestAbstract != null)
            {
                mParam[0] = a;
                mAppDomain.Invoke(mTestAbstract, mInstance, mParam);
            }
        }
        public override void TestVirtual(string str)
        {
            if (mTestVirtual == null)
                mTestVirtual = mInstance.Type.GetMethod("TestVirtual", 1);
            //必须设置一个标示位来表示当前是否在调用中,否则如果脚本类里调用了base.TestVirtual()就会造成无限循环
            if (mTestVirtual != null && !mTestVirtualInvoking)
            {
                mTestVirtualInvoking = true;
                mParam[0] = str;
                mAppDomain.Invoke(mTestVirtual, mInstance, mParam);
                mTestVirtualInvoking = false;
            }
            else
            {
                base.TestVirtual(str);
            }
        }
        public override string ToString()
        {
            if (mTostring == null)
                mTostring = mAppDomain.ObjectType.GetMethod("ToString", 0);

            IMethod m = mInstance.Type.GetVirtualMethod(mTostring);
            if (m == null || m is ILMethod)
            {
                return mInstance.ToString();
            }
            else
                return mInstance.Type.FullName;
        }
    }
}

public class CRLBindingTestClass
{
    public static float DoSomeTest(int a, int b)
    {
        return a + b;
    }
}

public delegate void TestDelegatMeth(int a);
public delegate string TestDelegatFunction(int a);
//协成适配器
public class CoroutineAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType { get { return null; } }

    public override System.Type[] BaseCLRTypes
    {
        get
        {
            //跨域继承只能有1个Adapter，因此应该尽量避免一个类同时实现多个外部接口，对于coroutine来说是IEnumerator<object>,IEnumerator和IDisposable，
            //ILRuntime虽然支持，但是一定要小心这种用法，使用不当很容易造成不可预期的问题
            //日常开发如果需要实现多个DLL外部接口，请在Unity这边先做一个基类实现那些个接口，然后继承那个基类
            return new System.Type[] { typeof(IEnumerator<object>), typeof(IEnumerator), typeof(System.IDisposable) };
        }
    }
    public override System.Type AdaptorType { get { return typeof(Adaptor); } }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }
    class Adaptor : IEnumerator<System.Object>, IEnumerator, System.IDisposable, CrossBindingAdaptorType
    {
        private AppDomain mAppDomain;
        private ILTypeInstance mInstance;

        private IMethod mTostring;

        public Adaptor() { }
        public Adaptor(AppDomain appdomain, ILTypeInstance instance)
        {
            mAppDomain = appdomain;
            mInstance = instance;
        }

        //下面将所有虚函数都重载一遍，并中转到热更内
        //在适配器中重写所有需要在热更脚本重写的方法，并将控制权转移到脚本里去
        public ILTypeInstance ILInstance { get { return mInstance; } }

        IMethod mCurrentMethod;
        bool mCurrentMethodGot;
        public object Current
        {
            get
            {
                if (!mCurrentMethodGot)
                {
                    mCurrentMethod = mInstance.Type.GetMethod("get_Current", 0);
                    if (mCurrentMethod == null)
                    {
                        //这里写System.Collections.IEnumerator.get_Current而不是直接get_Current是因为coroutine生成的类是显式实现这个接口的，通过Reflector等反编译软件可得知
                        //为了兼容其他只实现了单一Current属性的，所以上面先直接取了get_Current
                        mCurrentMethod = mInstance.Type.GetMethod("System.Collections.IEnumerator.get_Current", 0);
                    }
                    mCurrentMethodGot = true;
                }
                if (mCurrentMethod != null)
                {
                    var res = mAppDomain.Invoke(mCurrentMethod, mInstance, null);
                    return res;
                }
                else
                {
                    return null;
                }
            }
        }

        IMethod mDisposeMethod;
        bool mDisposeMethodGot;
        public void Dispose()
        {
            if (!mDisposeMethodGot)
            {
                mDisposeMethod = mInstance.Type.GetMethod("Dispose", 0);
                if (mDisposeMethod == null)
                {
                    mDisposeMethod = mInstance.Type.GetMethod("System.IDisposable.Dispose", 0);
                }
                mDisposeMethodGot = true;
            }

            if (mDisposeMethod != null)
            {
                mAppDomain.Invoke(mDisposeMethod, mInstance, null);
            }
        }


        IMethod mMoveNextMethod;
        bool mMoveNextMethodGot;
        public bool MoveNext()
        {
            if (!mMoveNextMethodGot)
            {
                mMoveNextMethod = mInstance.Type.GetMethod("MoveNext", 0);
                mMoveNextMethodGot = true;
            }

            if (mMoveNextMethod != null)
            {
                return (bool)mAppDomain.Invoke(mMoveNextMethod, mInstance, null);
            }
            else
            {
                return false;
            }
        }


        IMethod mResetMethod;
        bool mResetMethodGot;
        public void Reset()
        {
            if (!mResetMethodGot)
            {
                mResetMethod = mInstance.Type.GetMethod("Reset", 0);
                mResetMethodGot = true;
            }

            if (mResetMethod != null)
            {
                mAppDomain.Invoke(mResetMethod, mInstance, null);
            }
        }

        public override string ToString()
        {
            if (mTostring == null)
                mTostring = mAppDomain.ObjectType.GetMethod("ToString", 0);

            IMethod m = mInstance.Type.GetVirtualMethod(mTostring);
            if (m == null || m is ILMethod)
            {
                return mInstance.ToString();
            }
            else
                return mInstance.Type.FullName;
        }
    }

}

//MonoBehaviour适配器
public class MonoBehaviourAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType
    {
        get
        {
            return typeof(MonoBehaviour);
        }
    }

    public override System.Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }
    //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

        public AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

        IMethod mAwakeMethod;
        bool mAwakeMethodGot;
        public void Awake()
        {
            //Unity会在ILRuntime准备好这个实例前调用Awake，所以这里暂时先不掉用
            if (instance != null)
            {
                if (!mAwakeMethodGot)
                {
                    mAwakeMethod = instance.Type.GetMethod("Awake", 0);
                    mAwakeMethodGot = true;
                }

                if (mAwakeMethod != null)
                {
                    appdomain.Invoke(mAwakeMethod, instance, null);
                }
            }
        }
        IMethod mStartMethod;
        bool mStartMethodGot;
        void Start()
        {
            if (!mStartMethodGot)
            {
                mStartMethod = instance.Type.GetMethod("Start", 0);
                mStartMethodGot = true;
            }

            if (mStartMethod != null)
            {
                appdomain.Invoke(mStartMethod, instance, null);
            }
        }

        IMethod mUpdateMethod;
        bool mUpdateMethodGot;
        void Update()
        {
            if (!mUpdateMethodGot)
            {
                mUpdateMethod = instance.Type.GetMethod("Update", 0);
                mUpdateMethodGot = true;
            }

            if (mUpdateMethod != null)
            {
                appdomain.Invoke(mUpdateMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }


}
#endregion

public class ILRuntimeManager : Singleton<ILRuntimeManager>
{
    //测试委托跨域
    public TestDelegatMeth DelegateMethod;
    public TestDelegatFunction DelegateFunc;
    public System.Action<string> DelegateAction;

    ILRuntimeManager() { }
    private const string DLLPATH = "Assets/ABResources/Data/HotFix/Hotfix.dll.bytes";
    private const string PDBPATH = "Assets/ABResources/Data/HotFix/Hotfix.pdb.bytes";
    AppDomain mAppDomain;
    public AppDomain AppDomainCtrl { get { return mAppDomain; } }
    public void Init()
    {
        LoadHotFixAssembly();
    }
    MemoryStream fs;
    MemoryStream p;
    private void LoadHotFixAssembly()
    {
        //整个工程只有一个ILRuntime的AppDomain
        mAppDomain = new AppDomain();
        //读取热更资源dll
        TextAsset dllText = ResourcesManager.Instance.LoadResouce<TextAsset>(DLLPATH);
        //PBD文件,调试数据,日志报错
        TextAsset pdbText = ResourcesManager.Instance.LoadResouce<TextAsset>(PDBPATH);


        //using (MemoryStream fs = new MemoryStream(dllText.bytes))
        //{
        //    using (MemoryStream p = new MemoryStream(pdbText.bytes))
        //    {
        //        mAppDomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        //        InitializeILRuntime();
        //        OnHotFixLoaded();
        //    }
        //}
         fs = new MemoryStream(dllText.bytes);

         p = new MemoryStream(pdbText.bytes);

        mAppDomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        InitializeILRuntime();
        OnHotFixLoaded();
        //启动热更断点调试服务器  
        mAppDomain.DebugService.StartDebugService(56000);
    }
    public void CloseStream()
    {
        fs.Close();
        p.Close();
    }
    private void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        mAppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif

        //注册委托跨域的适配器
        //默认委托注册仅仅支持系统自带的Action以及Function 
        mAppDomain.DelegateManager.RegisterMethodDelegate<int>();
        mAppDomain.DelegateManager.RegisterFunctionDelegate<int, string>();
        mAppDomain.DelegateManager.RegisterMethodDelegate<string>();


        //注册ResourcesManager中的委托适配器
        mAppDomain.DelegateManager.RegisterMethodDelegate<System.String, UnityEngine.Object, System.Object, System.Object, System.Object>();


        //自定义委托或unity委托注册
        mAppDomain.DelegateManager.RegisterDelegateConvertor<TestDelegatMeth>((action =>
        {
            return new TestDelegatMeth((a) =>
            {
                ((System.Action<int>)action)(a);
            });
        }));
        mAppDomain.DelegateManager.RegisterDelegateConvertor<TestDelegatFunction>((func =>
        {
            return new TestDelegatFunction((a) =>
            {
                return ((System.Func<int, string>)func)(a);
            });
        }));
        mAppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((action =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((System.Action)action)();
            });
        }));

        //自定义ResourcesManager中的委托适配器
        mAppDomain.DelegateManager.RegisterDelegateConvertor<OnAsyncObjFinish>((action) =>
        {
            return new OnAsyncObjFinish((path, obj, param1, param2, param3) =>
            {
                ((System.Action<System.String, UnityEngine.Object, System.Object, System.Object, System.Object>)action)(path, obj, param1, param2, param3);
            });
        });
        //跨域继承的注册
        mAppDomain.RegisterCrossBindingAdaptor(new InheritanceAdapter());

        //注册协程适配器
        //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter，直接注册即可
        mAppDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        //注册Mono适配器
        mAppDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        //注册BaseUI适配器
        mAppDomain.RegisterCrossBindingAdaptor(new BaseUIAdapter());

        SetupCLRRedirection();
        SetupCLRRedirection2();

        //绑定注册(最后执行) 需要先
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(mAppDomain);

    }
    private void OnHotFixLoaded()
    {
        //第一个简单方法调用 调用无参数静态方法 mAppDomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        //mAppDomain.Invoke("Hotfix.TestClass", "StaticFunTest", null, null);
        //第二种调用 先单独获取类，之后一直使用这个类来调用
        IType type = mAppDomain.LoadedTypes["Hotfix.TestClass"];
        //根据方法名称和参数个数获取方法
        IMethod method = type.GetMethod("StaticFunTest", 0);
        mAppDomain.Invoke(method, null, null);
        //-------------------------------------------------------------------------------------
        //根据获取函数来调用有参的函数
        //第一种传参调用
        //IMethod method2 = type.GetMethod("StaticFunTest2",1);
        //mAppDomain.Invoke(method2, null, 10);
        //第二种传参调用
        IType intType = mAppDomain.GetType(typeof(int));
        List<IType> paraList = new List<IType>();
        paraList.Add(intType);
        IMethod method2 = type.GetMethod("StaticFunTest2", paraList, null);
        mAppDomain.Invoke(method2, null, 10);
        //----------------------------------------------------------------------------------------
        //实例化热更工程里的类
        //第一种实例化(可带参)
        object obj = mAppDomain.Instantiate("Hotfix.TestClass", new object[] { 15 });
        //第二种实例化(不带参)
        //object obj = ((ILType)type).Instantiate();
        //获取属性
        //int id = (int)mAppDomain.Invoke("Hotfix.TestClass", "get_ID", obj, null);//获取属性前面需要加get_
        //Debug.Log("ID" + id);
        object obj1 = ILRuntimeManager.Instance.AppDomainCtrl.Instantiate("Hotfix.LoadingPanelLogic");
        //-----------------------------------------------------------------------------------------
        //第一种泛型方法调用
        IType stringType = mAppDomain.GetType(typeof(string));
        IType[] genericArguments = new IType[] { stringType };
        //mAppDomain.InvokeGenericMethod("Hotfix.TestClass", "GenericMethod",genericArguments,null,"Jayden");
        //第二种泛型方法调用
        paraList.Clear();
        paraList.Add(stringType);
        IMethod genericMethod = type.GetMethod("GenericMethod", paraList, genericArguments);
        mAppDomain.Invoke(genericMethod, null, "Jayden22222222");

        //--------------------------------------------------------------------------------------
        //委托调用
        IType delegateType = mAppDomain.LoadedTypes["Hotfix.TestDelegate"];

        //IMethod delegateInit = delegateType.GetMethod("Initialize",0);
        //mAppDomain.Invoke(delegateInit,null);

        //IMethod delegateRun = delegateType.GetMethod("RunTest", 2);
        //mAppDomain.Invoke(delegateRun, null,10,"Jayden");

        //跨域委托调用  尽量使用系统自带的Action以及Function 
        IMethod DelegateInit = delegateType.GetMethod("Initialize2", 0);
        mAppDomain.Invoke(DelegateInit, null);

        //IMethod DelegateRun = delegateType.GetMethod("RunTest2", 2);
        //mAppDomain.Invoke(DelegateRun, null,10,"Jayden");
        // 也可以直接调用委托  委托在热更工程中注册
        DelegateMethod?.Invoke(10);
        string returnFunction = DelegateFunc?.Invoke(10);
        Debug.Log("ReturnA:" + returnFunction);
        DelegateAction?.Invoke("Jayden");
        //--------------------------------------------------------------------------------------
        //跨域继承 第一种
        //TestClassBase InheritanceObj = mAppDomain.Instantiate<TestClassBase>("Hotfix.TestInheritance");
        //InheritanceObj.TestAbstract(556);
        //InheritanceObj.TestVirtual("JadenVirtual");
        //跨域继承 第二种

        TestClassBase InheritanceObj = (TestClassBase)mAppDomain.Invoke("Hotfix.TestInheritance", "NewObject", null);
        InheritanceObj.TestAbstract(100);
        InheritanceObj.TestVirtual("JadenVirtual");

        //--------------------------------------------------------------------------------------
        //CLR绑定测试
        long curTime = System.DateTime.Now.Ticks;
        mAppDomain.Invoke("Hotfix.TestCLRBinding", "RunTest", null, null);
        Debug.Log("使用时间:" + (System.DateTime.Now.Ticks - curTime));


        //----------------------------------------------------------------------------------------
        //协成测试
        mAppDomain.Invoke("Hotfix.TestCortoutine", "RunTest", null, null);
        //----------------------------------------------------------------------------------------
        //Mono测试
        //  mAppDomain.Invoke("Hotfix.TestMono", "RunTest", null, GameRoot.Instance.gameObject);
        mAppDomain.Invoke("Hotfix.TestMono", "RunTest1", null, GameRoot.Instance.gameObject);

    }
    //AddComponent CLR重定向
    unsafe void SetupCLRRedirection()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
            {
                mAppDomain.RegisterCLRMethodRedirection(i, AddComponent);
            }
        }
    }
    //GetComponent CLR重定向
    unsafe void SetupCLRRedirection2()
    {
        //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
            {
                mAppDomain.RegisterCLRMethodRedirection(i, GetComponent);
            }
        }
    }

    unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = ILRuntime.Runtime.Stack.StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                //接下来创建Adapter实例
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                clrInstance.ILInstance = ilInstance;
                clrInstance.AppDomain = __domain;
                //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                ilInstance.CLRInstance = clrInstance;

                res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

                clrInstance.Awake();//因为Unity调用这个方法时还没准备好所以这里补调一次
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }

    unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        //CLR重定向的说明请看相关文档和教程，这里不多做解释
        AppDomain __domain = __intp.AppDomain;

        var ptr = __esp - 1;
        //成员方法的第一个参数为this
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
        if (instance == null)
            throw new System.NullReferenceException();
        __intp.Free(ptr);

        var genericArgument = __method.GenericArguments;
        //AddComponent应该有且只有1个泛型参数
        if (genericArgument != null && genericArgument.Length == 1)
        {
            var type = genericArgument[0];
            object res = null;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
                    {
                        if (clrInstance.ILInstance.Type == type)
                        {
                            res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
                            break;
                        }
                    }
                }
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }


}
