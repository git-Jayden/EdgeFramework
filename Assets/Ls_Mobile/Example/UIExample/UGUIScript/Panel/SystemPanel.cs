using UnityEngine;
using System.Collections;
using EdgeFramework;
using UnityEngine.UI;

public class SystemPanel : BasePanel {
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
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnExit()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnClosePanel()
    {
        UIManager.Instance.PopPanel();
    }
}
