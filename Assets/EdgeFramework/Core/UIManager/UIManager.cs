using EdgeFramework.Res;
using EdgeFramework.Sheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EdgeFramework.UI
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
        None = 0
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
        /// UI面板堆栈  打开的窗口列表
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

        public void OnUpdate()
        {
            if (mPanelStack == null || mPanelStack.Count <= 0) return;
     
            foreach (var item in mPanelStack)
            {
           
                if (item.IsHotFix)
                    ILRuntimeManager.Instance.AppDomainCtrl.Invoke(item.HotFixClassName, "OnUpdate", item);
                else
                item.OnUpdate();
            }
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
        /// <typeparam name="T"></typeparam>
        /// <param name="panelType"></param>
        /// <param name="resource">是否是热更加载 </param>
        /// <param name="param"></param>
        /// <returns></returns>
        public BaseUI PushPanel<T>(UIPanelTypeEnum panelType,bool resource=false, object param1 = null, object param2 = null, object param3 = null) where T:BaseUI,new()
        {
            //判断一下栈里面是否有页面
            if (mPanelStack.Count > 0)
            {
                BaseUI curPanel = mPanelStack.Peek();
                if (curPanel.IsHotFix)
                {
                    ILRuntimeManager.Instance.AppDomainCtrl.Invoke(curPanel.HotFixClassName, "OnPause", curPanel);
                }
                else
                {
                    curPanel.OnPause();
                }
          
            }
            mCurSiblingIndex++;
            BaseUI panel = GetPanel<T>(panelType, resource);
            if (panel.IsHotFix)
            {
                ILRuntimeManager.Instance.AppDomainCtrl.Invoke(panel.HotFixClassName, "OnEnter", panel, param1, param2, param3);
            }
            else
            {
                panel.OnEnter(param1, param2, param3);
            }
            panel.IsHotFix = !resource;
            panel.SetSiblingIndex(ViewSiblingIndex.LOW + mCurSiblingIndex);
            mPanelStack.Push(panel);
            return panel;
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
            if (topPanel.IsHotFix)
            {
                ILRuntimeManager.Instance.AppDomainCtrl.Invoke(topPanel.HotFixClassName, "OnExit", topPanel);
            }
            else
            {
                topPanel.OnExit();
            }

      
            if (mPanelDict.ContainsKey(topPanel.PanelType))
            {
                mPanelDict.Remove(topPanel.PanelType);
            }
            if (topPanel.IsHotFix)
            {
                if (destroy)
                {
                    topPanel.ClearPanelBtn();
                    ObjectManager.Instance.ReleaseObject(topPanel.UIObj, 0, true);
                }
                else
                {
                    ObjectManager.Instance.ReleaseObject(topPanel.UIObj, recycleParent: false);
                }
            }
            else
            {
                GameObject.Destroy(topPanel.UIObj);
            }
            topPanel.ClearData();
            topPanel = null;
            if (mPanelStack.Count <= 0) return;
            BaseUI topPanel2 = mPanelStack.Peek();

            if (topPanel2.IsHotFix)
            {
                ILRuntimeManager.Instance.AppDomainCtrl.Invoke(topPanel2.HotFixClassName, "OnResume", topPanel2);
            }
            else
            {
                topPanel2.OnResume();
            }
        }
        /// <summary>
        /// 根据面板类型 得到实例化的面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelType"></param>
        /// <param name="resource">如果是资源本地加载  本地路径文件Resource/Prefab/UI/名字</param>
        /// <returns></returns>
        private BaseUI GetPanel<T>(UIPanelTypeEnum panelType,bool resource=false) where T:BaseUI,new ()
        {
            BaseUI baseUI;
            if (resource)
            {
                baseUI = System.Activator.CreateInstance(typeof(T)) as BaseUI;
            }
            else
            {
                string typeName = System.Enum.GetName(typeof(UIPanelTypeEnum), panelType);
                string hotName = "Hotfix." + typeName + "Logic";
                baseUI = ILRuntimeManager.Instance.AppDomainCtrl.Instantiate<BaseUI>(hotName);
  
                baseUI.HotFixClassName = hotName;
            }

            string path = SheetManager.Instance.GetUIPanelSheet(panelType).Path;
            GameObject go;
            if (resource)
            {
                go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/"+ panelType.ToString())) as GameObject;
            }
            else
            {
                go = ObjectManager.Instance.InstantiateObject(path, false, false);
            }
            if (go == null)
            {
                Debug.Log("创建窗口Prefab失败：" + panelType.ToString());
                return null;
            }
            go.transform.SetParent(WindRoot, false);
            baseUI.Init( go,panelType);
            mPanelDict.Add(panelType, baseUI);
            return baseUI;
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
                if (mPanelDict[key].IsHotFix)
                {
                    mPanelDict[key].ClearPanelBtn();
                    ObjectManager.Instance.ReleaseObject(mPanelDict[key].UIObj, 0, true, false);
                }
                else
                {
                    GameObject.Destroy(mPanelDict[key].UIObj);
                }
                if (mPanelDict[key].IsHotFix)
                {
                    ILRuntimeManager.Instance.AppDomainCtrl.Invoke(mPanelDict[key].HotFixClassName, "OnExit", mPanelDict[key]);
                }
                else
                {
                    mPanelDict[key].OnExit();
                }
                mPanelDict[key].ClearPanelBtn();

                mPanelDict.Remove(key);
                mPanelStack.Pop();
                if (mPanelStack.Count <= 0) return;
                BaseUI topPanel2 = mPanelStack.Peek();

                if (topPanel2.IsHotFix)
                {
                    ILRuntimeManager.Instance.AppDomainCtrl.Invoke(topPanel2.HotFixClassName, "OnResume", topPanel2);
                }
                else
                {
                    topPanel2.OnResume();
                }
            }
        }
        /// <summary>
        /// 强制打开指定界面，删除栈内所有UI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bTop"></param>
        /// <param name="paralist"></param>
        public void SwitchStateByName<T>(UIPanelTypeEnum panelType) where T : BaseUI, new()
        {
            CloseAllPanel();
            PushPanel<T>(panelType);
        }

        /// <summary>
        /// 替换当前Panel
        /// </summary>
        public void Replace<T>(UIPanelTypeEnum panelType) where T:BaseUI,new ()
        {
            PopPanel(true);
            PushPanel<T>(panelType);
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