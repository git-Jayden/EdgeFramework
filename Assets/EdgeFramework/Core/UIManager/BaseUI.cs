using EdgeFramework.Res;
using EdgeFramework.Sheet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EdgeFramework.UI
{
    public abstract class BaseUI
    {
        [HideInInspector]
        public GameObject UIObj { get;private set; }
        [HideInInspector]
        public RectTransform RectTrans { get;private set; }
        [HideInInspector]
        public UIPanelTypeEnum PanelType { get;private set; }

        private List<Button> mAllButton = new List<Button>();
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(GameObject gameObject, UIPanelTypeEnum panelType)
        {
            UIObj = gameObject;
            RectTrans = UIObj.GetComponent<RectTransform>();
            PanelType = panelType;
        }

        /// <summary>
        /// 界面被显示出来
        /// </summary>
        public virtual void OnEnter(params object[] param) {  }
        public virtual void OnUpdate() { }
        /// <summary>
        /// 界面暂停
        /// </summary>
        public virtual void OnPause(){ }

        /// <summary>
        /// 界面继续
        /// </summary>
        public virtual void OnResume(){  }

        /// <summary>
        /// 界面不显示,退出这个界面，界面关闭
        /// </summary>
        public virtual void OnExit(){  }

        /// <summary>
        /// 向该页面发送的消息
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="paralist"></param>
        /// <returns></returns>
        public virtual bool OnMessage(UIMsgID msgID, params object[] paralist) { return true; }


        /// <summary>
        /// 移除该面板所有的button事件
        /// </summary>
        public  void ClearPanelBtn()
        {
            RemoveAllButtonListener();
            mAllButton.Clear();
        }
   
        private void RemoveAllButtonListener()
        {
            foreach (Button btn in mAllButton)
            {
                btn.onClick.RemoveAllListeners();
            }
        }
        /// <summary>
        /// 添加button事件监听
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="action"></param>
        public void AddBtnClickListener(Button btn, UnityEngine.Events.UnityAction action)
        {
            if (btn != null)
            {
                if (!mAllButton.Contains(btn))
                {
                    mAllButton.Add(btn);
                }
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
                btn.onClick.AddListener(PlayBtnSound);
            }
        }
        /// <summary>
        /// 添加button事件监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="btn">Button</param>
        /// <param name="action">Button事件</param>
        /// <param name="para">参数</param>
        public void AddBtnClickListener<T>(Button btn, UnityEngine.Events.UnityAction<T> action, T para)
        {
            if (btn != null)
            {
                if (!mAllButton.Contains(btn))
                {
                    mAllButton.Add(btn);
                }
                if (action != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(PlayBtnSound);
                    btn.onClick.AddListener(delegate () { action(para); });
                }
            }
        }

        /// <summary>
        /// 同步替换图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="image"></param>
        /// <param name="setNativeSize">是否设置设置为图片原大小，不拉升</param>
        /// <returns></returns>
        public bool ChangeImageSprite(string path, Image image, bool setNativeSize = false)
        {
            if (image == null)
                return false;
            Sprite sp = ResourcesManager.Instance.LoadResouce<Sprite>(path);
            if (sp != null)
            {
                if (image.sprite != null)
                    image.sprite = null;
                image.sprite = sp;
                if (setNativeSize)
                {
                    image.SetNativeSize();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 异步替换图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="image"></param>
        /// <param name="setNativeSize">是否设置设置为图片原大小，不拉升</param>
        public void ChangeImageSpriteAsync(string path, Image image, bool setNativeSize = false)
        {
            if (image == null) return;
            ResourcesManager.Instance.AsyncLoadResouce(path, OnLoadSpriteFinish, LoadResPriority.RES_MIDDLE, image, setNativeSize, true);
        }
        /// <summary>
        /// 图片加载完成
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        void OnLoadSpriteFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
        {
            if (obj != null)
            {
                Sprite sp = obj as Sprite;
                Image image = param1 as Image;
                bool setNativeSize = (bool)param2;
                if (image.sprite != null)
                    image.sprite = null;
                image.sprite = sp;
                if (setNativeSize)
                {
                    image.SetNativeSize();
                }
            }
        }
        /// <summary>
        /// 设置层级
        /// </summary>
        public virtual void SetSiblingIndex(int siblingIndex)
        {
            if (UIObj != null)
            {
                UIObj.transform.SetSiblingIndex(siblingIndex);
            }
        }
        /// <summary>
        /// 播放button声音
        /// </summary>
        public abstract void PlayBtnSound();

    }
}
