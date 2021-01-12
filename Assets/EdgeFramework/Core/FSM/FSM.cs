using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSM
{
    private List<IState> m_list;
    private IState m_currentState;


	public FSM()
    {
        m_list = new List<IState>();
    }
    //添加状态
    public bool addState(IState _state) 
	{
        IState _tmpState = getState(_state.getName());
        if (_tmpState == null)
        {
            m_list.Add(_state);
            return true;
        }
        return false;
    }
    //删除状态
    public bool removeState(IState _state) 
	{       
        IState _tmpState = getState(_state.getName());
        if(_tmpState != null)
        { 
            m_list.Remove(_tmpState);
            return true;
        }
        return false;
    }
     //获取当前状态
    public IState getCurrentState()
    {
        return m_currentState;
    }
    //获取相应状态
    public IState getState(string _name)
	{
        //遍历List里面所有状态取出相应的
        foreach( IState _state in m_list)
        {
            if( _state.getName() == _name )
            {
                return _state;
            }
        }
        return null;
    }
    //改变状态
    public bool changeState(string _name) 
	{
        //要改变的状态
        IState _tmpState = getState(_name);
        if (_tmpState == null)
        {
           return false;
        }
        //当前状态不为空
        if (m_currentState != null)
        {	
            m_currentState.onExit();
        }
        //把要改变的状态赋值给当前状态
        m_currentState = _tmpState;
        m_currentState.onEnter();//启动当前状态的OnEnter
        return true;
    }
    //更新
    public void update(float step)
    {
            if (m_currentState != null)
            {
                m_currentState.onUpdate(step);
            }
    }
    //移除所有状态
    public void removeAllState()
    {
        if (m_currentState != null)
        {
            m_currentState.onExit();
            m_currentState = null;
        }
        m_list.Clear();
    
    }

}