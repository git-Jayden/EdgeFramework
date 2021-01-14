using System;
namespace EdgeFramework
{
    /// <inheritdoc />
    /// <summary>
    /// like filter, add condition
    /// </summary>
    public class UntilAction : NodeAction, IPoolable
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
            if (null != mCondition)
            {
                mCondition();
                Finished= true;
                
            }else
            Finished= false;
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
