using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    public class SequenceNode : NodeAction, INode
    {
        /// <summary>
        /// 所有节点
        /// </summary>
        protected readonly List<IAction> mNodes = new List<IAction>();
        /// <summary>
        /// 还未执行的节点
        /// </summary>
        protected readonly List<IAction> mExcutingNodes = new List<IAction>();
        /// <summary>
        /// 还未执行的节点的数量
        /// </summary>
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
                Debug.LogError(node);
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }
        /// <summary>
        /// 节点重置 将所有已经完成的节点重置为未完成
        /// </summary>
        protected override void OnReset()
        {
            mExcutingNodes.Clear();
            foreach (var node in mNodes)
            {
                node.Reset();
                mExcutingNodes.Add(node);
            }
        }
        //开始执行节点链中的节点
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
        /// <summary>
        /// 当前节点完成后执行此函数
        /// </summary>
        protected virtual void OnCurrentActionFinished() { }
        /// <summary>
        /// 实例化节点链  可将所有需链式执行的节点添加进来
        /// </summary>
        /// <param name="nodes"></param>
        public SequenceNode(params IAction[] nodes)
        {
            foreach (var node in nodes)
            {
                mNodes.Add(node);
                mExcutingNodes.Add(node);
            }
        }
        /// <summary>
        /// 将appendedNode节点添加到节点链中
        /// </summary>
        /// <param name="appendedNode"></param>
        /// <returns></returns>
        public SequenceNode Append(IAction appendedNode)
        {
            mNodes.Add(appendedNode);
            mExcutingNodes.Add(appendedNode);
            return this;
        }
        /// <summary>
        /// 销毁节点链时清理所有数据
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mNodes)
            {
                mNodes.ForEach(node => node.Dispose());
                mNodes.Clear();
            }

            if (null != mExcutingNodes)
            {
                mExcutingNodes.Clear();
            }
        }
    }
}
        
