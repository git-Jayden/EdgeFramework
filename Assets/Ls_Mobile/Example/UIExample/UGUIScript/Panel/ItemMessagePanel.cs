using UnityEngine;
using System.Collections;
using DG.Tweening;
using Ls_Mobile;
using UnityEngine.UI;

public class ItemMessagePanel : BasePanel {
    private CanvasGroup canvasGroup;
    private   Button CloseButton;
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

        transform.localScale = Vector3.zero;
        transform.DOScale(1, .5f);
    }

    public override void OnExit()
    {
        //canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        transform.DOScale(0, .5f).OnComplete(() => canvasGroup.alpha = 0);
    }

    public void OnClosePanel()
    {
        UIManager.Instance.PopPanel();
    }
}
