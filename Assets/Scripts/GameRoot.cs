using EdgeFramework;
using UnityEngine;

public class GameRoot : MonoSingleton<GameRoot>
{
    GameRoot() { }

    public ProcedureManager ProcedureMgr { get; private set; }

    private void Start()
    {
        ProcedureMgr = new ProcedureManager();
        ProcedureMgr.OnInit();
        //TODO登录


        onGameReset();
    }
    private void Update()
    {
        ProcedureMgr.OnUpdate(Time.deltaTime);
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
}