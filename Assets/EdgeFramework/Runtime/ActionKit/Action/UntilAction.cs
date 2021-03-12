/****************************************************
	ÎÄ¼þ£ºUntilAction.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/03/11 11:25   	
	Features£º
*****************************************************/

using System;

namespace EdgeFramework
{
    /// <inheritdoc />
    /// <summary>
    /// like filter, add condition
    /// </summary>
    [OnlyUsedByCode]
    public class UntilAction : ActionKitAction, IPoolable
    {
        private Func<bool> mCondition;

        public static UntilAction Allocate(Func<bool> condition)
        {
            var retNode = SafeObjectPool<UntilAction>.Instance.Allocate();
            retNode.mCondition = condition;
            return retNode;
        }

        protected override void OnExecute(float dt)
        {
            Finished = mCondition.Invoke();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<UntilAction>.Instance.Recycle(this);
        }

        void IPoolable.OnRecycled()
        {
            Reset();
            mCondition = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
}