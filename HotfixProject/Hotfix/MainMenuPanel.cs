using EdgeFramework.UI;

public class MainMenuPanel : BaseUI
{
    public override void PlayBtnSound()
    {
  
    }


    public override void OnEnter(object param1 = null, object param2 = null, object param3 = null)
    {
        UIObj.SetActive(true);
        base.OnEnter(param1, param2, param3);
    }
    public override void OnExit()
    {
        UIObj.SetActive(false);
        base.OnExit();
    }
}
