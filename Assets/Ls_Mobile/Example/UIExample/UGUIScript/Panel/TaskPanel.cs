using UnityEngine;
using System.Collections;
using DG.Tweening;
using EdgeFramework;
using UnityEngine.UI;

public class TaskPanel : BasePanel {

    private CanvasGroup canvasGroup;
    private Button CloseButton;
    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        CloseButton = transform.Find("CloseButton").GetComponent<Button>();


    }

    public override void OnEnter(params object[] paralist)
    {
        AddButtonClickListener(CloseButton, OnClosePanel);
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOFade(1, .5f);
    }

    /// <summary>
    /// 处理页面的 关闭
    /// </summary>
    public override void OnExit()
    {
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOFade(0, .5f);
    }

    public void OnClosePanel()
    {
        UIManager.Instance.PopPanel();
    }
}
