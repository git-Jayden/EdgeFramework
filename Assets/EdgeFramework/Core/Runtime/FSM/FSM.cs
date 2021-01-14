/****************************************************
	文件：FSM.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:54   	
	Features：
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    public class FSM
    {
        private List<IState> mList;
        public IState CurState { get; private set; }
        public MonoBehaviour Mono { get; private set; }
        public FSM(MonoBehaviour mono)
        {
            mList = new List<IState>();
            Mono = mono;

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
        public bool ChangeState(string _name, params object[] param)
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
            CurState.OnEnter(param);//启动当前状态的OnEnter
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
}