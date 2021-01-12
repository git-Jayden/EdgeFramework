
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//主界面 选择课程状态
public class StateChooseClass : StateBase
{
  
    public StateChooseClass(FSM _fsm)
        : base(_fsm, StateDefine.STATE_CHOOSE_CLASS)
    {

    }

    public override void onEnter()
    {

        Debug.Log("进入流程");
    }
    private void ChangeState()
    {
        getFSM().changeState(StateDefine.STATE_CLASS_CONTENT);
    }


    public override void onUpdate(float step)
    {

    }
    public override void onExit()
    {

    }

}