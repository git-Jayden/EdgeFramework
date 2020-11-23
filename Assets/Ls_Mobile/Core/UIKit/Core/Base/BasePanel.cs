using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EdgeFramework
{
    public class BasePanel : MonoBehaviour
    {
        public GameObject GameObject { get; set; }
        public Transform Transform { get; set; }
        public UIPanelType PanelType { get; set; }
        public bool Resource = false;

        public List<Button> allButton = new List<Button>();
        public List<Toggle> allToggle = new List<Toggle>();

        /// <summary>
        /// 界面被显示出来
        /// </summary>
        public virtual void OnEnter(params object[] paralist)
        {

        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        public virtual void OnPause()
        {

        }

        /// <summary>
        /// 界面继续
        /// </summary>
        public virtual void OnResume(params object[] paralist)
        {

        }
        /// <summary>
        /// 界面不显示,退出这个界面，界面被关系
        /// </summary>
        public virtual void OnExit()
        {

        }
        /// <summary>
        /// 向该页面发送的消息
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="paralist"></param>
        /// <returns></returns>
        public virtual bool OnMessage(UIMsgID msgID, params object[] paralist) { return true; }


        public virtual void OnClose()
        {
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            allButton.Clear();
            allToggle.Clear();
        }
        /// <summary>
        /// 同步替换图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="image"></param>
        /// <param name="setNativeSize"></param>
        /// <returns></returns>
        public bool ChangeImageSprite(string path, Image image, bool setNativeSize = false)
        {
            if (image == null)
                return false;
            Sprite sp = ResouceManager.Instance.LoadResouce<Sprite>(path);
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
        /// <param name="setNativeSize"></param>
        public void ChangeImageSpriteAsync(string path, Image image, bool setNativeSize = false)
        {
            if (image == null) return;
            ResouceManager.Instance.AsyncLoadResouce(path, OnLoadSpriteFinish, LoadResPriority.RES_MIDDLE, image, setNativeSize,true);
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
        /// 移除所有的button事件
        /// </summary>
        public void RemoveAllButtonListener()
        {
            foreach (Button btn in allButton)
            {
                btn.onClick.RemoveAllListeners();
            }
        }
        /// <summary>
        /// 移除所有的toggle事件
        /// </summary>
        public void RemoveAllToggleListener()
        {
            foreach (Toggle toggle in allToggle)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }
        /// <summary>
        /// 添加button事件监听
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="action"></param>
        public void AddButtonClickListener(Button btn, UnityEngine.Events.UnityAction action)
        {
            if (btn != null)
            {
                if (!allButton.Contains(btn))
                {
                    allButton.Add(btn);
                }
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
                btn.onClick.AddListener(BtnPlaySound);
            }
        }
        /// <summary>
        /// 添加button事件监听
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="action"></param>
        public void AddButtonClickListener<T>(Button btn, UnityEngine.Events.UnityAction<T> action,T para)
        {
            if (btn != null)
            {
                if (!allButton.Contains(btn))
                {
                    allButton.Add(btn);
                }
                if (action != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(delegate() { action(para); });
                    btn.onClick.AddListener(BtnPlaySound);
                }
            }
        }
        /// <summary>
        /// 添加toogle事件监听
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="action"></param>
        public void AddToggleClickListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
        {
            if (toggle != null)
            {
                if (!allToggle.Contains(toggle))
                {
                    allToggle.Add(toggle);
                }
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(action);
                toggle.onValueChanged.AddListener(TogglePlaySound);
            }
        }
        /// <summary>
        /// 播放button声音
        /// </summary>
        void BtnPlaySound()
        {

        }
        /// <summary>
        /// 播放toggle声音
        /// </summary>
        /// <param name="isOn"></param>
        void TogglePlaySound(bool isOn)
        {

        }

    }
}
