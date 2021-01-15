/****************************************************
	ÎÄ¼þ£ºQueueNode.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:14   	
	Features£º
*****************************************************/

using System.Collections.Generic;

namespace EdgeFramework
{
    public class QueueNode : ActionKitAction
    {
        private Queue<IAction> mQueue = new Queue<IAction>(20);

        public void Enqueue(IAction action)
        {
            mQueue.Enqueue(action);
        }

        protected override void OnExecute(float dt)
        {
            if (mQueue.Count != 0 && mQueue.Peek().Execute(dt))
            {
                mQueue.Dequeue().Dispose();
            }
        }
    }
}