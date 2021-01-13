
using UnityEngine;
using EdgeFramework;
using UnityEngine.UI;

public class CommomUIManager : MonoBehaviour
{
    [SerializeField]
    private HotfixPanel mHotfixPanel;
    [SerializeField]
    private SelectMessageBox mSelectMessageBox;

    [SerializeField]
    private Text HintLable;

    private float clock;
    void Awake()
    {
        LEventSystem.RegisterEvent(ShareEvent.OpenHotfixPanel, OpenHotfixPanel);

        LEventSystem.RegisterEvent(ShareEvent.OpenSelectMessageBox, OpenSelectMessageBox);
        LEventSystem.RegisterEvent(ShareEvent.ShowHint, ShowHint);

        HintLable.transform.parent.gameObject.SetActive(false);
        mHotfixPanel.gameObject.SetActive(false);
        mSelectMessageBox.gameObject.SetActive(false);
    }
    public void OpenHotfixPanel(int key, params object[] param)
    {
        mHotfixPanel.gameObject.SetActive(true);
        mHotfixPanel.OnInit();
    }

    public void OpenSelectMessageBox(int key, params object[] param)
    {
        mSelectMessageBox.gameObject.SetActive(true);
        mSelectMessageBox.Show(param);
    }

    public void ShowHint(int key, params object[] param)
    {
        string text = (string)param[0];
        float duration = (float)param[1];
        HintLable.transform.parent.gameObject.SetActive(true);
        HintLable.text = text;
        clock = duration;
    }
    void Update()
    {
        if (clock > 0)
        {
            clock -= Time.deltaTime;
            if (clock <= 0)
            {
                HintLable.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
