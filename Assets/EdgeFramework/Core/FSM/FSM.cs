
using System.Collections.Generic;

public class FSM
{
    private List<IState> mList;
    public IState CurState { get; private set; }

    public FSM()
    {
        mList = new List<IState>();
    }
    //添加状态
    public bool AddState(IState _state)
    {
        IState _tmpState = GetState(_state.GetName());
        if (_tmpState == null)
        {
            mList.Add(_state);
            return true;
        }
        return false;
    }
    //删除状态
    public bool RemoveState(IState _state)
    {
        IState _tmpState = GetState(_state.GetName());
        if (_tmpState != null)
        {
            mList.Remove(_tmpState);
            return true;
        }
        return false;
    }

    //获取相应状态
    public IState GetState(string _name)
    {
        //遍历List里面所有状态取出相应的
        foreach (IState _state in mList)
        {
            if (_state.GetName() == _name)
            {
                return _state;
            }
        }
        return null;
    }
    //改变状态
    public bool ChangeState(string _name)
    {
        //要改变的状态
        IState _tmpState = GetState(_name);
        if (_tmpState == null)
        {
            return false;
        }
        //当前状态不为空
        if (CurState != null)
        {
            CurState.OnExit();
        }
        //把要改变的状态赋值给当前状态
        CurState = _tmpState;
        CurState.OnEnter();//启动当前状态的OnEnter
        return true;
    }
    //更新
    public void OnUpdate(float step)
    {
        if (CurState != null)
        {
            CurState.OnUpdate(step);
        }
    }
    //移除所有状态
    public void RemoveAllState()
    {
        if (CurState != null)
        {
            CurState.OnExit();
            CurState = null;
        }
        mList.Clear();

    }

}