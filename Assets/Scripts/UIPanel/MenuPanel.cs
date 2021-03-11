using EdgeFramework.UI;

public class MenuPanel : BaseUI
{
    public override void PlayBtnSound()
    {
  
    }

    public override void OnEnter(params object[] param)
    {
        UIObj.SetActive(true);
        base.OnEnter(param);
    }
    public override void OnExit()
    {
        UIObj.SetActive(false);
        base.OnExit();
    }
}
