using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    public class SyncObjLoad : MonoBehaviour
    {
        private GameObject obj;

        // Start is called before the first frame update
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            AssetBundleManager.Instance.LoadAssetBundleConfig();
            ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs"));
            HotPatchManager.Instance.Init(this);
        }
        void Start()
        {
            //同步加载prefab
            // obj=ObjectManager.Instance.InstantiateObject("Assets/GameData/Prefabs/Attack.prefab", true);

            //预加载 prefab
            //ObjectManager.Instance.PreloadGameObject("Assets/Ls_Mobile/ResKit/Example/Prefabs/Attack.prefab", 20);
        }

        // Update is called once per frame
        void Update()
        {
            //同步加载GameObject
            if (Input.GetKeyDown(KeyCode.A))
            {
                ObjectManager.Instance.ReleaseObject(obj,recycleParent:false);
                obj = null;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                obj = ObjectManager.Instance.InstantiateObject("Assets/Ls_Mobile/Example/ResKitExample/Prefabs/Attack.prefab", true);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ObjectManager.Instance.ReleaseObject(obj, 0, true);
                obj = null;
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