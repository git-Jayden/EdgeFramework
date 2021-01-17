/****************************************************
	ÎÄ¼þ£ºActionChain.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:11   	
	Features£º
*****************************************************/

using System;
using UnityEngine;

namespace EdgeFramework
{
    public abstract class ActionChain : ActionKitAction, IActionChain, IDisposeWhen
    {
        public MonoBehaviour Executer { get; set; }

        protected abstract ActionKitAction mNode { get; }

        public abstract IActionChain Append(IAction node);

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition != null && mDisposeCondition.Invoke())
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

            if (mOnDisposedEvent != null)
            {
                mOnDisposedEvent.Invoke();
            }

            mOnDisposedEvent = null;
        }

        public IDisposeWhen Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }

        private bool mDisposeWhenCondition = false;
        private Func<bool> mDisposeCondition;
        private System.Action mOnDisposedEvent = null;

        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }

        IDisposeEventRegister IDisposeEventRegister.OnFinished(System.Action onFinishedEvent)
        {
            OnEndedCallback += onFinishedEvent;
            return this;
        }

        public void OnDisposed(System.Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }
    }
}
