/****************************************************
	ÎÄ¼þ£ºOnDestroyTrigger.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:11   	
	Features£º
*****************************************************/


using System;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework

{
    public class OnDestroyTrigger : MonoBehaviour
    {
        HashSet<IDisposable> mDisposables = new HashSet<IDisposable>();

        public void AddDispose(IDisposable disposable)
        {
            if (!mDisposables.Contains(disposable))
            {
                mDisposables.Add(disposable);
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                foreach (var disposable in mDisposables)
                {
                    disposable.Dispose();
                }

                mDisposables.Clear();
                mDisposables = null;
            }
        }
    }
}
