using EdgeFramework;
using EdgeFramework.Res;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureCheckUpdate : ProcedureBase
{
    public ProcedureCheckUpdate(FSM _fsm)
: base(_fsm, StateDefine.PROCEDURE_CHECK_UPDATE)
    {

    }
    public override void OnEnter()
    {
        //打开更新界面检查更新
        LEventSystem.Instance.Send(ShareEvent.OpenHotfixPanel);
    }

    public override void OnExit()
    {
    
    }

    public override void OnUpdate(float step)
    {
 
    }
    public void ChangeState()
    {
        GetFSM().ChangeState(StateDefine.PROCEDURE_LOADING_SCENE);
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
        //TODO代码热更

        text.text = "加载数据表... ...";

        image.fillAmount = 0.7f;
        yield return null;
        text.text = "加载配置... ...";
        LoadConfiger();
        image.fillAmount = 0.9f;
        yield return null;
        text.text = "初始化地图... ...";

        image.fillAmount = 1f;
        yield return null;
    }
    //加载配置表
    void LoadConfiger()
    {
        //加载UI配置表
        SheetManager.Instance.InitUIPanel();
        //加载音效配置表
        SheetManager.Instance.InitSound();
        //加载背景音乐配置表
        SheetManager.Instance.InitMusic();
    }

}
