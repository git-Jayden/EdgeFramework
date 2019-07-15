using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ls_Mobile {
    public class HotfixPanel :BasePanel
    {
        public Image image;
        public Text text;
        public Text sliderTopTex;
        [Header("热更信息界面")]
        public GameObject infoPanel;
        public Text hotContentTex;

        private float sumTime = 0;
        private void Awake()
        {
            sumTime = 0;
            image.fillAmount = 0;
            text.text = string.Format("{0:F}M/S",0);
            HotPatchManager.Instance.ServerInfoError += ServerInfoError;
            HotPatchManager.Instance.ItemError += ItemError;

#if UNITY_EDITOR
            StartOnFinish();
#else

            if (HotPatchManager.Instance.ComputeUnPackFile())
            {
                sliderTopTex.text = "解压中";
                HotPatchManager.Instance.StartUnackFile(()=> 
                {
                    sumTime = 0;
                    Hotfix();
                });
            }
            else
            {
                Hotfix();
            }
#endif
        }
        void Hotfix()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //提示网络错误,检测网络连接是否正常
                UIFrameExample.OpenCommonConfirm("网络连接失败","网络连接失败，请检查网络连接是否正常?",()=>{ Application.Quit(); }, () => { Application.Quit(); });

            }
            else
            {
                CheckVersion();
            }
        }
        void CheckVersion()
        {
            //检查该版本是否有热更
            HotPatchManager.Instance.CheckVersion((hot)=> 
            {
                //如果有热更
                if (hot)
                {
                    //提示玩家是否确定热更下载
                    UIFrameExample.OpenCommonConfirm("热更确定",string.Format( "当前版本为{0},有{1:F}M大小热更新,是否确定下载?",
                        HotPatchManager.Instance.CurVersion, HotPatchManager.Instance.LoadSumSize/1024.0f),
                     OnClickStartDownLoad, OnClickCancleDownLoad);
                }
                else
                {
                    //进入游戏
                    StartOnFinish();
                }
            });
        }
        //点击开始下载
        void OnClickStartDownLoad()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    UIFrameExample.OpenCommonConfirm("下载确认", "当前使用的是手机流量，是否继续下载？", StartDownLoad, OnClickCancleDownLoad);
                }
            }
            else
            {
                StartDownLoad();
            }
        }
        //点击取消下载
        void OnClickCancleDownLoad()
        {
            Application.Quit();
        }
        /// <summary>
        /// 正式开始下载
        /// </summary>
        void StartDownLoad()
        {
            sliderTopTex.text = "下载中...";
            infoPanel.SetActive(true);
            hotContentTex.text = HotPatchManager.Instance.CurrentPatches.Des;
            UIFrameExample.Instance.StartCoroutine(HotPatchManager.Instance.StartDownLoadAB(StartOnFinish));
        }
        /// <summary>
        /// 下载完成回调，或者没有下载的东西直接进入游戏
        /// </summary>
        void StartOnFinish()
        {
            UIFrameExample.Instance.StartCoroutine(OnFinish());
        }
        IEnumerator OnFinish()
        {
            yield return UIFrameExample.Instance.StartCoroutine(UIFrameExample.Instance.StartGame(image,sliderTopTex));
            UIManager.Instance.PopPanel();
        }



        public  void Update()
        {
            if (HotPatchManager.Instance.StartUnPack)
            {
               sumTime += Time.deltaTime;
               image.fillAmount = HotPatchManager.Instance.GetUnpackProgress();
                float speed = (HotPatchManager.Instance.AlreadyUnPackSize / 1024.0f) / sumTime;
              text.text = string.Format("{0:F} M/S", speed);
            }

            if (HotPatchManager.Instance.StartDownload)
            {
                sumTime += Time.deltaTime;
                image.fillAmount = HotPatchManager.Instance.GetProgress();
                float speed = (HotPatchManager.Instance.GetLoadSize() / 1024.0f) / sumTime;
                text.text = string.Format("{0:F} M/S", speed);
            }
        }
        void ServerInfoError()
        {
            UIFrameExample.OpenCommonConfirm("服务器列表获取失败", "服务器列表获取失败，请检查网络链接是否正常？尝试重新下载！", CheckVersion, Application.Quit);
        }
        void ItemError(string all)
        {
            UIFrameExample.OpenCommonConfirm("资源下载失败", string.Format("{0}等资源下载失败，请重新尝试下载！", all), AnewDownload, QuitApp);
        }
        void QuitApp()
        {
            Application.Quit();
        }
        void AnewDownload()
        {
            HotPatchManager.Instance.CheckVersion((hot) =>
            {
                if (hot)
                {
                    StartDownLoad();
                }
                else
                {
                    StartOnFinish();
                }
            });
        }
        public override void OnExit()
        {
            HotPatchManager.Instance.ServerInfoError -= ServerInfoError;
            HotPatchManager.Instance.ItemError -= ItemError;
            //加载场景
            GameMapManager.Instance.LoadScene(ConStr.MenuScene, UIPanelType.Loading);
        }
    }


}