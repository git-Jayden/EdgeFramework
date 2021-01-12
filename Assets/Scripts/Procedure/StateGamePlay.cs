using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGamePlay : ProcedureBase
{
	private FSM m_fsm;
	public StateGamePlay(FSM _fsm)
		: base(_fsm,StateDefine.STATE_GAME_PLAY)
	{
	}

	public override void OnEnter()
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
