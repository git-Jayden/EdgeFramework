using EdgeFramework.UI;

public class MenuPanel : BaseUI
{
    public override void PlayBtnSound()
    {
  
    }

    public override void OnEnter(params object[] param)
    {
        gameObject.SetActive(true);
        base.OnEnter(param);
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
        base.OnExit();
    }
}
