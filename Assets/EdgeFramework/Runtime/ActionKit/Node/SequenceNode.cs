/****************************************************
	文件：SequenceNode.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:15   	
	Features：
*****************************************************/

using System.Collections.Generic;

namespace EdgeFramework
{
    /// <summary>
    /// 序列执行节点
    /// </summary>
    [OnlyUsedByCode]
    public class SequenceNode : ActionKitAction, INode,IResetable
    {
        protected List<IAction> mNodes = ListPool<IAction>.Get();
        protected List<IAction> mExcutingNodes = ListPool<IAction>.Get();

        public int TotalCount
        {
            get { return mExcutingNodes.Count; }
        }

        public IAction CurrentExecutingNode
        {
            get
            {
                var currentNode = mExcutingNodes[0];
                var node = currentNode as INode;
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }

        protected override void OnReset()
        {
            mExcutingNodes.Clear();
            foreach (var node in mNodes)
            {
                node.Reset();
                mExcutingNodes.Add(node);
            }
        }

        protected override void OnExecute(float dt)
        {
            if (mExcutingNodes.Count > 0)
            {
                // 如果有异常，则进行销毁，不再进行下边的操作
                if (mExcutingNodes[0].Disposed && !mExcutingNodes[0].Finished)
                {
                    Dispose();
                    return;
                }

                while (mExcutingNodes[0].Execute(dt))
                {
                    mExcutingNodes.RemoveAt(0);

                    OnCurrentActionFinished();

                    if (mExcutingNodes.Count == 0)
                    {
                        break;
                    }
                }
            }

            Finished = mExcutingNodes.Count == 0;
        }

        protected virtual void OnCurrentActionFinished()
        {
        }

        public SequenceNode(params IAction[] nodes)
        {
            foreach (var node in nodes)
            {
                mNodes.Add(node);
                mExcutingNodes.Add(node);
            }
        }

        public SequenceNode Append(IAction appendedNode)
        {
            mNodes.Add(appendedNode);
            mExcutingNodes.Add(appendedNode);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mNodes)
            {
                mNodes.ForEach(node => node.Dispose());
                mNodes.Clear();
                mNodes.Release2Pool();
                mNodes = null;
            }

            if (null != mExcutingNodes)
            {
                mExcutingNodes.Release2Pool();
                mExcutingNodes = null;
            }
        }
    }

}