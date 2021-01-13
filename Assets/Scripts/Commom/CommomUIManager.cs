
using UnityEngine;
using EdgeFramework;

public class CommomUIManager : MonoBehaviour
{
    [SerializeField]
    private HotfixPanel mHotfixPanel;
    [SerializeField]
    private SelectMessageBox mSelectMessageBox;
    void Awake()
    {
        LEventSystem.RegisterEvent(ShareEvent.OpenHotfixPanel, OpenHotfixPanel);
        LEventSystem.RegisterEvent(ShareEvent.OpenSelectMessageBox, OpenSelectMessageBox);

        mHotfixPanel.gameObject.SetActive(false);
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
}
