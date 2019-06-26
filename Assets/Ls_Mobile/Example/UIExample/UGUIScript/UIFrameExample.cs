using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ls_Mobile;
using UnityEngine.EventSystems;

public class UIFrameExample : MonoSimplify<UIFrameExample>
{

    protected override void Awake()
    {
        base.Awake();
      
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        UIManager.Instance.Init(
      transform.Find("UIRoot") as RectTransform,
      transform.Find("UIRoot/WindRoot") as RectTransform,
      transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
      transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>());


        GameMapManager.Instance.LoadScene(ConStr.MenuScene, UIPanelType.Loading);
       // UIManager.Instance.PushPanel(UIPanelType.MainMenu);

    }
    //void RegisterUI()
    //{
    //    UIManager0.Instance.Regist<MenuUi>("MenuPanel.prefab");
    //}
    // Update is called once per frame
    void Update()
    {

    }

}
