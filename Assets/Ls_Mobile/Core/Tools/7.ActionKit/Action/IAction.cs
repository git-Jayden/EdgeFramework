using System;
namespace Ls_Mobile
{
    /// <summary>
    /// 执行节点的基类
    /// </summary>
    public interface IAction : IDisposable
    {
        bool Disposed{ get; }

        bool Execute(float delta);

        void Reset();

        void Finish();

        bool Finished { get; }
    }
}