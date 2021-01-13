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
            //UnityEngine.Events.UnityAction confirmAction = paralist[2] as UnityEngine.Events.UnityAction;
            //confirmAction();
            callback.Call(this, true);
            gameObject.SetActive(false);
            //TODO sound
        });
        cancleBtn.onClick.AddListener(() =>
        {
            //UnityEngine.Events.UnityAction cancle = paralist[3] as UnityEngine.Events.UnityAction;
            //cancle();
            callback.Call(this, false);
            gameObject.SetActive(false);
            //TODO sound
        });

    }
}
