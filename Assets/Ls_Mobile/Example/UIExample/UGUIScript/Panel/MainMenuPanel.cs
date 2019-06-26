using UnityEngine;
using System.Collections;
using Ls_Mobile;
using UnityEngine.UI;

public class MainMenuPanel : BasePanel {

    private CanvasGroup canvasGroup;
    private Button TaskButton;
    private Button KnapsackButton;
    private Button BattleButton;
    private Button SkillButton;
    private Button ShopButton;
    private Button SystemButton;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        TaskButton = transform.Find("IconPanel/TaskButton").GetComponent<Button>();
        KnapsackButton = transform.Find("IconPanel/KnapsackButton").GetComponent<Button>();
        BattleButton = transform.Find("IconPanel/BattleButton").GetComponent<Button>();
        SkillButton = transform.Find("IconPanel/SkillButton").GetComponent<Button>();
        ShopButton = transform.Find("IconPanel/ShopButton").GetComponent<Button>();
        SystemButton = transform.Find("IconPanel/SystemButton").GetComponent<Button>();
    }
    public override void OnEnter(params object[] paralist)
    {
        AddButtonClickListener(TaskButton, OnPushPanel, UIPanelType.Task);
        AddButtonClickListener(KnapsackButton, OnPushPanel, UIPanelType.Knapsack);
        AddButtonClickListener(SkillButton, OnPushPanel, UIPanelType.Skill);
        AddButtonClickListener(ShopButton, OnPushPanel, UIPanelType.Shop);
        AddButtonClickListener(SystemButton, OnPushPanel, UIPanelType.System);

    }

    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;//当弹出新的面板的时候，让主菜单面板 不再和鼠标交互
    }
    public override void OnResume(params object[] paralist)
    {
        canvasGroup.blocksRaycasts = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UIManager.Instance.PopPanel();
        }
    }
    public override void OnExit()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnPushPanel(UIPanelType tp)
    {
        UIManager.Instance.PushPanel(tp);
    }

}
