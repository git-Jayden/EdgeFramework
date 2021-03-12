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
    unsafe class ProcedureLoadingScene_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ProcedureLoadingScene);

            field = type.GetField("LoadingProgress", flag);
            app.RegisterCLRFieldGetter(field, get_LoadingProgress_0);
            app.RegisterCLRFieldSetter(field, set_LoadingProgress_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoadingProgress_0, AssignFromStack_LoadingProgress_0);


        }



        static object get_LoadingProgress_0(ref object o)
        {
            return ((global::ProcedureLoadingScene)o).LoadingProgress;
        }

        static StackObject* CopyToStack_LoadingProgress_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ProcedureLoadingScene)o).LoadingProgress;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_LoadingProgress_0(ref object o, object v)
        {
            ((global::ProcedureLoadingScene)o).LoadingProgress = (System.Int32)v;
        }

        static StackObject* AssignFromStack_LoadingProgress_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @LoadingProgress = ptr_of_this_method->Value;
            ((global::ProcedureLoadingScene)o).LoadingProgress = @LoadingProgress;
            return ptr_of_this_method;
        }



    }
}
