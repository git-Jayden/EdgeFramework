using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EdgeFramework;

public class LoadingPanel : BasePanel
{
    private Slider slider;
    private Text text;
    private string sceneName;
    private void Awake()
    {
        slider = transform.Find("BG/Slider").GetComponent<Slider>();
        text = transform.Find("Text").GetComponent<Text>();
    }
    public override void OnEnter(params object[] paralist)
    {
        if (paralist != null)
            sceneName = (string)paralist[0];
    }
    private void Update()
    {

        slider.value = GameMapManager.LoadingProgress / 100.0f;
        text.text = string.Format("{0}%", GameMapManager.LoadingProgress);
        if (GameMapManager.LoadingProgress >= 100)
        {
            LoadOtherScene();
        }
    }
    /// <summary>
    /// 加载对应场景第一个UI
    /// </summary>
    public void LoadOtherScene()
    {
        //根据场景名字打开对应第一个界面
        if (sceneName == Constants.MenuScene)
        {
            UIManager.Instance.PopPanel();
            UIManager.Instance.PushPanel(UIPanelType.MainMenu);
        }

    }
}
