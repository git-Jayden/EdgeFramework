/****************************************************
	文件：IState.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:54   	
	Features：
*****************************************************/
namespace EdgeFramework
{
    public interface IProcedureState
    {
        void OnEnter(object[] param);
        void OnUpdate(float step);
        void OnExit();



        string GetName();

    }
}