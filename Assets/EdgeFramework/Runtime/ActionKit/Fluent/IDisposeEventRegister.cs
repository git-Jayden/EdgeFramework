

using System;

namespace EdgeFramework
{
    public interface IDisposeEventRegister
    {
        void OnDisposed(System.Action onDisposedEvent);

        IDisposeEventRegister OnFinished(System.Action onFinishedEvent);
    }
}