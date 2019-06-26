﻿using UnityEngine;
using System;
namespace Ls_Mobile
{
    public abstract class ActionChain : NodeAction, IActionChain, IDisposeWhen
    {
        public MonoBehaviour Executer { get; set; }

        protected abstract NodeAction mNode { get; }

        public abstract IActionChain Append(IAction node);

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition.InvokeGracefully())
            {
                Finish();
            }
            else
            {
                Finished = mNode.Execute(dt);
            }
        }
        protected override void OnEnd()
        {
            base.OnEnd();
            Dispose();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Executer = null;
            mDisposeWhenCondition = false;
            mDisposeCondition = null;
            mOnDisposedEvent.InvokeGracefully();
            mOnDisposedEvent = null;
        }
        /// <summary>
        /// 开始执行节点链
        /// </summary>
        /// <returns></returns>
        public IDisposeWhen Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }
        private bool mDisposeWhenCondition = false;
        private Func<bool> mDisposeCondition;
        private Action mOnDisposedEvent = null;


        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }
        public void OnDisposed(System.Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }
        IDisposeEventRegister IDisposeEventRegister.OnFinished(Action onFinishedEvent)
        {
            OnEndedCallback += onFinishedEvent;
            return this;
        }
    }
}