using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ls_Mobile{
    public enum UIMsgID
    {
        None = 0,
    }
    public class UIManager :Singleton<UIManager>
    {

        //UI节点
        public RectTransform uiRoot;
        //窗口节点
        private RectTransform windRoot;
        //UI摄像机
        private Camera uiCamera;
        //EventSystem节点
        private EventSystem eventSystem;
        //屏幕的宽高比
        private float canvasRate = 0;



        private Dictionary<UIPanelType, string> panelPathDict=new Dictionary<UIPanelType, string>();//存储所有面板Prefab的路径
        private Dictionary<UIPanelType, BasePanel> panelDict=new Dictionary<UIPanelType, BasePanel>();//保存所有实例化面板的游戏物体身上的BasePanel组件
        private Stack<BasePanel> panelStack=new Stack<BasePanel>();
        UIManager()
        {
       
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonPath">配置文件路径</param>
        /// <param name="uiRoot">ui父节点</param>
        /// <param name="wndRoot">窗口父节点</param>
        /// <param name="uiCamera">ui摄像机</param>
        /// <param name="eventSystem">事件系统</param>
        public void Init( RectTransform uiRoot, RectTransform wndRoot, Camera uiCamera, EventSystem eventSystem)
        {
    
            this.uiRoot = uiRoot;
            this.windRoot = wndRoot;
            this.uiCamera = uiCamera;
            this.eventSystem = eventSystem;
            this.canvasRate = Screen.height / (uiCamera.orthographicSize * 2);
            ParseUIPanelTypeJson();
        }

        /// <summary>
        /// 显示或者隐藏所有UI
        /// </summary>
        /// <param name="show"></param>
        public void ShowOrHideUI(bool show)
        {
            if (uiRoot != null)
            {
                uiRoot.gameObject.SetActive(show);
            }
        }
        /// <summary>
        /// 设置默认选择对象
        /// </summary>
        /// <param name="obj"></param>
        public void SetNormalSelectObj(GameObject obj)
        {
            if (eventSystem == null)
            {
                eventSystem = EventSystem.current;

            }
            eventSystem.firstSelectedGameObject = obj;
        }

        /// <summary>
        /// 发送消息给窗口
        /// </summary>
        /// <param name="name">窗口名</param>
        /// <param name="msgID">消息ID</param>
        /// <param name="paralist">参数数组</param>
        /// <returns></returns>
        public bool SendMessageToWindow(UIPanelType panelType, UIMsgID msgID = 0, params object[] paralist)
        {
            BasePanel panel = panelDict.TryGet(panelType); //FindWinowByName<Window>(name);
            if (panel != null)
            {
                return panel.OnMessage(msgID, paralist);
            }
            return false;
        }
        /// <summary>
        /// 把某个页面入栈，  把某个页面显示在界面上
        /// </summary>
        /// <param name="panelType"></param>
        public void PushPanel(UIPanelType panelType, bool bTop = true, params object[] paralist)
        {
            //判断一下栈里面是否有页面
            if (panelStack.Count > 0)
            {
                BasePanel topPanel = panelStack.Peek();
                topPanel.OnPause();
            }
            BasePanel panel = GetPanel(panelType);
      
            if (bTop)
            {
                panel.Transform.SetAsLastSibling();
            }
            panel.OnEnter(paralist);
            panelStack.Push(panel);
        }
        /// <summary>
        /// 根据面板类型 得到实例化的面板
        /// </summary>
        /// <returns></returns>
        private BasePanel GetPanel(UIPanelType panelType)
        {
            BasePanel panel = panelDict.TryGet(panelType);

            if (panel == null)
            {
                string path = panelPathDict.TryGet(panelType);

                GameObject instPanel = ObjectManager.Instance.InstantiateObject(path,false,false);//GameObject.Instantiate(Resources.Load(path)) as GameObject;
                if (instPanel == null)
                {
                    Debug.Log("创建窗口Prefab失败:" + instPanel);
                }
                instPanel.transform.SetParent(windRoot, false);

                panelDict.Add(panelType, instPanel.GetComponent<BasePanel>());

                panel = instPanel.GetComponent<BasePanel>();
                panel.GameObject = instPanel;
                panel.Transform = instPanel.transform;
                panel.PanelType = panelType;
                return panel;
            }
            else
            {
                return panel;
            }
        }


        /// <summary>
        /// 出栈 ，把页面从界面上移除
        /// </summary>
        public void PopPanel(bool destroy = false, params object[] paralist)
        {
                if (panelStack == null)
                panelStack = new Stack<BasePanel>();

            if (panelStack.Count <= 0) return;

            //关闭栈顶页面的显示
            BasePanel topPanel = panelStack.Pop();
            topPanel.OnExit();
            topPanel.OnClose();
            panelDict.Remove(topPanel.PanelType);
            if (destroy)
            {
                ObjectManager.Instance.ReleaseObject(topPanel.GameObject, 0, true);
            }
            else
            {
                ObjectManager.Instance.ReleaseObject(topPanel.GameObject, recycleParent: false);
            }
            if (panelStack.Count <= 0) return;
            BasePanel topPanel2 = panelStack.Peek();
            topPanel2.OnResume(paralist);
        }

        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        public void CloseAllPanel()
        {
            if (panelStack == null)
                panelStack = new Stack<BasePanel>();
            if (panelStack.Count <= 0) return;
            foreach (var key in panelDict.Keys)
            {
                ObjectManager.Instance.ReleaseObject(panelDict[key].GameObject, recycleParent: false);
                panelDict[key].OnExit();
                panelDict[key].OnClose();
                panelDict.Remove(key);
                panelStack.Pop();
                if (panelStack.Count <= 0) return;
                BasePanel topPanel2 = panelStack.Peek();
                topPanel2.OnResume();
            }
        }
        /// <summary>
        /// 切换到唯一窗口
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bTop"></param>
        /// <param name="paralist"></param>
        public void SwitchStateByName(UIPanelType panelType, bool bTop = true, params object[] paralist)
        {
            CloseAllPanel();
            PushPanel(panelType, bTop, paralist);
        }

        [Serializable]
        class UIPanelTypeJson
        {
            public List<UIPanelInfo> infoList;
        }
        private void ParseUIPanelTypeJson()
        {
            panelPathDict = new Dictionary<UIPanelType, string>();

            //TextAsset ta = Resources.Load<TextAsset>("UIPanelType");
            TextAsset ta = ResouceManager.Instance.LoadResouce<TextAsset>(ConStr.UIjsonPath);

            UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text);

            foreach (UIPanelInfo info in jsonObject.infoList)
            {
                //Debug.Log(info.panelType);
                panelPathDict.Add(info.panelType, info.path);
            }
        }
        /// <summary>
        /// just for test
        /// </summary>
        public void Test()
        {
            string path;
            panelPathDict.TryGetValue(UIPanelType.Knapsack, out path);
            Debug.Log(path);
        }
    }
}
