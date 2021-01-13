/****************************************************
	文件：AssetBundleManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:59   	
	Features：
*****************************************************/
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace EdgeFramework.Res
{
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        private string mAbConfigABName = "assetbundleconfig";
        //资源关系依赖配表，可以根据crc来找到对应资源块 key为路径的Crc
        private Dictionary<uint, TResouceItem> mResouceItemDic = new Dictionary<uint, TResouceItem>();
        //存储已经加载的Ab包,key为AB包名的Crc 
        private Dictionary<uint, TAssetBundleItem> mAssetBundleItemDic = new Dictionary<uint, TAssetBundleItem>();
        //AssetBundleItem类对象池
        private SimpleObjectPool<TAssetBundleItem> mAssetBundleItemPool = new SimpleObjectPool<TAssetBundleItem>(() => new TAssetBundleItem(), initCount: 500);


        AssetBundleManager() { }

        protected string ABLoadPath
        {
            get
            {
#if UNITY_ANDROID||UNITY_IPHONE
                return Application.persistentDataPath + "/Origin/";
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
            mResouceItemDic.Clear();
            string configPath = ABLoadPath + mAbConfigABName;
            string hotAbPath = HotPatchManager.Instance.ComputeABPath(mAbConfigABName);
            configPath = string.IsNullOrEmpty(hotAbPath) ? configPath : hotAbPath;
            byte[] bytes = AES.AESFileByteDecrypt(configPath, EdgeFrameworkConst.AESKEY);
            AssetBundle configAB = AssetBundle.LoadFromMemory(bytes);
            TextAsset textAsset = configAB.LoadAsset<TextAsset>(mAbConfigABName);
            if (textAsset == null)
            {
                Debug.LogError("AssetBundleConfig is no exist!");
                return false;
            }
            MemoryStream stream = new MemoryStream(textAsset.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
            stream.Close();
            for (int i = 0; i < config.AbList.Count; i++)
            {
                ABBase abBase = config.AbList[i];
                TResouceItem item = new TResouceItem();
                item.Crc = abBase.Crc;
                item.AssetName = abBase.AssetName;
                item.AbName = abBase.AbName;
                item.DependAssetBundle = abBase.AbDependce;
                if (mResouceItemDic.ContainsKey(item.Crc))
                {
                    Debug.LogError("重复的Crc:" + item.AssetName + "ab包名:" + item.AbName);
                }
                else
                {
                    mResouceItemDic.Add(item.Crc, item);
                }
            }
            return true;
        }
        /// <summary>
        /// 根据路径的crc加载中间类ResouceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public TResouceItem LoadResouceAssetBundle(uint crc)
        {
            TResouceItem item = null;
            if (!mResouceItemDic.TryGetValue(crc, out item) || item == null)
            {
                Debug.LogError(string.Format("LoadResourceAssetBundle Erro:can not find crc {0} in AssetBundleConfig", crc.ToString()));
                return item;
            }
            //if (item.assetBundle != null)
            //{
            //    item.RefCount++;
            //    return item;
            //}
            item.AssetBundle = LoadAssetBundle(item.AbName);
            if (item.DependAssetBundle != null)
            {
                for (int i = 0; i < item.DependAssetBundle.Count; i++)
                {
                    LoadAssetBundle(item.DependAssetBundle[i]);
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
            TAssetBundleItem item = null;
            uint crc = CRC32.GetCRC32(name);
            if (!mAssetBundleItemDic.TryGetValue(crc, out item))
            {
                AssetBundle assetBundel = null;

                string hotAbPath = HotPatchManager.Instance.ComputeABPath(name);
                string fullpath = string.IsNullOrEmpty(hotAbPath) ? ABLoadPath + name : hotAbPath;
                byte[] bytes = AES.AESFileByteDecrypt(fullpath, EdgeFrameworkConst.AESKEY);
                assetBundel = AssetBundle.LoadFromMemory(bytes);

                if (assetBundel == null)
                {
                    Debug.LogError("Load AssetBundle Error:" + fullpath);
                }

                item = mAssetBundleItemPool.Allocate();
                item.AB = assetBundel;
                item.ReCount++;
                mAssetBundleItemDic.Add(crc, item);
            }
            else
            {
                item.ReCount++;

            }
            return item.AB;

        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="item"></param>
        public void ReleaseAsset(TResouceItem item)
        {
            if (item == null)
                return;
            if (item.DependAssetBundle != null && item.DependAssetBundle.Count > 0)
            {
                for (int i = 0; i < item.DependAssetBundle.Count; i++)
                {
                    UnLoadAssetBundle(item.DependAssetBundle[i]);
                }
            }
            UnLoadAssetBundle(item.AbName);
        }
        void UnLoadAssetBundle(string name)
        {
            TAssetBundleItem item = null;
            uint crc = CRC32.GetCRC32(name);
            if (mAssetBundleItemDic.TryGetValue(crc, out item) && item != null)
            {
                item.ReCount--;
                if (item.ReCount <= 0 && item.AB != null)
                {
                    item.AB.Unload(true);
                    item.Reset();
                    mAssetBundleItemPool.Recycle(item);
                    mAssetBundleItemDic.Remove(crc);
                }
            }
        }
        /// <summary>
        /// 根据crc查找ResourceItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public TResouceItem FindResouceItem(uint crc)
        {
            TResouceItem item = null;
            mResouceItemDic.TryGetValue(crc, out item);
            return item;
        }
    }
    public class TAssetBundleItem
    {
        public AssetBundle AB = null;
        public int ReCount;
        public void Reset()
        {
            AB = null;
            ReCount = 0;
        }
    }
    public class TResouceItem
    {
        //资源路径的CRC
        public uint Crc = 0;
        //资源的文件名
        public string AssetName = string.Empty;
        //该资源所在的AssetBundel名字
        public string AbName = string.Empty;
        //该资源所依赖的AssetBundle
        public List<string> DependAssetBundle = null;

        //该资源加载完的AB包
        public AssetBundle AssetBundle = null;
        //资源对象
        public Object Obj = null;
        //资源唯一标识
        public int Guid = 0;
        //资源最后所使用的时间
        public float LastUserTime = 0.0f;
        //引用计数
        protected int mRefCount = 0;
        //是否跳场景清理
        public bool Clear = true;
        public int RefCount
        {
            get { return mRefCount; }
            set
            {
                mRefCount = value;
                if (mRefCount < 0)
                {
                    Debug.LogError("refcount<0" + mRefCount + "," + (Obj != null ? Obj.name : "name is null"));
                }
            }
        }

    }
}
