/****************************************************
	文件：BehaviourActionNonPrioritizedSelector.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:08   	
	Features：
*****************************************************/
using System;
using System.Collections.Generic;

namespace EdgeFramework
{
    public class BehaviourActionNonPrioritizedSelector : BehaviourActionPrioritizedSelector
    {
        public BehaviourActionNonPrioritizedSelector()
            : base()
        {
        }
        protected override bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            BehaviourActionPrioritizedSelector.TBTActionPrioritizedSelectorContext thisContext = 
                getContext<BehaviourActionPrioritizedSelector.TBTActionPrioritizedSelectorContext>(wData);
            //check last node first
            if (IsIndexValid(thisContext.currentSelectedIndex)) {
                BehaviourAction node = GetChild<BehaviourAction>(thisContext.currentSelectedIndex);
                if (node.Evaluate(wData)) {
                    return true;
                }
            }
            return base.onEvaluate(wData);
        }
    }
}
