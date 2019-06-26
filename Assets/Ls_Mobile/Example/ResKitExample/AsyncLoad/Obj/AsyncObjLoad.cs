using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile
{
    public class AsyncObjLoad : MonoBehaviour
    {
        private GameObject m_obj;
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            AssetBundleManager.Instance.LoadAssetBundleConfig();
            ResouceManager.Instance.Init(this);
            ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs"));
        }

        // Start is called before the first frame update
        void Start()
        {
            //异步加载prefab
            // ObjectManager.Instance.InstantiateObjectAsync("Assets/GameData/Prefabs/Attack.prefab", LoadAsyncFinish,LoadResPriority.RES_HIGHT,true);
        }
        void LoadAsyncFinish(string path, Object obj, object parma1, object param2, object param3)
        {
            m_obj = obj as GameObject;

        }

        // Update is called once per frame
        void Update()
        {
            //异步加载GameObject
            if (Input.GetKeyDown(KeyCode.A))
            {
                ObjectManager.Instance.ReleaseObject(m_obj);
                m_obj = null;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ObjectManager.Instance.InstantiateObjectAsync("Assets/Prefabs/Attack.prefab", LoadAsyncFinish, LoadResPriority.RES_HIGHT, true);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ObjectManager.Instance.ReleaseObject(m_obj, 0, true);
                m_obj = null;
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
