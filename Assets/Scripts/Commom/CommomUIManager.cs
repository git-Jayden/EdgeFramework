/****************************************************
	文件：CommomUIManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:29   	
	Features：
*****************************************************/
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
        GameEventSystem.RegisterEvent(ShareEvent.OpenHotfixPanel, OpenHotfixPanel);

        GameEventSystem.RegisterEvent(ShareEvent.OpenSelectMessageBox, OpenSelectMessageBox);
        GameEventSystem.RegisterEvent(ShareEvent.ShowHint, ShowHint);

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
