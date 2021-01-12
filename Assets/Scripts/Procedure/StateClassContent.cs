using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateClassContent : ProcedureBase
{


    public StateClassContent(FSM _fsm)
        : base(_fsm, StateDefine.STATE_CLASS_CONTENT)
    {

    }

    public override void OnEnter()
    {
      
    }
    //根据id开始剧情
    private void StartEpisode()
    {
      
    }


    public override void OnUpdate(float step)
    {
     
    }

    //结束后改变状态
    private void ChangeState()
    {
        GetFSM().ChangeState(StateDefine.STATE_GAME_PLAY);
    }


    public override void OnExit()
    {

    }
}
