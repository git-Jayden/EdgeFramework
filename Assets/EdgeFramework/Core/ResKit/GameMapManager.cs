/****************************************************
	文件：GameMapManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:59   	
	Features：
*****************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace EdgeFramework.Res
{
    public class GameMapManager : Singleton<GameMapManager>
    {
        //加载场景完成回调
        public Action LoadSceneOverCallBack;
        //加载场景开始回调
        public Action LoadSceneEnterCallBack;

        //当前场景名
        public string CurrentMapName { get; set; }
        //场景是否加载完成
        public bool AlreadyLoadScene { get; set; }
        //切换场景进度条
        public static int LoadingProgress = 0;

        private MonoBehaviour mono;

        GameMapManager() { }
        /// <summary>
        /// 场景管理初始化
        /// </summary>
        /// <param name="mono"></param>
        public void Init(MonoBehaviour mono)
        {
            this.mono = mono;
        }
        /// <summary>
        /// 设置场景环境
        /// </summary>
        /// <param name="name">场景名</param>
        void SetSceneSetting(string name)
        {
            //设置各种场景环境,可以根据配表来TODO
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="name">场景名</param>
        public void LoadScene(string name)
        {
            LoadingProgress = 0;
            mono.StartCoroutine(LoadSceneAsync(name));
            
          //  UIPanelManager.Instance.PushPanel(Loading);
        }
        IEnumerator LoadSceneAsync(string name)
        {
            LoadSceneEnterCallBack?.Invoke();
            ClearCache();
            AlreadyLoadScene = false;
            AsyncOperation unLoadScene = SceneManager.LoadSceneAsync("EmptyScene", LoadSceneMode.Single);
            while (unLoadScene != null && !unLoadScene.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            LoadingProgress = 0;
            int targetProgress = 0;
            AsyncOperation asyncScene = SceneManager.LoadSceneAsync(name);
            if (asyncScene != null && !asyncScene.isDone)
            {
                asyncScene.allowSceneActivation = false;
                while (asyncScene.progress < 0.9f)
                {
                    targetProgress = (int)asyncScene.progress * 100;
                    yield return new WaitForEndOfFrame();
                    //平滑过渡
                    while (LoadingProgress < targetProgress)
                    {
                        ++LoadingProgress;
                        yield return new WaitForEndOfFrame();
                    }
                }
                CurrentMapName = name;
                SetSceneSetting(name);
                //自行加载剩余的10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress - 2)
                {
                    ++LoadingProgress;
                    yield return new WaitForEndOfFrame();
                }
                LoadingProgress = 100;
                asyncScene.allowSceneActivation = true;
                AlreadyLoadScene = true;
                LoadSceneOverCallBack?.Invoke();
            }
            yield return null;
        }
        private void ClearCache()
        {
            ObjectManager.Instance.ClearCache();
            ResourcesManager.Instance.ClearCache();
        }
    }
}
