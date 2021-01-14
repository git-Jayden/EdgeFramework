using System;

namespace EdgeFramework
{
    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点
    /// </summary>
    public class EventAction : NodeAction, IPoolable
    {
        private Action mOnExecuteEvent;

        /// <summary>
        /// TODO:这里填可变参数会有问题
        /// </summary>
        /// <param name="onExecuteEvents"></param>
        /// <returns></returns>
        public static EventAction Allocate(params Action[]OnExecuteEvents)
        {
            var retNode = SafeObjectPool<EventAction>.Instance.Allocate();
            Array.ForEach(OnExecuteEvents, onExecuteEvent=> retNode.mOnExecuteEvent+= onExecuteEvent);
            return retNode;
        }

        /// <summary>
        /// finished
        /// </summary>
        protected override void OnExecute(float dt)
        {
            mOnExecuteEvent?.Invoke();
            Finished = true;
        }
        protected override void OnDispose()
        {
            SafeObjectPool<EventAction>.Instance.Recycle(this);
        }
        public bool IsRecycled { get; set; }

        public void OnRecycled()
        {
            Reset();
            mOnExecuteEvent = null;
        }
    }
}