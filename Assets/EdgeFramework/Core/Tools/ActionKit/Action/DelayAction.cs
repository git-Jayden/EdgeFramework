namespace EdgeFramework
{
    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点
    /// </summary>
    public class DelayAction : NodeAction,IPoolable
    {

        public float DelayTime;
        /// <summary>
        /// 分配 将事件和时间分配下去并返回当前节点
        /// </summary>
        /// <param name="dalayTime">N秒</param>
        /// <param name="onEndCallback">N秒后执行该函数</param>
        /// <param name="OnBeganCallback">开始计时时执行该函数</param>
        /// <param name="OnDisposedCallback"></param>
        /// <returns></returns>
        public static DelayAction Allocate(float dalayTime,System.Action onEndCallback=null, System.Action OnBeganCallback = null, System.Action OnDisposedCallback = null)
        {
            var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
            retNode.DelayTime= dalayTime;
            retNode.OnEndedCallback = onEndCallback;
            retNode.OnBeganCallback = OnBeganCallback;
            retNode.OnDisposedCallback = OnDisposedCallback;
            return retNode;
        }
        public DelayAction()
        { }
        public DelayAction(float delayTime)
        {
            DelayTime = delayTime;
        }
        private float mCurrentSeconds = 0.0f;

        protected override void OnReset()
        {
            mCurrentSeconds = 0.0f;
        }
        protected override void OnExecute(float dt)
        {
            mCurrentSeconds += dt;
            Finished = mCurrentSeconds >= DelayTime;
        }
        protected override void OnDispose()
        {
            SafeObjectPool<DelayAction>.Instance.Recycle(this);
        }
        public void OnRecycled()
        {
            DelayTime = 0.0f;
            Reset();
        }
        public bool IsRecycled { get; set; }
    }

}