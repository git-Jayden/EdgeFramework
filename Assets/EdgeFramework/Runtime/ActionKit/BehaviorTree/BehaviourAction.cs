/****************************************************
	文件：BehaviourAction.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:07   	
	Features：
*****************************************************/
using System;

namespace EdgeFramework
{
    public class TBTActionContext
    {
    }

    public abstract class BehaviourAction : BehaviourTreeNode
    {
        static private int sUNIQUEKEY = 0;
        static private int genUniqueKey()
        {
            if (sUNIQUEKEY >= int.MaxValue){
                sUNIQUEKEY = 0;
            } else {
                sUNIQUEKEY = sUNIQUEKEY + 1;
            }
            return sUNIQUEKEY;
        }
        //-------------------------------------------------------------
        protected int _uniqueKey;
        protected BehaviourPrecondition _precondition;
#if DEBUG
        protected string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
#endif
        //-------------------------------------------------------------
        public BehaviourAction(int maxChildCount)
            : base(maxChildCount)
        {
            _uniqueKey = BehaviourAction.genUniqueKey();
        }
        ~BehaviourAction()
        {
            _precondition = null;
        }
        //-------------------------------------------------------------
        public bool Evaluate(/*in*/BehaviourTreeData wData)
        {
            return (_precondition == null || _precondition.IsTrue(wData)) && onEvaluate(wData);
        }
        public int Update(BehaviourTreeData wData)
        {
            return onUpdate(wData);
        }
        public void Transition(BehaviourTreeData wData)
        {
            onTransition(wData);
        }
        public BehaviourAction SetPrecondition(BehaviourPrecondition precondition)
        {
            _precondition = precondition;
            return this;
        }
        public override int GetHashCode()
        {
            return _uniqueKey;
        }
        protected T getContext<T>(BehaviourTreeData wData) where T : TBTActionContext, new()
        {
            int uniqueKey = GetHashCode();
            T thisContext;
            if (wData.context.ContainsKey(uniqueKey) == false) {
                thisContext = new T();
                wData.context.Add(uniqueKey, thisContext);
            } else {
                thisContext = (T)wData.context[uniqueKey];
            }
            return thisContext;
        }
        //--------------------------------------------------------
        // inherented by children
        protected virtual bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            return true;
        }
        protected virtual int onUpdate(BehaviourTreeData wData)
        {
            return BehaviourTreeRunningStatus.FINISHED;
        }
        protected virtual void onTransition(BehaviourTreeData wData)
        {
        }
    }
}
