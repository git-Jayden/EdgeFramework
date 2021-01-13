
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//主界面 选择课程状态
public class StateChooseClass : ProcedureBase
{
  
    public StateChooseClass(FSM _fsm)
        : base(_fsm, StateDefine.STATE_CHOOSE_CLASS)
    {

    }

    public override void OnEnter(object[] param)
    {

        Debug.Log("进入流程");
    }
    private void ChangeState()
    {
        GetFSM().ChangeState(StateDefine.STATE_CLASS_CONTENT);
    }


    public override void OnUpdate(float step)
    {

    }
    public override void OnExit()
    {

    }

}