/****************************************************
	ÎÄ¼þ£ºActionQueue.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:10   	
	Features£º
*****************************************************/


using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    [MonoSingletonPath("[ActionKit]/ActionQueue")]
    public class ActionQueue : MonoBehaviour, ISingleton
    {
        private List<IAction> mActions = new List<IAction>();

        public static void Append(IAction action)
        {
            mInstance.mActions.Add(action);
        }

        // Update is called once per frame
        private void Update()
        {
            if (mActions.Count != 0 && mActions[0].Execute(Time.deltaTime))
            {
                mActions.RemoveAt(0);
            }
        }

        void ISingleton.OnSingletonInit()
        {
        }

        private static ActionQueue mInstance
        {
            get { return MonoSingletonProperty<ActionQueue>.Instance; }
        }
    }
}
