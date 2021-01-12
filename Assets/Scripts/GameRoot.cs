using EdgeFramework;
using UnityEngine;

public class GameRoot : MonoSingleton<GameRoot>
{
    GameRoot() { }
    public FSM m_fsm;

    private void Start()
    {
        m_fsm = new FSM();
        m_fsm.addState(new StateChooseClass(m_fsm));
        m_fsm.addState(new StateClassContent(m_fsm));
        m_fsm.addState(new StateGamePlay(m_fsm));
        m_fsm.addState(new StateEndGame(m_fsm));
        
        //TODO登录


        onGameReset();
    }

    /// <summary>
    /// 游戏初始化 重置
    /// </summary>
    /// <param name="reset_state">是否重置状态</param>
    public void onGameReset(bool reset_state = true)
    {
        if (reset_state)
        {
            m_fsm.changeState(StateDefine.STATE_CHOOSE_CLASS);
        }
    }
}
