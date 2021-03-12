
/****************************************************
	文件：SelectMessageBox.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:29   	
	Features：
*****************************************************/
using UnityEngine;
using UnityEngine.UI;

public class CallbackSelect
{
    public delegate void Fun(SelectMessageBox box, bool b, object[] objs);

    private Fun fun;
    private object[] ps;

    public CallbackSelect(Fun fun, params object[] objs)
    {
        this.fun = fun;
        this.ps = objs;
    }

    internal void Call(SelectMessageBox box, bool b)
    {
        if (fun != null)
        {
            fun(box, b, ps);
        }
    }
}

public class SelectMessageBox : MonoBehaviour
{
    public Text title;
    public Text des;
    public Button confirmBtn;
    public Button cancleBtn;

    private CallbackSelect callback;

    public void Show(params object[] paralist)
    {
        string title = paralist[0]as string;
        string des = paralist[1] as string;
        callback = (CallbackSelect)paralist[2];
        this.title.text = title;
        this.des.text = des;
        confirmBtn.onClick.AddListener(() =>
        {
            callback.Call(this, true);
            gameObject.SetActive(false);
            //AudioPlayer.Instance.PlaySound(SoundEnum.EnterClick);
        });
        cancleBtn.onClick.AddListener(() =>
        {

            callback.Call(this, false);
            gameObject.SetActive(false);
            //AudioPlayer.Instance.PlaySound(SoundEnum.EnterClick);
        });

    }
}
