using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ls_Mobile
{
    public class SyncResLoad : MonoBehaviour
    {
        public AudioSource m_Audio;
        private AudioClip clip;
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            AssetBundleManager.Instance.LoadAssetBundleConfig();
        }
        // Start is called before the first frame update
        void Start()
        {
            //同步加载 音效等
            //clip = ResouceManager.Instance.LoadResouce<AudioClip>("Assets/GameData/Sounds/senlin.mp3");
            //m_Audio.clip = clip;
            //m_Audio.Play();

            //预加载 音效等
           // ResouceManager.Instance.PreloadRes("Assets/Ls_Mobile/Example/ResKitExample/GameData/Sounds/senlin.mp3");
        }

        // Update is called once per frame
        void Update()
        {
            //同步加载资源 音效等
            if (Input.GetKeyDown(KeyCode.A))
            {
                long Time = System.DateTime.Now.Ticks;
                clip = ResouceManager.Instance.LoadResouce<AudioClip>("Assets/Ls_Mobile/Example/ResKitExample/GameData/Sounds/senlin.mp3");
                Debug.Log("预加载时间:" + (System.DateTime.Now.Ticks - Time));
                m_Audio.clip = clip;
                m_Audio.Play();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //资源卸载
                ResouceManager.Instance.ReleaseResouce(clip, true);
                m_Audio.Stop();
                m_Audio.clip = null;
                clip = null;
            }
        }
        private void OnApplicationQuit()
        {
            ResouceManager.Instance.ClearCache();
            Resources.UnloadUnusedAssets();
            Debug.Log("清空编辑器缓存");
        }
    }
}