using EdgeFramework;
using EdgeFramework.Res;
using EdgeFramework.Sheet;
using EdgeFramework.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 加载场景
/// </summary>
public class ProcedureLoadingScene : ProcedureBase
{
    //切换场景进度条
    public int LoadingProgress = 0;
    //当前场景名
    public string CurrentMapName { get; set; }
    //场景是否加载完成
    public bool AlreadyLoadScene { get; set; }
    private string NextProcedure;

    public ProcedureLoadingScene(ProcedureFSM _fsm)
  : base(_fsm, StateDefine.PROCEDURE_LOADING_SCENE)
    {

    }
    public override void OnEnter(object[] param)
    {
        string sceneName = (string)param[0];
        NextProcedure = (string)param[1];
        //加载场景
        LoadingProgress = 0;
        GetFSM().Mono.StartCoroutine(LoadSceneAsync(sceneName));
        UIManager.Instance.PushPanel<LoadingPanel>(UIPanelTypeEnum.LoadingPanel, this);

    }

    public override void OnExit()
    {
        UIManager.Instance.PopPanel();
    }

    public override void OnUpdate(float step)
    {

    }


    IEnumerator LoadSceneAsync(string name)
    {
        LoadSceneEnter();
        ClearCache();
        AlreadyLoadScene = false;
        AsyncOperation unLoadScene = SceneManager.LoadSceneAsync(SceneName.EMPTY_SCENE, LoadSceneMode.Single);
        while (unLoadScene != null && !unLoadScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        LoadingProgress = 0;
        int targetProgress = 0;
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(name);
        if (asyncScene != null && !asyncScene.isDone)
        {
            asyncScene.allowSceneActivation = false;
            while (asyncScene.progress < 0.9f)
            {
                targetProgress = (int)asyncScene.progress * 100;
                yield return new WaitForEndOfFrame();
                //平滑过渡
                while (LoadingProgress < targetProgress)
                {
                    ++LoadingProgress;
                    yield return new WaitForEndOfFrame();
                }
            }
            CurrentMapName = name;
            //自行加载剩余的10%
            targetProgress = 100;
            while (LoadingProgress < targetProgress - 2)
            {
                ++LoadingProgress;
                yield return new WaitForEndOfFrame();
            }
            LoadingProgress = 100;
            asyncScene.allowSceneActivation = true;
            AlreadyLoadScene = true;
            LoadSceneOver(name);
        }
        yield return null;
    }
    //开始加载场景
    private void LoadSceneEnter()
    {

    }
    //加载场景完成
    private void LoadSceneOver(string name)
    {
        //设置各种场景环境,可以根据配表来TODO
        GetFSM().ChangeState(NextProcedure);
    }
    private void ClearCache()
    {
        ObjectManager.Instance.ClearCache();
        ResourcesManager.Instance.ClearCache();
    }

}
