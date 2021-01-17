

using System;

namespace EdgeFramework
{
    public interface IDisposeWhen : IDisposeEventRegister
    {
        IDisposeEventRegister DisposeWhen(Func<bool> condition);
    }
}