/****************************************************
	ÎÄ¼þ£ºActionKitEvent.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:17   	
	Features£º
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{

    public class ActionKitEvent : MonoBehaviour , ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<ActionData> ActionsDatas = new List<ActionData>();

        public void Execute()
        {
            var sequence = this.Sequence();
            
            foreach (var t in ActionsDatas)
            {
                var type = ActionTypeDB.GetTypeByFullName(t.ActionName);
                var action = JsonUtility.FromJson(t.AcitonData, type) as IAction;
                sequence.Append(action);
            }

            sequence.Begin();
        }

        [SerializeField]
        public List<ActionKitAction> Actions = new List<ActionKitAction>();
        
        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}