using EdgeFramework;
using EdgeFramework.Res;
using EdgeFramework.UI;
using System.Collections;
using UnityEngine;

public class GameRoot : MonoSingleton<GameRoot>
{
    GameRoot() { }

    public ProcedureManager ProcedureMgr { get; private set; }

    private void Awake()
    {
        //资源加载初始化
        ResourcesManager.Instance.Init(GameRoot.Instance);
        ObjectManager.Instance.Init(GameRoot.Instance.transform.Find("RecyclePoolTrs"), GameRoot.Instance.transform.Find("SceneTrs"));
        HotPatchManager.Instance.Init(GameRoot.Instance);
        GameObject.DontDestroyOnLoad(gameObject);
        //UIManager初始化
        UIManager.Instance.OnInit(transform);

    }
    private void Start()
    {
        ProcedureMgr = new ProcedureManager();
        ProcedureMgr.OnInit(this);


        onGameReset();
    }
    private void Update()
    {
        ProcedureMgr.OnUpdate(Time.deltaTime);
        UIManager.Instance.OnUpdate();
    }
    

    /// <summary>
    /// 游戏初始化重置
    /// </summary>
    /// <param name="reset_state">是否重置状态</param>
    public void onGameReset(bool reset_state = true)
    {
        if (reset_state)
        {
            ProcedureMgr.FsmCtrl.ChangeState(StateDefine.PROCEDURE_LAUNCH);
        }
    }
    protected void OnApplicationQuit()
    {
        ILRuntimeManager.Instance.OnDestroy();
        ResourcesManager.Instance.ClearCache();
        Resources.UnloadUnusedAssets();
        Debug.Log("清空编辑器缓存");
    }
}