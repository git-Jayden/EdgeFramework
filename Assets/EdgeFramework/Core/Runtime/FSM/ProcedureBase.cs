
public abstract class ProcedureBase : IState {

    private readonly string mStateName;
	private FSM mFsm;
    //构造函数 把fsm和状态名字传递过来
	public ProcedureBase(FSM _fsm,string _name)
    {
        mFsm = _fsm; 
        mStateName = _name;
    }
    //获取状态名字
    public string GetName()
    {
        return mStateName;
    } 

    public abstract void OnEnter(object[] param);
    public abstract void OnUpdate(float step);
    public abstract void OnExit();
    //获取状态
	public FSM GetFSM()
    {
        return mFsm;
    }
}
