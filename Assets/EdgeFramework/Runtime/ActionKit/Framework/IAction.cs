/****************************************************
	文件：IAction.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:13   	
	Features：
*****************************************************/

using System;

namespace EdgeFramework
{
    /// <summary>
    /// 执行节点的基类
    /// </summary>
    public interface IAction : IDisposable
    {

        bool Disposed { get; }

        bool Execute(float delta);

        void Reset();

        void Finish();

        bool Finished { get; }
    }
}