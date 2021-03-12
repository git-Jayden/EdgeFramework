using System;
using EdgeFramework.UI;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class BaseUIAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType { get { return typeof(BaseUI); } }

    public override Type AdaptorType {
        get
        {
            //实际的适配器类
            return typeof(Adapter);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adapter(appdomain, instance);
    }
    class Adapter : BaseUI, CrossBindingAdaptorType
    { 
        private ILTypeInstance mInstance;
        private ILRuntime.Runtime.Enviorment.AppDomain mAppDomain;
        private object[] mParalist = new object[3];
        private IMethod mOnEnterMethod;
        private IMethod mOnPauseMethod;
        private IMethod mOnResumeMethod;
        private IMethod mOnUpdateMethod;
        private IMethod mOnExitMethod;
        private IMethod mPlayBtnSoundMethod;
        private IMethod mToString;
   


        public Adapter() { }
        public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            mInstance = instance;
            mAppDomain = appdomain;

        }
        public ILTypeInstance ILInstance
        {
            get
            {
                return mInstance;
            }
        }
        public override void OnEnter(object param1 = null, object param2 = null, object param3 = null)
        {
            if (mOnEnterMethod == null)
            {
                mOnEnterMethod = mInstance.Type.GetMethod("OnEnter", 3);
            }
            if (mOnEnterMethod != null)
            {
                mParalist[0] = param1;
                mParalist[1] = param2;
                mParalist[2] = param3;
                mAppDomain.Invoke(mOnEnterMethod, mInstance, mParalist);
            }
        }
        public override void OnPause()
        {
            if (mOnPauseMethod == null)
            {
                mOnPauseMethod = mInstance.Type.GetMethod("OnPause",0);
            }
            if (mOnPauseMethod != null)
            {
                mAppDomain.Invoke(mOnPauseMethod, mInstance);
            }
        }
        public override void OnResume()
        {
            if (mOnResumeMethod == null)
            {
                mOnResumeMethod = mInstance.Type.GetMethod("OnResume", 0);
            }
            if (mOnResumeMethod != null)
            {
                mAppDomain.Invoke(mOnResumeMethod, mInstance);
            }
        }

        public override void OnUpdate()
        {
            if (mOnUpdateMethod == null)
            {
                mOnUpdateMethod = mInstance.Type.GetMethod("OnUpdate", 0);
            }

            if (mOnUpdateMethod != null)
            {
                mAppDomain.Invoke(mOnUpdateMethod, mInstance);
            }
        }

        public override void OnExit()
        {
            if (mOnExitMethod == null)
            {
                mOnExitMethod = mInstance.Type.GetMethod("OnExit", 0);
            }
            if (mOnExitMethod != null)
            {
                mAppDomain.Invoke(mOnExitMethod, mInstance);
            }
        }

        public override void PlayBtnSound()
        {
            if (mPlayBtnSoundMethod == null)
            {
                mPlayBtnSoundMethod = mInstance.Type.GetMethod("PlayBtnSound", 0);
            }
            if (mPlayBtnSoundMethod != null)
            {
                mAppDomain.Invoke(mPlayBtnSoundMethod, mInstance);
            }
        }

        //OnMessage自己处理

        public override string ToString()
        {
            if (mToString == null)
            {
                mToString = mAppDomain.ObjectType.GetMethod("ToString", 0);
            }
            IMethod m = mInstance.Type.GetVirtualMethod(mToString);
            if (m == null || m is ILMethod)
            {
                return mInstance.ToString();
            }
            else
            {
                return mInstance.Type.FullName;
            }
        }
    }
}
