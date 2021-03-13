# EdgeFramework使用教程

### 一、AssetsBundle打包
#####  1.AppConfig配置
点击EdgeFramework->AssetsBundle->AppConfig或者快捷键F6调出AssetBundle的配置文件。

如下图，AllPrefabPath为配置需要打AB包的Prefab的路径，只需要填写至文件夹的路径即可，打包的AB包名即为Prefab的名字，AllFileDirAb下即填写需要打包的资源的文件夹路径以及填写需要打包生成的AB包名
***注:如Prefab包中包含了某些资源文件则该资源文件可不必打包入资源中，例如UIPrefab中包含了某几张图片，则这些图片不必再打AB包，因为打包会将prefab所有的依赖文件都打包入包内***
![image.png](https://upload-images.jianshu.io/upload_images/3912830-dab2288ae9c41b88.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

###### 2.BuildBundle  打包说明
点击EdgeFramework->AssetsBundle->BuildBundle或者快捷键F8即可启动打包

打包所生成文件

***(1)AssetbundleCofig.xml文件***

该xml只用来查看打包信息
该文件在Assets根目录下，打开后如下图ABList代表打包的资源，Path代表该打包资源的原资源路径，Crc为该资源路径的唯一ID,ABName代表该资源打入了这个Ab的包名中,AssetName代表资源的名字

另ABDependce代表依赖项，意思就是加载ABList中资源还依赖于该ABDependce Ab包中的资源
![image.png](https://upload-images.jianshu.io/upload_images/3912830-632352a4dd868a7e.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

***注:如遇资源加载报错打开该xml根据报错的Crc看资源路径，xml中如果没有该crc代表资源没打入包中或者资源加载的路径填写错误***

***（2）AssetbundleConfig.bytes文件***
 该bytes内容与上面的XML一致，只不过这个文件是用来加载使用，该文件所在目录Assets/ABResources/Data/Config/ABData/

***（3）ABMD5.bytes本地md5校验文件***
资源所在目录Assets\Resources下，该文件是用来校验本地本地资源的解压，在程序开始运行的时候会将StreamAssets下的AssetsBundle解压入Application.persistentDataPath持久化数据存储目录中，这时候就需要Md5值校验文件

![image](https://user-images.githubusercontent.com/24520716/110446473-2bcd8280-80fa-11eb-8de1-829561f19269.png)

***（4）AssetsBundle包***
在工程根目录下AssetBundle\平台\目录下
![image.png](https://upload-images.jianshu.io/upload_images/3912830-8228d99b9ef909a3.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

***（5）ABMD5.bytes服务器md5校验文件***
 在工程根目录下Version\平台\ABMD5_0.1.bytes   0.1为App的版本 打热更包的时候会选择app版本号中的md5做对比，会对比出与最初打包出来Ab包中的资源文件哪些文件做了更改

注:打包APP或者需要使用AB方式加载的时候，必须将工程根目录下AssetBundle\平台\目录下的ab包复制到StreamAssets/AssetBundle目录下，复制AB包快捷按钮点击EdgeFramework->AssetsBundle->CopyBundleToStreamAssets也可以使用快捷按钮F10。
EdgeFramework->AssetsBundle->DeleteStreamAssets删除StreamAssets下的AB包

另:AppConfig.cs脚本中可设置，例如是否使用AB进行资源加载，是否检查版本更新，配置服务器资源下载的地址等等
![image.png](https://upload-images.jianshu.io/upload_images/3912830-064d2df1d8eb473d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

### 二、AssetsBundle资源加载
***1.AssetsBundle加载配置***
需要资源加载需将AppConfig.cs脚本中的UseAssetBundle设置为true,并将工程根目录下AssetBundle\平台\下的Ab包拷贝到StreamAssets\AssetBundle\目录下这时候可以使用EdgeFramework->AssetsBundle->CopyBundleToStreamAssets可直接将Ab包拷贝入Stream目录下，如需移除streamAssets下Ab包也可点击EdgeFramework->AssetsBundle->DeleteStreamAssets下自动移除Ab包，建议在Editor下使用编辑器加载。需打Apk的时候使用Ab加载，只需要设置UseAssetBundle变量即可

***2.AssetsBundle代码加载***

（1）资源同步加载ResourcesManager.LoadResouce(string path)，具体其他函数可自行查看ResourcesManager,包括预加载资源，异步加载资源，资源卸载，取消异步加载资源等

（2）Prefab同步加载ObjectManager.InstantiateObject(string path)，具体其他函数可自行查看ObjectManager,包括预加载Gamobject，异步加载，回收资源，取消异步加载资源等

### 三、打热更包以及热更配置文件的配置
***1.配置资源热更***

打开AppConfig.cs脚本，将CheckVersionUpdate检查更新设置为true,并且设置好ServerResourceURL资源的url路径

***2.打热更包***

（1）点击EdgeFramework->AssetsBundle->打热更包，如下图，选择当前app版本所打出来AssetsBundle包生成出来的ABMD5.bytes文件，路径在EdgeFramework\Version\平台\版本 下，下面热更补丁版本为热更的版本，代表第几次热更的版本，意思将当前的资源文件与最初的资源文件的md5做对比，如当前资源与之前的资源文件Md5不一致代表该资源需更新重新打一份ab包出来
![image](https://user-images.githubusercontent.com/24520716/110447136-e198d100-80fa-11eb-9e6c-95525f69b957.png)

点击打热更包后，会生成差异的ab文件以及一份热更配置文件出来，目录在工程根节点下/Hot/平台/下，资源目录如下图

![image.png](https://upload-images.jianshu.io/upload_images/3912830-494c6a3006a27781.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

其中有一份配置文件Patch.xml，可打开看到，Name代表要更新的Ab包的包名，url代表更新下载的url,后面是平台，md5和资源的大小
![image.png](https://upload-images.jianshu.io/upload_images/3912830-6b9d201cc6cfa4c2.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
***3.配置Serverinfo.xml服务器配置文件***

将工程根节点下/Hot/平台/下的AB包拷贝，然后可在资源服务器下创建文件夹，文件夹路径为   资源服务器/AssetBundle/App版本号/热更版本号/  然后将之前所拷贝出来的热更ab包放入该目录下

随后在资源服务器的根目录下创建Serverinfo.xml,文件内容如下
![image.png](https://upload-images.jianshu.io/upload_images/3912830-51270792779bd62f.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
GameVersion  Version=为app版本，下面Path为之前打热更包生成出来的Pathces，将拷贝过来，将xmlns移除，并可添加Des更新描述，如上图，下面可添加多个Pathces，但是APP只会对最后一个Pathces进行更新检查，只需要更改服务器端的Pathces即可进行版本的回退以及对版本的更新

服务器路径节点为下图

![image.png](https://upload-images.jianshu.io/upload_images/3912830-bfcb35b5bcd755f5.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

完成上方操作后运行服务器，并运行APP后可看到已可以更新资源

![image](https://user-images.githubusercontent.com/24520716/110559064-83153680-817e-11eb-81fb-88232b87820d.png)

![image](https://user-images.githubusercontent.com/24520716/110559894-fd928600-817f-11eb-9a46-0d32dc3bbc9d.png)
###  四、ILRuntime代码热更
热更工程路径在Frame\EdgeFramework\HotfixProject\Hotfix下，UI的逻辑层代码在热更工程的UIPanel文件夹下，Test文件夹下是热更的测试代码
![image.png](https://upload-images.jianshu.io/upload_images/3912830-585b2ead2ce77a6d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
完成热更代码后点击生成会将dll以及pdb生成在unity工程路径下EdgeFramework\Assets\ABResources\Data\HotFix下，然后在unity中点击EdgeFramework->ILRuntime->修改热更dll为bytes会将刚刚生成的dll转化成bytes以便于ILRuntime加载dll。
![image.png](https://upload-images.jianshu.io/upload_images/3912830-5686a0a25c3014b0.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
ILRuntime加载热更工程的dll代码在ILRuntimeManager.cs中
![image.png](https://upload-images.jianshu.io/upload_images/3912830-0b2168e33d6d9239.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
ILRuntimeManager中还有ILRuntime使用测试的代码，包括简单方法调用带参与不带参的、实例化热更工程里的类带参与不带参的、泛型方法的调用、委托的调用、跨域委托调用、跨域委托调用、跨域继承、CLR绑定、热更工程使用协程、热更工程使用Monbehavior。
其中只要跨域的都需要注册适配器
跨域委托调用  尽量使用系统自带的Action以及Function ,使用系统自带的Action以及Function则只需要注册适配器，如果需要使用自定义的委托则还需要自定义委托的适配器，如下
***跨域委托***
```
Unity工程
public delegate void TestDelegatMeth(int a);
public delegate string TestDelegatFunction(int a);
public class ILRuntimeManager : Singleton<ILRuntimeManager>
{
    //测试委托跨域
    public TestDelegatMeth DelegateMethod;
    public TestDelegatFunction DelegateFunc;
    public System.Action<string> DelegateAction;

//系统自带的委托需要注册适配器，自定义的委托还需要自定义委托适配器
private void InitializeILRuntime()
{
        //注册委托跨域的适配器
        //默认委托注册仅仅支持系统自带的Action以及Function 
        mAppDomain.DelegateManager.RegisterMethodDelegate<int>();
        mAppDomain.DelegateManager.RegisterFunctionDelegate<int, string>();

        mAppDomain.DelegateManager.RegisterMethodDelegate<string>();

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
}
//调用热更函数
    private void OnHotFixLoaded()
    {
  //跨域委托调用  尽量使用系统自带的Action以及Function 
        IMethod DelegateInit = delegateType.GetMethod("Initialize2", 0);
        mAppDomain.Invoke(DelegateInit, null);

        //IMethod DelegateRun = delegateType.GetMethod("RunTest2", 2);
        //mAppDomain.Invoke(DelegateRun, null,10,"Jayden");
}
}
热更工程
namespace Hotfix
{
   public  class TestDelegate
    {
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
```
***跨域继承***需要写跨域适配器，然后注册

```
U3D工程
//父类
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
    private void InitializeILRuntime()
    {
        //跨域继承的注册
        mAppDomain.RegisterCrossBindingAdaptor(new InheritanceAdapter());
}
    private void OnHotFixLoaded()
    {
        //跨域继承 第一种
        //TestClassBase InheritanceObj = mAppDomain.Instantiate<TestClassBase>("Hotfix.TestInheritance");
        //InheritanceObj.TestAbstract(556);
        //InheritanceObj.TestVirtual("JadenVirtual");
        //跨域继承 第二种

        TestClassBase InheritanceObj = (TestClassBase)mAppDomain.Invoke("Hotfix.TestInheritance", "NewObject", null);
        InheritanceObj.TestAbstract(100);
        InheritanceObj.TestVirtual("JadenVirtual");
}
热更换工程
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
```
***CLR绑定***
需要在GenerateCLRBindingByAnalysis.cs的函数InitILRuntime中注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用，
![image.png](https://upload-images.jianshu.io/upload_images/3912830-52ef91bf1cad3e91.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
完成后点击EdgeFramework->ILRuntime->通过自动分析热更dll生成CLR绑定
![image.png](https://upload-images.jianshu.io/upload_images/3912830-98e1838a08a2d70e.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
点击后会在Assets/Scripts/ILRuntime/Generated/文件夹中自动生成绑定的代码
![image.png](https://upload-images.jianshu.io/upload_images/3912830-6c74f26801c6bba8.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
这时候即可在ILRuntimeManager.cs中的InitializeILRuntime（）函数中注册绑定
```
  //绑定注册(最后执行) 需要先
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(mAppDomain);
```
最后测试可发现调用热更工程时间比不绑定快很多
***热更工程使用协程***
```
U3D工程
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

    private void InitializeILRuntime()
    {

        //注册协程适配器
        //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter，直接注册即可
        mAppDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
}
    private void OnHotFixLoaded()
    {
        //协成测试
        mAppDomain.Invoke("Hotfix.TestCortoutine", "RunTest", null, null);
}
热更工程
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
```
***热更工程使用MonoBehaviour***
```
U3D工程
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
    private void InitializeILRuntime()
    {
        //注册Mono适配器
        mAppDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
}
    private void OnHotFixLoaded()
    {
        //Mono测试
        //  mAppDomain.Invoke("Hotfix.TestMono", "RunTest", null, GameRoot.Instance.gameObject);
        mAppDomain.Invoke("Hotfix.TestMono", "RunTest1", null, GameRoot.Instance.gameObject);
}
热更工程
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

```
###  五、表格数据
Excels表格数据路径EdgeFramework\Excels\xlsx\，表格数据后缀必须是xlsx，excels表格数据的前四行用于结构定义, 其余为数据
```
第一行：'-' (需要序列化使用的数据)， 'ignore'(代表忽略该列)
第二行：布尔(bool) 整型(int) 浮点数(float) 字符串(string) 数组(array<基本类型>) 枚举(xxxEnum, 自定义名字+Enum后缀)
第三行：关键字(MoveSpeed, 首字母大写式驼峰命名规则)
第四行：注释
```
![image.png](https://upload-images.jianshu.io/upload_images/3912830-8e5756ae716466dc.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
编辑完成表格后必须关闭表格才能转表,然后可在脚本SheetEditor.cs中配置表格的数据,将表格数据加入字典中,key为表格的名字,value为SheetExportBase
![表格数据配置.png](https://upload-images.jianshu.io/upload_images/3912830-3002afbee4478214.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
SheetExportBase可以配置导出的数据类型
![image.png](https://upload-images.jianshu.io/upload_images/3912830-befcbcaa997bed27.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
上图中表格数据配置
1为表格名字，2为表格名字，3为表格导出字典时key的类型，4为都出字典时key在数据中的变量，5表示导出的类型，可导出字典，list以及两个都同时导出
上图表格数据配置中可看到SoundSheet中到只导出字典，希望导出的字典以枚举类型SoundEnum为key。而key在数据中的变量为SoundType
![image.png](https://upload-images.jianshu.io/upload_images/3912830-70e81bd7ec8882e5.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
数据表格配置完成后可以直接点击EdgeFramework->Sheet->ExportBytes将Excels 导出bytes,并自动生成加载的Code.
![image.png](https://upload-images.jianshu.io/upload_images/3912830-0f9b1812b5fbbcce.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

表格bytes生成路径为Assets/ABResources/Data/Sheets/文件夹下
![image.png](https://upload-images.jianshu.io/upload_images/3912830-368f26acc8f650cb.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
Code代码生成在Assets/Scripts/Sheet/文件夹下
![image.png](https://upload-images.jianshu.io/upload_images/3912830-8af69bc648b95b70.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
SheetProtobuf.cs中为所有表格中的模板数据类，加载时使用Protobuf序列化.
![image.png](https://upload-images.jianshu.io/upload_images/3912830-a465e4cc0f02c9c7.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
SheetManager.cs中为所有表格加载的函数，例如如需加载SoundSheet表格中的某一行数据，则直接SheetManager.Instance.GetSoundSheet（SoundEnum 类型变量）根据枚举变量可直接查找获取到该行的数据。
![image.png](https://upload-images.jianshu.io/upload_images/3912830-c2966e253fe0c47a.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
如需自行扩展函数的话可直接在SheetManagerEx.cs中扩展。
### 六、BuildAPP
打包Apk的时候可直接点击EdgeFramework->AssetsBundle->BuildApp或者快捷键F12即可启动打包,可看到下图，打包App时会自动设置版本号，自动设置keystore，并重新打Ab包,拷贝入streamassets下。并打打包完毕后自动移除streamassets下的ab包避免git推送
![image](https://user-images.githubusercontent.com/24520716/110449026-ba430380-80fc-11eb-9486-8b1c0671ddd1.png)
### 七、代码规范
```
(1)枚举类型和枚举常量都使用大驼峰命名，可加Type后缀
        public enum ExampleType
        {
        None,
        ExampleOne
        }
(2)类名一般用大驼峰，即首字母大写,一般我会以对象名相同的名创建
(3)类名称尽量少用或不用缩写，若使用了缩写一定要在注释中详细注明类的用途   
(4)类名要用名词。模板类开头用T。例如TSubject  
(5)接口开头用I。接口名要用名词。
(6)缩写
        /**
        * GameObject->Go
        * Transform->Trans
        * Position->Pos
        * Button->Btn
        * Dictionary->Dict
        * Number->Num
        * Current->Cur
        * Controller->Ctrl
        */
(7)  /// 私有最好也别省略private
    /// 私有变量可以加前缀 m 表示私有成员 mExampleBtn
(8) ///公有变量和公共属性使用首字母大写驼峰式类的,尽量用属性代替公有变量。
(9)///常量所有单词大写，多个单词之间用下划线隔开
(10)///方法名一律使用首字母大写驼峰式
(11)///局部变量最好用var匿名声明 小写驼峰式
(12)///静态变量可以加前缀 s 表示静态 sExample
```
