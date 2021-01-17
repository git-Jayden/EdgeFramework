/****************************************************
	文件：IActionChain.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:11   	
	Features：
*****************************************************/

using UnityEngine;

namespace EdgeFramework
{
    /// <summary>
    /// 执行链表,
    /// </summary>
    public interface IActionChain : IAction
    {
        MonoBehaviour Executer { get; set; }

        IActionChain Append(IAction node);

        IDisposeWhen Begin();
    }
}
