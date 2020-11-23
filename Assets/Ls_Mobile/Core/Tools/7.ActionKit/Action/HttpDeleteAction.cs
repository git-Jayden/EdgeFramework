using System;
using System.Collections.Generic;
namespace EdgeFramework
{
    public abstract class HttpDeleteAction:NodeAction
    {
        protected abstract Dictionary<string, string> Headers { get; }
        protected abstract string Url { get; }


        protected abstract string Id { get; }

        private IDisposable mDisposable;

        protected override void OnBegin()
        {
            mDisposable = API.HttpDelete(Url.FillFormat(Id), Headers, () =>
            {
                mDisposable = null;
                OnResponse();
                Finish();
            });

        }
        protected abstract void OnResponse();
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
