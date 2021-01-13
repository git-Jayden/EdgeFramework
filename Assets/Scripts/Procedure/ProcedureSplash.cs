

public class ProcedureSplash : ProcedureBase
{
    public ProcedureSplash(FSM _fsm)
  : base(_fsm, StateDefine.PROCEDURE_SPLASH)
    {

    }
    public override void OnEnter()
    {
       
     
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate(float step)
    {
        // TODO: 这里可以播放一个 Splash 动画
        GetFSM().ChangeState(StateDefine.PROCEDURE_CHECK_UPDATE);
 
    }
    
}
