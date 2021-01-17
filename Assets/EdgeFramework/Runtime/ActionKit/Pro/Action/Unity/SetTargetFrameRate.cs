/****************************************************
	ÎÄ¼þ£ºSetTargetFrameRate.cs
	Author£ºJaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date£º2021/01/15 9:16   	
	Features£º
*****************************************************/

using UnityEngine;

namespace EdgeFramework
{
    [ActionGroup("Unity")]
    public class SetTargetFrameRate : ActionKitAction
    {
        [SerializeField] public int FPS = 30;
        
        protected override void OnBegin()
        {
            Application.targetFrameRate = FPS;
            Finish();
        }
    }
}