using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EdgeFramework
{
    public class MonoSimplify<T> : MonoBehaviour, ISingleton where T : MonoSimplify<T>
    {
        public static MonoSimplify<T> Instance
        {
            get { return MonoSingletonProperty<MonoSimplify<T>>.Instance; }
        }
        public void OnSingletonInit()
        {

        }
        protected virtual void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            ////加载AssetBundle配置表
            //AssetBundleManager.Instance.LoadAssetBundleConfig();
            ////资源管理器初始化
            //ResouceManager.Instance.Init(this);
            ////对象管理器初始化
            //ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs"));
        }
        protected virtual void Start()
        {
            GameMapManager.Instance.Init(this);
        }

        protected virtual void  OnApplicationQuit()
        {
            ResouceManager.Instance.ClearCache();
            Resources.UnloadUnusedAssets();
            Debug.Log("清空编辑器缓存");
        }


    }
}