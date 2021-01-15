/****************************************************
	文件：Timeline.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:18   	
	Features：
*****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace EdgeFramework
{
  /// <summary>
    /// 时间轴执行节点
    /// </summary>
    public class Timeline : ActionKitAction
    {
        private float mCurTime = 0;

        public System.Action OnTimelineBeganCallback
        {
            get { return OnBeganCallback; }
            set { OnBeganCallback = value; }
        }

        public System.Action OnTimelineEndedCallback
        {
            get { return OnEndedCallback; }
            set { OnEndedCallback = value; }
        }

        public Action<string> OnKeyEventsReceivedCallback = null;

        public class TimelinePair
        {
            public float Time;
            public IAction Node;

            public TimelinePair(float time, IAction node)
            {
                Time = time;
                Node = node;
            }
        }

        /// <summary>
        /// refator 2 one list? all in one list;
        /// </summary>
        public Queue<TimelinePair> TimelineQueue = new Queue<TimelinePair>();

        protected override void OnReset()
        {
            mCurTime = 0.0f;

            foreach (var timelinePair in TimelineQueue)
            {
                timelinePair.Node.Reset();
            }
        }

        protected override void OnExecute(float dt)
        {
            mCurTime += dt;

            foreach (var pair in TimelineQueue.Where(pair => pair.Time < mCurTime && !pair.Node.Finished))
            {
                if (pair.Node.Execute(dt))
                {
                    Finished = TimelineQueue.Count(timelinePair => !timelinePair.Node.Finished) == 0;
                }
            }
        }

        public Timeline(params TimelinePair[] pairs)
        {
            foreach (var pair in pairs)
            {
                TimelineQueue.Enqueue(pair);
            }
        }

        public void Append(TimelinePair pair)
        {
            TimelineQueue.Enqueue(pair);
        }

        public void Append(float time, IAction node)
        {
            TimelineQueue.Enqueue(new TimelinePair(time, node));
        }

        protected override void OnDispose()
        {
            foreach (var timelinePair in TimelineQueue)
            {
                timelinePair.Node.Dispose();
            }

            TimelineQueue.Clear();
            TimelineQueue = null;
        }
    }
}