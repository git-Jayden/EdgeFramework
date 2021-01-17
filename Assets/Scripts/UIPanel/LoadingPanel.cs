using EdgeFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BaseUI
{
    private Slider slider;
    private Text text;
    ProcedureLoadingScene loading;

    private void Awake()
    {
        slider = transform.Find("BG/Slider").GetComponent<Slider>();
        text = transform.Find("BG/Text").GetComponent<Text>();
    }
    private void Update()
    {
        slider.value = loading.LoadingProgress / 100.0f;
        text.text = string.Format("{0}%", loading.LoadingProgress);
    }
   
    public override void OnEnter(params object[] param)
    {
        base.OnEnter(param);
        loading = (ProcedureLoadingScene)param[0];
        gameObject.SetActive(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
        loading = null;
    }
    
    public override void PlayBtnSound()
    {
  
    }
}
