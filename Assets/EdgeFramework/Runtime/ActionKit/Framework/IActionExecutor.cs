/****************************************************
	ÎÄ¼þ£ºIActionExecutor.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:14   	
	Features£º
*****************************************************/

namespace EdgeFramework
{
    public interface IActionExecutor 
    {
        void ExecuteAction(IAction action);
    }

    public class MonoExecutor : IActionExecutor
    {
        public void ExecuteAction(IAction action)
        {
            
        }
    }
}