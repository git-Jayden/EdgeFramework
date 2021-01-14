/****************************************************
	文件：CustomScrollBase.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 15:39   	
	Features：自定义滚动层基类
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace EdgeFramework.UGUIEx
{
    public class CustomScrollBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public virtual void OnBeginDrag(PointerEventData eventData) { }

        public virtual void OnDrag(PointerEventData eventData) { }

        public virtual void OnEndDrag(PointerEventData eventData) { }
    }
}
