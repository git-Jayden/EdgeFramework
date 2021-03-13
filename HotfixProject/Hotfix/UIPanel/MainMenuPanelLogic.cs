using EdgeFramework.UI;
using UnityEngine;
namespace Hotfix
{
    public class MainMenuPanelLogic : BaseUI
    {
        
        
        public override void PlayBtnSound()
        {

        }

        public override void OnUpdate()
        {
     
        }
        public override void OnEnter(object param1 = null, object param2 = null, object param3 = null)
        {
            UIObj.SetActive(true);

        }
        public override void OnExit()
        {
            UIObj.SetActive(false);
    
        }
    }
}