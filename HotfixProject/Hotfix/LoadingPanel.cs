using EdgeFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BaseUI
{
    private Slider slider;
    private Text text;
    ProcedureLoadingScene loading;


    //private void Awake()
    //{
    //    slider = transform.Find("BG/Slider").GetComponent<Slider>();
    //    text = transform.Find("BG/Text").GetComponent<Text>();
    //}
    public override void OnUpdate()
    {
        base.OnUpdate();
        slider.value = loading.LoadingProgress / 100.0f;
        text.text = string.Format("{0}%", loading.LoadingProgress);
    }
    //private void Update()
    //{
    //    slider.value = loading.LoadingProgress / 100.0f;
    //    text.text = string.Format("{0}%", loading.LoadingProgress);
    //}
   
    //public override void OnEnter(params object[] param)
    //{
    //    base.OnEnter(param);
    //    slider = RectTrans.Find("BG/Slider").GetComponent<Slider>();
    //    text = RectTrans.Find("BG/Text").GetComponent<Text>();

    //    loading = (ProcedureLoadingScene)param[0];
    //    UIObj.SetActive(true);
    //}
    public override void OnEnter(object param1 = null, object param2 = null, object param3 = null)
    {
        base.OnEnter(param1, param2, param3);
        slider = RectTrans.Find("BG/Slider").GetComponent<Slider>();
        text = RectTrans.Find("BG/Text").GetComponent<Text>();

        loading = (ProcedureLoadingScene)param1;
        UIObj.SetActive(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        UIObj.SetActive(false);
        loading = null;
    }
    
    public override void PlayBtnSound()
    {
  
    }
}
