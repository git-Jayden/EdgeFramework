using EdgeFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BaseUI
{
    private Slider slider;
    private Text text;
    ProcedureLoadingScene loading;

    public override void OnUpdate()
    {
        base.OnUpdate();
        slider.value = loading.LoadingProgress / 100.0f;
        text.text = string.Format("{0}%", loading.LoadingProgress);
        Debug.Log(loading.LoadingProgress);
    }
    public  void OnCreate()
    {
        slider = UIObj.transform.Find("BG/Slider").GetComponent<Slider>();
        text = UIObj.transform.Find("BG/Text").GetComponent<Text>();
    }
    public override void OnEnter(params object[] param)
    {
        base.OnEnter(param);
        if (slider == null)
            OnCreate();
        loading = (ProcedureLoadingScene)param[0];
        UIObj.SetActive(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        UIObj.SetActive(false);
    }
    
    public override void PlayBtnSound()
    {
  
    }
}
