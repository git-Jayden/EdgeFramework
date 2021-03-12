using EdgeFramework.UI;

using UnityEngine;
namespace Hotfix
{
    public class LoadingPanelLogic : BaseUI
    {
      
        ProcedureLoadingScene loading;
        LoadingPanel mPanel;

 

        public override void OnUpdate()
        {
          
            if (loading == null) return;
            mPanel.slider.value = loading.LoadingProgress / 100.0f;
            mPanel.text.text = string.Format("{0}%", loading.LoadingProgress);
        }

        public override void OnEnter(object param1 = null, object param2 = null, object param3 = null)
        {
       
            mPanel = UIObj.GetComponent<LoadingPanel>();
            loading = (ProcedureLoadingScene)param1;

            UIObj.SetActive(true);
        }
        public override void OnExit()
        {

            UIObj.SetActive(false);
            loading = null;
        }
        public override void OnPause()
        {
         
        }
        public override void OnResume()
        {
     
        }
        public override void PlayBtnSound()
        {
          
        }

    }
}
