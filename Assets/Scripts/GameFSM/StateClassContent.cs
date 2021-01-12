using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateClassContent : StateBase
{


    public StateClassContent(FSM _fsm)
        : base(_fsm, StateDefine.STATE_CLASS_CONTENT)
    {

    }

    public override void onEnter()
    {
      
    }
    //根据id开始剧情
    private void StartEpisode()
    {
      
    }


    public override void onUpdate(float step)
    {
     
    }

    //结束后改变状态
    private void ChangeState()
    {
        getFSM().changeState(StateDefine.STATE_GAME_PLAY);
    }


    public override void onExit()
    {

    }
}
