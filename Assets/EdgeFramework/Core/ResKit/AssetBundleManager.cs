using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace EdgeFramework
{
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        protected string abConfigABName = "assetbundleconfig";
        //资源关系依赖配表，可以根据crc来找到对应资源块 key为路径的Crc
        protected Dictionary<uint, ResouceItem> resouceItemDic = new Dictionary<uint, ResouceItem>();
        //存储已经加载的Ab包,key为AB包名的Crc 
        protected Dictionary<uint, AssetBundleItem> assetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
        //AssetBundleItem类对象池
        protected SimpleObjectPool<AssetBundleItem> assetBundleItemPool = new SimpleObjectPool<AssetBundleItem>(() => new AssetBundleItem(), initCount: 500);


        AssetBundleManager() { }

        protected string ABLoadPath
        {
            get
            {
#if UNITY_ANDROID||UNITY_IPHONE
                return FilePath.PersistentDataPath + "Origin/";
#else
            return    FilePath.StreamingAssetsPath;
#endif
            }
        }
        /// <summary>
        /// 加载Ab配置表
        /// </summary>
        /// <returns></returns>
        public bool LoadAssetBundleConfig()
        {
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
                return false;
#endif
            resouceItemDic.Clear();
            string configPath = ABLoadPath + abConfigABName;
            string hotAbPath = HotPatchManager.Instance.ComputeABPath(abConfigABName);
            configPath = string.IsNullOrEmpty(hotAbPath) ? configPath : hotAbPath;
            byte[] bytes = AES.AESFileByteDecrypt(configPath,Constants.AESKEY);
            AssetBundle configAB = AssetBundle.LoadFromMemory(bytes);
            TextAsset textAsset = configAB.LoadAsset<TextAsset>(abConfigABName);
            if (textAsset == null)
            {
                Debug.LogError("AssetBundleConfig is no exist!");
                return false;
            }
            MemoryStream stream = new MemoryStream(textAsset.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
            stream.Close();
            for (int i = 0; i < config.abList.Count; i++)
            {
                ABBase abBase = config.abList[i];
                ResouceItem item = new ResouceItem();
                item.crc = abBase.crc;
                item.assetName = abBase.assetName;
                item.abName = abBase.abName;
                item.dependAssetBundle = abBase.abDependce;
                if (resouceItemDic.ContainsKey(item.crc))
                {
                    Debug.LogError("重复的Crc:" + item.assetName + "ab包名:" + item.abName);
                }
                else
                {
                    resouceItemDic.Add(item.crc, item);
                }
            }
            return true;
        }
        /// <summary>
        /// 根据路径的crc加载中间类ResouceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResouceItem LoadResouceAssetBundle(uint crc)
        {
            ResouceItem item = null;
            if (!resouceItemDic.TryGetValue(crc, out item) || item == null)
            {
                Debug.LogError(string.Format("LoadResourceAssetBundle Erro:can not find crc {0} in AssetBundleConfig", crc.ToString()));
                return item;
            }
            //if (item.assetBundle != null)
            //{
            //    item.RefCount++;
            //    return item;
            //}
            item.assetBundle = LoadAssetBundle(item.abName);
            if (item.dependAssetBundle != null)
            {
                for (int i = 0; i < item.dependAssetBundle.Count; i++)
                {
                    LoadAssetBundle(item.dependAssetBundle[i]);
                }
            }
            return item;
        }
        /// <summary>
        /// 加载单个AssetBundle根据名字
        /// </summary>
        /// <param name="name">AB 包名</param>
        /// <returns></returns>
        AssetBundle LoadAssetBundle(string name)
        {
            AssetBundleItem item = null;
            uint crc = CRC32.GetCRC32(name);
            if (!assetBundleItemDic.TryGetValue(crc, out item))
            {
                AssetBundle assetBundel = null;

                string hotAbPath = HotPatchManager.Instance.ComputeABPath(name);
                string fullpath = string.IsNullOrEmpty(hotAbPath) ? ABLoadPath + name : hotAbPath;
                byte[] bytes = AES.AESFileByteDecrypt(fullpath, Constants.AESKEY);
                assetBundel = AssetBundle.LoadFromMemory(bytes);

                if (assetBundel == null)
                {
                    Debug.LogError("Load AssetBundle Error:" + fullpath);
                }

                item = assetBundleItemPool.Allocate();
                item.assetBundle = assetBundel;
                item.reCount++;
                assetBundleItemDic.Add(crc, item);
            }
            else
            {
                item.reCount++;

            }
            return item.assetBundle;

        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="item"></param>
        public void ReleaseAsset(ResouceItem item)
        {
            if (item == null)
                return;
            if (item.dependAssetBundle != null && item.dependAssetBundle.Count > 0)
            {
                for (int i = 0; i < item.dependAssetBundle.Count; i++)
                {
                    UnLoadAssetBundle(item.dependAssetBundle[i]);
                }
            }
            UnLoadAssetBundle(item.abName);
        }
        void UnLoadAssetBundle(string name)
        {
            AssetBundleItem item = null;
            uint crc = CRC32.GetCRC32(name);
            if (assetBundleItemDic.TryGetValue(crc, out item) && item != null)
            {
                item.reCount--;
                if (item.reCount <= 0 && item.assetBundle != null)
                {
                    item.assetBundle.Unload(true);
                    item.Reset();
                    assetBundleItemPool.Recycle(item);
                    assetBundleItemDic.Remove(crc);
                }
            }
        }
        /// <summary>
        /// 根据crc查找ResourceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResouceItem FindResouceItem(uint crc)
        {
            ResouceItem item = null;
            resouceItemDic.TryGetValue(crc, out item);
            return item;
        }
    }
    public class AssetBundleItem
    {
        public AssetBundle assetBundle = null;
        public int reCount;
        public void Reset()
        {
            assetBundle = null;
            reCount = 0;
        }
    }
    public class ResouceItem
    {
        //资源路径的CRC
        public uint crc = 0;
        //资源的文件名
        public string assetName = string.Empty;
        //该资源所在的AssetBundel名字
        public string abName = string.Empty;
        //该资源所依赖的AssetBundle
        public List<string> dependAssetBundle = null;

        //该资源加载完的AB包
        public AssetBundle assetBundle = null;
        //资源对象
        public Object obj = null;
        //资源唯一标识
        public int guid = 0;
        //资源最后所使用的时间
        public float lastUserTime = 0.0f;
        //引用计数
        protected int refCount = 0;
        //是否跳场景清理
        public bool clear = true;
        public int RefCount
        {
            get { return refCount; }
            set
            {
                refCount = value;
                if (refCount < 0)
                {
                    Debug.LogError("refcount<0" + refCount + "," + (obj != null ? obj.name : "name is null"));
                }
            }
        }

    }
}
