/****************************************************
	文件：TimerHelper.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:27   	
	Features：
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
namespace EdgeFramework
{
    public class TimerHelper : MonoBehaviour
    {

        protected List<TimeItem> m_TimeItemList;
        protected bool m_IsUseAble = true;

        public void Add(TimeItem item)
        {
            if (!m_IsUseAble)
            {
                Debug.LogError("TimeHelper Not Use Able...");
                return;
            }

            if (m_TimeItemList == null)
            {
                m_TimeItemList = new List<TimeItem>(2);
            }
            m_TimeItemList.Add(item);
        }

        public void Clear()
        {
            if (m_TimeItemList != null)
            {
                for (int i = m_TimeItemList.Count - 1; i >= 0; --i)
                {
                    m_TimeItemList[i].Cancel();
                }

                m_TimeItemList.Clear();
            }
        }
    }
}