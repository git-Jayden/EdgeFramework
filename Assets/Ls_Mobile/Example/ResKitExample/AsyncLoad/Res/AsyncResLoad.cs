using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EdgeFramework
{
    public class AsyncResLoad : MonoBehaviour
    {
        public AudioSource m_Audio;
        private AudioClip clip;
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            AssetBundleManager.Instance.LoadAssetBundleConfig();
            ResouceManager.Instance.Init(this);
        }
        private void Start()
        {
            //异步加载 音效等
            //ResouceManager.Instance.AsyncLoadResource("Assets/GameData/Sounds/menusound.mp3", LoadFinish,LoadResPriority.RES_SLOW);
        }
        void LoadFinish(string path, Object obj, object parma1, object param2, object param3)
        {
            clip = obj as AudioClip;
            m_Audio.clip = clip;
            m_Audio.Play();
        }
        private void Update()
        {
            //异步加载资源 音效等
            if (Input.GetKeyDown(KeyCode.A))
            {
                long Time = System.DateTime.Now.Ticks;
                ResouceManager.Instance.AsyncLoadResouce("Assets/Ls_Mobile/Example/ResKitExample/GameData/Sounds/menusound.mp3", LoadFinish, LoadResPriority.RES_SLOW);
                Debug.Log("预加载时间:" + (System.DateTime.Now.Ticks - Time));
                m_Audio.clip = clip;
                m_Audio.Play();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ResouceManager.Instance.ReleaseResouce(clip, true);
                m_Audio.Stop();
                m_Audio.clip = null;
                clip = null;
            }
        }
    }
}