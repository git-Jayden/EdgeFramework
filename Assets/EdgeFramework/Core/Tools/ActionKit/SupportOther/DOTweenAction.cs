using System;
using DG.Tweening;
namespace EdgeFramework
{
    public class DOTweenAction : NodeAction, IPoolable, IPoolType
    {
        private Func<Tweener> mTweenFactory;

        public static DOTweenAction Allocate(Func<Tweener> tweenFactory)
        {
            var action = SafeObjectPool<DOTweenAction>.Instance.Allocate();

            action.mTweenFactory = tweenFactory;

            return action;
        }

        protected override void OnBegin()
        {
            var tween = mTweenFactory?.Invoke();

            tween.OnComplete(Finish);
        }

        public void OnRecycled()
        {
            mTweenFactory = null;
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<DOTweenAction>.Instance.Recycle(this);
        }
    }
    public static partial class IActionChainExtention
    {
        public static IActionChain DOTween(this IActionChain selfChain, Func<Tweener> tweenFactory)
        {
            return selfChain.Append(DOTweenAction.Allocate(tweenFactory));
        }
    }
}