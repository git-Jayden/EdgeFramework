/****************************************************
	文件：SequenceNodeChain.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:12   	
	Features：
*****************************************************/


namespace EdgeFramework
{
    /// <summary>
    /// 支持链式方法
    /// </summary>
    public class SequenceNodeChain : ActionChain
    {
        protected override ActionKitAction mNode
        {
            get { return mSequenceNode; }
        }

        private SequenceNode mSequenceNode;

        public SequenceNodeChain()
        {
            mSequenceNode = new SequenceNode();
        }

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
