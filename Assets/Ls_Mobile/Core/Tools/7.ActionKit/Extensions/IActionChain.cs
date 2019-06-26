using UnityEngine;
using System;
namespace Ls_Mobile
{
    /// <summary>
    /// 执行链表,
    /// </summary>
    public interface IActionChain : IAction
    {
        MonoBehaviour Executer { get; set; }
        IActionChain Append(IAction node);
        /// <summary>
        /// 开始执行节点链
        /// </summary>
        /// <returns></returns>
        IDisposeWhen Begin();
    }
    public interface IDisposeWhen : IDisposeEventRegister
    {
        IDisposeEventRegister DisposeWhen(Func<bool> condition);
    }
    public interface IDisposeEventRegister
    {
        /// <summary>
        /// 节点链执行完成后销毁清空所有数据
        /// </summary>
        void OnDisposed(System.Action onDisposedEvent);

        IDisposeEventRegister OnFinished(Action onFinishedEvent);
    }
}