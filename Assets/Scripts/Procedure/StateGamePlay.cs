using EdgeFramework;


public class StateGamePlay : ProcedureBase
{
	private ProcedureFSM m_fsm;
	public StateGamePlay(ProcedureFSM _fsm)
		: base(_fsm,StateDefine.STATE_GAME_PLAY)
	{
	}

	public override void OnEnter(object[] param)
	{
	
	}

    public override void OnUpdate(float step)
	{
	
	}

    //改变状态到结束Game
	private void onGameFinish()
	{
		GetFSM ().ChangeState (StateDefine.STATE_END_GAME);
	}

	public override void OnExit()
	{
		

	}
}
