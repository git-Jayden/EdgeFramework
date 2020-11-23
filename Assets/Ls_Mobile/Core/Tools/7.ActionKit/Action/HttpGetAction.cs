
using System;
using System.Collections.Generic;
namespace EdgeFramework
{
    public abstract class HttpGetAction: NodeAction
    {
        protected abstract Dictionary<string, string> Headers { get; }
        protected abstract string Url { get; }

        private IDisposable mDisposable;

        protected override void OnBegin()
        {
            mDisposable = API.HttpGet(Url, Headers, responseText =>
            {
                mDisposable = null;
                OnResponse(responseText);
                Finish();
            });
        }

        protected abstract void OnResponse(string text);

        protected override void OnDispose()
        {
            if (mDisposable.IsNotNull())
            {
                mDisposable.Dispose();
                mDisposable = null;
            }

            base.OnDispose();
        }

    }
}
