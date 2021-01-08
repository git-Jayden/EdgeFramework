namespace EdgeFramework
{
    public  class SequenceNodeChain : ActionChain
    {
        protected override NodeAction mNode
        {
            get { return mSequenceNode; }
        }

        private SequenceNode mSequenceNode;

        public SequenceNodeChain()
        {
            mSequenceNode = new SequenceNode();
        }
        /// <summary>
        /// 将node节点添加到当前实例mSequenceNode的节点链中并返回自身
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override IActionChain Append(IAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }
}
