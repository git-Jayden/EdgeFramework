/****************************************************
	ÎÄ¼þ£ºRepeatNodeChain.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:12   	
	Features£º
*****************************************************/


namespace EdgeFramework
{
    public class RepeatNodeChain : ActionChain
    {
        protected override ActionKitAction mNode
        {
            get { return mRepeatNodeAction; }
        }

        private RepeatNode mRepeatNodeAction;

        private SequenceNode mSequenceNode;

        public RepeatNodeChain(int repeatCount)
        {
            mSequenceNode = new SequenceNode();
            mRepeatNodeAction = new RepeatNode(mSequenceNode, repeatCount);
        }

        public override IActionChain Append(IAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mRepeatNodeAction)
            {
                mRepeatNodeAction.Dispose();
            }

            mRepeatNodeAction = null;

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }
}
