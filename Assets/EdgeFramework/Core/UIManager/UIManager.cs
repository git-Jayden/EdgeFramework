using EdgeFramework.Sheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EdgeFramework
{
    /// <summary>
    /// 视图UI层级定义
    /// </summary>
    public class ViewSiblingIndex
    {
        /// <summary>
        /// 正常UI层级
        /// </summary>
        public const int LOW = 0;
        /// <summary>
        /// 公告浮框等
        /// </summary>
        public const int MIDDLE = 100;
        /// <summary>
        /// UICommon、Loading等全屏覆盖UI
        /// </summary>
        public const int HIGH = 200;
        /// <summary>
        /// 最高层级
        /// </summary>
        public const int MAX = 999;
    }

    /// <summary>
    /// UI消息ID
    /// </summary>
    public enum UIMsgID
    {
        None = 0,
    }
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// UI节点
        /// </summary>
        public RectTransform UIRoot { get; private set; }
        /// <summary>
        /// 窗口节点
        /// </summary>
        public RectTransform WindRoot { get; private set; }
        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICamera { get; private set; }
        /// <summary>
        /// EventSystem节点
        /// </summary>
        public EventSystem EventSys { get; private set; }
        /// <summary>
        /// 屏幕的宽高比
        /// </summary>
        private float mCanvasRate = 0;


        /// <summary>
        /// 保存所有实例化面板的游戏物体身上的BasePanel组件
        /// </summary>
        private Dictionary<UIPanelTypeEnum, BaseUI> mPanelDict = new Dictionary<UIPanelTypeEnum, BaseUI>();
        /// <summary>
        /// UI面板堆栈
        /// </summary>
        private Stack<BaseUI> mPanelStack = new Stack<BaseUI>();
        /// <summary>
        /// 当前层级
        /// </summary>
        private int mCurSiblingIndex = 0;

        UIManager(){ }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="trans"></param>
        public void OnInit(Transform trans)
        {
            this.UIRoot = trans.Find("UIRoot") as RectTransform;
            this.WindRoot = trans.Find("UIRoot/WindRoot") as RectTransform;
            this.UICamera = trans.Find("UIRoot/UICamera").GetComponent<Camera>();
            this.EventSys= trans.Find("UIRoot/EventSystem").GetComponent<EventSystem>();
            this.mCanvasRate = Screen.height / (UICamera.orthographicSize * 2);
        }

        /// <summary>
        /// 显示或者隐藏所有UI
        /// </summary>
        /// <param name="show"></param>
        public void ShowOrHideUI(bool show)
        {
            if (UIRoot != null)
            {
                UIRoot.gameObject.SetActive(show);
            }
        }
        /// <summary>
        /// 设置默认选择对象
        /// </summary>
        /// <param name="obj"></param>
        public void SetNormalSelectObj(GameObject obj)
        {
            if (EventSys == null)
            {
                EventSys = EventSystem.current;

            }
            EventSys.firstSelectedGameObject = obj;
        }
        /// <summary>
        /// 发送消息给窗口
        /// </summary>
        /// <param name="name">窗口名</param>
        /// <param name="msgID">消息ID</param>
        /// <param name="paralist">参数数组</param>
        /// <returns></returns>
        public bool SendMessageToWindow(UIPanelTypeEnum panelType, UIMsgID msgID = 0, params object[] paralist)
        {
            BaseUI panel = mPanelDict.TryGet(panelType);
            if (panel != null)
            {
                return panel.OnMessage(msgID, paralist);
            }
            return false;
        }
        /// <summary>
        /// 把某个页面入栈，把某个页面显示在界面 上
        /// </summary>
        /// <param name="panelType"></param>
        public void PushPanel<T>(UIPanelTypeEnum panelType, params object[] paralist) where T : BaseUI, new()
        {
            //判断一下栈里面是否有页面
            if (mPanelStack.Count > 0)
            {
                BaseUI curPanel = mPanelStack.Peek();
                curPanel.OnPause();
            }
            mCurSiblingIndex++;
            BaseUI panel = GetPanel<T>(panelType, paralist);
            panel.OnEnter();
            panel.SetSiblingIndex(ViewSiblingIndex.LOW + mCurSiblingIndex);
            mPanelStack.Push(panel);
        }

        /// <summary>
        /// 退出栈顶View
        /// </summary>
        public void PopPanel(bool destroy = false)
        {
            if (mPanelStack == null)
                mPanelStack = new Stack<BaseUI>();
            if (mPanelStack.Count <= 0) return;
            mCurSiblingIndex--;
            //关闭栈顶页面的显示
            BaseUI topPanel = mPanelStack.Pop();
            topPanel.OnExit();
            topPanel.ClearPanelBtn();
            if (mPanelDict.ContainsKey(topPanel.PanelType))
            {
                mPanelDict.Remove(topPanel.PanelType);
            }
            if (destroy)
            {
                ObjectManager.Instance.ReleaseObject(topPanel.Go, 0, true);
            }
            else
            {
                ObjectManager.Instance.ReleaseObject(topPanel.Go, recycleParent: false);
            }
            topPanel = null;
            if (mPanelStack.Count <= 0) return;
            BaseUI topPanel2 = mPanelStack.Peek();
            topPanel2.OnResume();
        }
        /// <summary>
        /// 根据面板类型 得到实例化的面板
        /// </summary>
        /// <returns></returns>
        private BaseUI GetPanel<T>(UIPanelTypeEnum panelType, params object[] paralist) where T : BaseUI, new()
        {
            string path = SheetManager.Instance.GetUIPanel(panelType).Path;
            GameObject go =ObjectManager.Instance.InstantiateObject(path);
            go.transform.SetParent(WindRoot, false);

            var view = new T();
            view.Create(go, panelType, paralist);
            mPanelDict.Add(panelType, view);
          
            return view;
        }

        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        public void CloseAllPanel()
        {
            if (mPanelStack == null)
                mPanelStack = new Stack<BaseUI>();
            if (mPanelStack.Count <= 0) return;
            foreach (var key in mPanelDict.Keys)
            {
                ObjectManager.Instance.ReleaseObject(mPanelDict[key].Go,0,true, false);
                mPanelDict[key].OnExit();
                mPanelDict[key].ClearPanelBtn();
                mPanelDict.Remove(key);
                mPanelStack.Pop();
                if (mPanelStack.Count <= 0) return;
                BaseUI topPanel2 = mPanelStack.Peek();
                topPanel2.OnResume();
            }
        }
        /// <summary>
        /// 强制打开指定界面，删除栈内所有UI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bTop"></param>
        /// <param name="paralist"></param>
        public void SwitchStateByName<T>(UIPanelTypeEnum panelType, params object[] paralist) where T : BaseUI, new()
        {
            CloseAllPanel();
            PushPanel<T>(panelType, paralist);
        }

        /// <summary>
        /// 替换当前Panel
        /// </summary>
        public void Replace<T>(UIPanelTypeEnum panelType, params object[] data) where T : BaseUI, new()
        {
            PopPanel(true);
            PushPanel<T>(panelType, data);
        }
        /// <summary>
        /// 按指定层级添加UI
        /// 直接添加的View不进入Stack管理
        /// </summary>
        public void Add<T>(UIPanelTypeEnum panelType, int siblingIndex) where T : BaseUI, new()
        {
            var view = GetPanel<T>(panelType);
            view.SetSiblingIndex(siblingIndex);
        }
    }

}