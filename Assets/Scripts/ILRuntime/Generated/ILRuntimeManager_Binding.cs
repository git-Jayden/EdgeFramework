using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class ILRuntimeManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ILRuntimeManager);

            field = type.GetField("DelegateMethod", flag);
            app.RegisterCLRFieldGetter(field, get_DelegateMethod_0);
            app.RegisterCLRFieldSetter(field, set_DelegateMethod_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_DelegateMethod_0, AssignFromStack_DelegateMethod_0);
            field = type.GetField("DelegateFunc", flag);
            app.RegisterCLRFieldGetter(field, get_DelegateFunc_1);
            app.RegisterCLRFieldSetter(field, set_DelegateFunc_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_DelegateFunc_1, AssignFromStack_DelegateFunc_1);
            field = type.GetField("DelegateAction", flag);
            app.RegisterCLRFieldGetter(field, get_DelegateAction_2);
            app.RegisterCLRFieldSetter(field, set_DelegateAction_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_DelegateAction_2, AssignFromStack_DelegateAction_2);


        }



        static object get_DelegateMethod_0(ref object o)
        {
            return ((global::ILRuntimeManager)o).DelegateMethod;
        }

        static StackObject* CopyToStack_DelegateMethod_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ILRuntimeManager)o).DelegateMethod;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_DelegateMethod_0(ref object o, object v)
        {
            ((global::ILRuntimeManager)o).DelegateMethod = (global::TestDelegatMeth)v;
        }

        static StackObject* AssignFromStack_DelegateMethod_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::TestDelegatMeth @DelegateMethod = (global::TestDelegatMeth)typeof(global::TestDelegatMeth).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ILRuntimeManager)o).DelegateMethod = @DelegateMethod;
            return ptr_of_this_method;
        }

        static object get_DelegateFunc_1(ref object o)
        {
            return ((global::ILRuntimeManager)o).DelegateFunc;
        }

        static StackObject* CopyToStack_DelegateFunc_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ILRuntimeManager)o).DelegateFunc;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_DelegateFunc_1(ref object o, object v)
        {
            ((global::ILRuntimeManager)o).DelegateFunc = (global::TestDelegatFunction)v;
        }

        static StackObject* AssignFromStack_DelegateFunc_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::TestDelegatFunction @DelegateFunc = (global::TestDelegatFunction)typeof(global::TestDelegatFunction).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ILRuntimeManager)o).DelegateFunc = @DelegateFunc;
            return ptr_of_this_method;
        }

        static object get_DelegateAction_2(ref object o)
        {
            return ((global::ILRuntimeManager)o).DelegateAction;
        }

        static StackObject* CopyToStack_DelegateAction_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ILRuntimeManager)o).DelegateAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_DelegateAction_2(ref object o, object v)
        {
            ((global::ILRuntimeManager)o).DelegateAction = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_DelegateAction_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @DelegateAction = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ILRuntimeManager)o).DelegateAction = @DelegateAction;
            return ptr_of_this_method;
        }



    }
}
