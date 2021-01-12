using UnityEngine;
using System.Collections;

public abstract class StateBase : IState {

    private readonly string m_stateName;
	private FSM m_fsm;
    //构造函数 把fsm和状态名字传递过来
	public StateBase(FSM _fsm,string name)
    {
        m_fsm = _fsm; 
        m_stateName = name;
    }
    //获取状态名字
    public string getName()
    {
        return m_stateName;
    } 
    // Use this for initialization
    public abstract void onEnter();
    public abstract void onUpdate(float step);
    public abstract void onExit();
    //获取状态
	public FSM getFSM()
    {
        return m_fsm;
    }
}
