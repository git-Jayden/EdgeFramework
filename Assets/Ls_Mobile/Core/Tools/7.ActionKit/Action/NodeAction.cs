
using System;
namespace Ls_Mobile
{
    public class NodeAction : IAction
    {
        public Action OnBeganCallback = null;
        public Action OnEndedCallback = null;
        public Action OnDisposedCallback = null;
        protected bool mOnBeginCalled = false;
        #region IAction Support
        bool IAction.Disposed
        {
            get { return mDisposed; }
        }
        protected bool mDisposed = false;

        public bool Finished { get; protected set; }

        public virtual void Finish()
        {
            Finished = true;
            OnEndedCallback.InvokeGracefully();
            OnEnd();
        }

        public void Break()
        {
            Finished = true;
        }
        #endregion


        #region ResetableSupport

        public void Reset()
        {
            Finished = false;
            mOnBeginCalled = false;
            mDisposed = false;
            OnReset();
        }
        #endregion
        #region IExecutable Support
        public bool Execute(float dt)
        {
            //有可能被别的地方调用
            if (Finished)
            {
                return Finished;
            }
            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();
                OnBeganCallback.InvokeGracefully();
            }
            if (!Finished)
            {
                OnExecute(dt);
            }
            if (Finished)
            {
                Finish();
            }
            return Finished || mDisposed;
        }

        #endregion

        protected virtual void OnReset()
        {
        }
        protected virtual void OnBegin()
        {
        }

        /// <summary>
        /// finished
        /// </summary>
        protected virtual void OnExecute(float dt)
        {
        }
        protected virtual void OnEnd()
        {
        }
        protected virtual void OnDispose()
        {
        }

        #region IDisposable Support

        public void Dispose2()
        {
            if (mDisposed) return;
            mDisposed = true;

            OnBeganCallback = null;
            OnEndedCallback = null;
            OnDisposedCallback.InvokeGracefully();
            OnDisposedCallback = null;
            OnDispose();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}