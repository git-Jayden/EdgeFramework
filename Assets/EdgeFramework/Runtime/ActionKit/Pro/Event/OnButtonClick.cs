/****************************************************
	�ļ���OnButtonClick.cs
	Author��JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date��2021/01/15 9:16   	
	Features��
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace EdgeFramework
{
    [RequireComponent(typeof(Button))]
    public class OnButtonClick : ActionKitEvent
    {
        private void Awake()
        {
            GetComponent<Button>()
                .onClick.AddListener(Execute);
        }
    }
}