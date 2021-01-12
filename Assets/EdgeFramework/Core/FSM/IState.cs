

public interface IState
{
    void OnEnter();
    void OnUpdate(float step);
    void OnExit();



    string GetName();

}