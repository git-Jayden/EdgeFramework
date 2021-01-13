

public interface IState
{
    void OnEnter(object[] param);
    void OnUpdate(float step);
    void OnExit();



    string GetName();

}