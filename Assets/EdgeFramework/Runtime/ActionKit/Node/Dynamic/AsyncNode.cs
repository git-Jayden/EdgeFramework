/****************************************************
	ÎÄ¼þ£ºAsyncNode.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:14   	
	Features£º
*****************************************************/
using System.Collections.Generic;
using System.Linq;

namespace EdgeFramework
{
    public class AsyncNode : ActionKitAction
    {
        public HashSet<IAction> mActions = new HashSet<IAction>();
        
        public void Add(IAction action)
        {
            mActions.Add(action);
        }

        protected override void OnExecute(float dt)
        {
            foreach (var action in mActions.Where(action => action.Execute(dt)))
            {
                mActions.Remove(action);
                action.Dispose();
            }
        }
    }
}