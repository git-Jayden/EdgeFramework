using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdgeFramework
{
    public interface IRefCounter
    {
        int RefCount { get; }

        void Retain(object refOwner = null);
        void Release(object refOwner = null);
    }
    public class SimpleRC : IRefCounter
    {
        /// <summary>
        /// 数量初始化
        /// </summary>
        public SimpleRC()
        {
            RefCount = 0;
        }
        /// <summary>
        /// 当前数量
        /// </summary>
        public int RefCount { get; private set; }
        /// <summary>
        /// 数量++
        /// </summary>
        /// <param name="refOwner"></param>
        public void Retain(object refOwner = null)
        {
            ++RefCount;
        }
        /// <summary>
        /// 数量--
        /// </summary>
        /// <param name="refOwner"></param>
        public void Release(object refOwner = null)
        {
            --RefCount;
            if (RefCount == 0)
            {
                OnZeroRef();
            }
        }
        /// <summary>
        /// 当数量减到0
        /// </summary>
        protected virtual void OnZeroRef()
        {
        }
    }

}
