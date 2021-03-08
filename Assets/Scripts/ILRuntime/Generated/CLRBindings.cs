using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_Debug_Binding.Register(app);
            System_String_Binding.Register(app);
            CRLBindingTestClass_Binding.Register(app);
            EdgeFramework_MonoSingleton_1_GameRoot_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            TestDelegatMeth_Binding.Register(app);
            TestDelegatFunction_Binding.Register(app);
            System_Action_1_String_Binding.Register(app);
            System_Int32_Binding.Register(app);
            EdgeFramework_Singleton_1_ILRuntimeManager_Binding.Register(app);
            ILRuntimeManager_Binding.Register(app);
            TestClassBase_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
