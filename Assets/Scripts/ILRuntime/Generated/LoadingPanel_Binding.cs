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
    unsafe class LoadingPanel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::LoadingPanel);

            field = type.GetField("slider", flag);
            app.RegisterCLRFieldGetter(field, get_slider_0);
            app.RegisterCLRFieldSetter(field, set_slider_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_slider_0, AssignFromStack_slider_0);
            field = type.GetField("text", flag);
            app.RegisterCLRFieldGetter(field, get_text_1);
            app.RegisterCLRFieldSetter(field, set_text_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_text_1, AssignFromStack_text_1);


        }



        static object get_slider_0(ref object o)
        {
            return ((global::LoadingPanel)o).slider;
        }

        static StackObject* CopyToStack_slider_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoadingPanel)o).slider;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_slider_0(ref object o, object v)
        {
            ((global::LoadingPanel)o).slider = (UnityEngine.UI.Slider)v;
        }

        static StackObject* AssignFromStack_slider_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Slider @slider = (UnityEngine.UI.Slider)typeof(UnityEngine.UI.Slider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::LoadingPanel)o).slider = @slider;
            return ptr_of_this_method;
        }

        static object get_text_1(ref object o)
        {
            return ((global::LoadingPanel)o).text;
        }

        static StackObject* CopyToStack_text_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LoadingPanel)o).text;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_text_1(ref object o, object v)
        {
            ((global::LoadingPanel)o).text = (UnityEngine.UI.Text)v;
        }

        static StackObject* AssignFromStack_text_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.UI.Text @text = (UnityEngine.UI.Text)typeof(UnityEngine.UI.Text).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::LoadingPanel)o).text = @text;
            return ptr_of_this_method;
        }



    }
}
