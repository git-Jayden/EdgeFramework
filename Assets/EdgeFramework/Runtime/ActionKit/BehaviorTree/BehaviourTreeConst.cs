/****************************************************
	文件：BehaviourTreeConst.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:09   	
	Features：
*****************************************************/
using System;

namespace EdgeFramework
{
    public class BehaviourTreeRunningStatus
    {
        //-------------------------------------------------------
        //Any value which is below ZERO means error occurs 
        //-------------------------------------------------------
        //default running status
        public const int EXECUTING   = 0;
        public const int FINISHED    = 1;
        public const int TRANSITION  = 2;
        //-------------------------------------------------------
        //User running status
        //100-999, reserved user executing status
        public const int USER_EXECUTING = 100;
        //>=1000, reserved user finished status
        public const int USER_FINISHED = 1000;
        //-------------------------------------------------------
        static public bool IsOK(int runningStatus)
        {
            return runningStatus == BehaviourTreeRunningStatus.FINISHED ||
                   runningStatus >= BehaviourTreeRunningStatus.USER_FINISHED;
        }
        static public bool IsError(int runningStatus)
        {
            return runningStatus < 0;
        }
        static public bool IsFinished(int runningStatus)
        {
            return IsOK(runningStatus) || IsError(runningStatus);
        }
        static public bool IsExecuting(int runningStatus)
        {
            return !IsFinished(runningStatus);
        }
    }
}
