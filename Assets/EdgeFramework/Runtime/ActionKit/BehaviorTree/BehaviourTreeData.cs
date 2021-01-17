/****************************************************
	文件：BehaviourTreeData.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:09   	
	Features：
*****************************************************/
using System;
using System.Collections.Generic;

namespace EdgeFramework
{
    public class BehaviourTreeData 
    {
        //------------------------------------------------------
        internal Dictionary<int, TBTActionContext> mContext;
        internal Dictionary<int, TBTActionContext> context 
        {
            get 
            {
                return mContext;
            }
        }
        //------------------------------------------------------
        public BehaviourTreeData()
        {
            mContext = new Dictionary<int, TBTActionContext>();
        }
        ~BehaviourTreeData()
        {
            mContext = null;
        }
    }
}
