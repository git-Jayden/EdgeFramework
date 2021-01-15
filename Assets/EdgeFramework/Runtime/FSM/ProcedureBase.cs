/****************************************************
	文件：ProcedureBase.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:56   	
	Features：
*****************************************************/
namespace EdgeFramework
{
    public abstract class ProcedureBase : IProcedureState
    {

        private readonly string mStateName;
        private ProcedureFSM mFsm;
        //构造函数 把fsm和状态名字传递过来
        public ProcedureBase(ProcedureFSM _fsm, string _name)
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
        public ProcedureFSM GetFSM()
        {
            return mFsm;
        }
    }
}
