using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace EdgeFramework
{
    public class CommonConfirmPanel : BasePanel
    {
        public Text title;
        public Text des;
        public Button confirmBtn;
        public Button cancleBtn;
        public  void Show(params object[] paralist)
        {
            string title=paralist[0] as string;
            string des = paralist[1] as string;
            this.title.text = title;
            this.des.text = des;
            AddButtonClickListener(confirmBtn,()=> 
            {
                UnityEngine.Events.UnityAction confirmAction= paralist[2] as UnityEngine.Events.UnityAction;
                confirmAction();
                Destroy(gameObject);
            });
            AddButtonClickListener(cancleBtn, () =>
            {
                UnityEngine.Events.UnityAction cancle = paralist[3] as UnityEngine.Events.UnityAction;
                cancle();
                Destroy(gameObject);
            });
        }
    }
}