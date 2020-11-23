using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EdgeFramework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIFrameExample : MonoBehaviour, ISingleton
{
    public static UIFrameExample Instance
    {
        get { return MonoSingletonProperty<UIFrameExample>.Instance; }
    }
    public void OnSingletonInit()
    {

    }
    protected void Awake()
    {

        GameObject.DontDestroyOnLoad(gameObject);

        ResouceManager.Instance.Init(this);
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs"));
        HotPatchManager.Instance.Init(this);
    }
    // Start is called before the first frame update
    protected void Start()
    {
        UIManager.Instance.Init(transform);
        UIManager.Instance.PushPanel(UIPanelType.HotFixPanel, resource: true);

        //跳转场景
        //GameMapManager.Instance.LoadScene(ConStr.MenuScene, UIPanelType.Loading);
        //UIManager.Instance.PushPanel(UIPanelType.Hotfix);

    }

    public IEnumerator StartGame(Image image, Text text)
    {
        image.fillAmount = 0;
        yield return null;
        text.text = "加载本地数据... ...";
        AssetBundleManager.Instance.LoadAssetBundleConfig();
        image.fillAmount = 0.1f;
        yield return null;
        text.text = "加载dll... ...";
        //ILRuntimeManager.Instance.Init();
        //image.fillAmount = 0.2f;
        //yield return null;
        text.text = "加载数据表... ...";
        LoadConfiger();
        image.fillAmount = 0.7f;
        yield return null;
        text.text = "加载配置... ...";
        UIManager.Instance.ParseUIPanelTypeJson();
          image.fillAmount = 0.9f;
        yield return null;
        text.text = "初始化地图... ...";
        GameMapManager.Instance.Init(this);
        image.fillAmount = 1f;
        yield return null;
    }
    public static void OpenCommonConfirm(string title, string str, UnityEngine.Events.UnityAction confirmAction, UnityEngine.Events.UnityAction cancleAction)
    {
        GameObject commonObj = GameObject.Instantiate(Resources.Load<GameObject>("CommonConfirm")) as GameObject;
        commonObj.transform.SetParent(UIManager.Instance.windRoot, false);
        CommonConfirmPanel commonitem = commonObj.GetComponent<CommonConfirmPanel>();
        commonitem.Show(title, str, confirmAction, cancleAction);
    }
    //加载配置表
    void LoadConfiger()
    {
        //ConfigManager.Instance.LoadData<MonsterData>(CFG.TABLE_MONSTER);
        //ConfigManager.Instance.LoadData<BuffData>(CFG.TABLE_BUFF);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnApplicationQuit()
    {
        ResouceManager.Instance.ClearCache();
        Resources.UnloadUnusedAssets();
        Debug.Log("清空编辑器缓存");
    }
}
