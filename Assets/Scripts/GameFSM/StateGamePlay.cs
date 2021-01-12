using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGamePlay : StateBase
{
	private FSM m_fsm;
	public StateGamePlay(FSM _fsm)
		: base(_fsm,StateDefine.STATE_GAME_PLAY)
	{
	}

	public override void onEnter()
	{
	
	}

    public override void onUpdate(float step)
	{
	
	}

    //改变状态到结束Game
	private void onGameFinish()
	{
		getFSM ().changeState (StateDefine.STATE_END_GAME);
	}

	public override void onExit()
	{
		

	}
}
