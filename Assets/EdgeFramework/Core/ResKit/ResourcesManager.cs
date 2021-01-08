using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EdgeFramework
{
    public enum LoadResPriority
    {
        RES_HIGHT = 0,//最高优先级
        RES_MIDDLE,//一般优先级
        RES_SLOW,//低优先级
        RES_NUM,//优先级数量
    }
    public class ResouceObj
    {
        //路径对应Crc
        public uint crc = 0;
        //存ResourceItem
        public ResouceItem resItem = null;
        //实例化出来的GameObject
        public GameObject cloneObj = null;
        //是否跳场景清除
        public bool clear = true;
        //储存GUID
        public long guid = 0;
        //是否已经放回对象池
        public bool already = false;
        //--------------------------------

        //是否放到场景节点下
        public bool setSceneParent = false;
        //示例化资源加载完成回调
        public OnAsyncObjFinish dealFinish = null;
        //异步参数
        public object param1, param2, param3 = null;
        //离线数据
        public OfflineData offlineData = null;

        public void Reset()
        {
            crc = 0;
            cloneObj = null;
            clear = true;
            guid = 0;
            resItem = null;
            already = false;
            setSceneParent = false;
            OnAsyncObjFinish m_DealFinish = null;
            param1 = null;
            param2 = null;
            param3 = null;
            offlineData = null;

        }
    }
    public class AsyncLoadResParam
    {
        public List<AsyncCallBack> callbackList = new List<AsyncCallBack>();
        public uint crc;
        public string path;
        public bool sprite = false;
        public LoadResPriority priority = LoadResPriority.RES_SLOW;

        public void Reset()
        {
            callbackList.Clear();
            crc = 0;
            path = "";
            sprite = false;
            priority = LoadResPriority.RES_SLOW;
        }
    }
    public class AsyncCallBack
    {
        //加载完成的回调（针对ObjectManager）
        public OnAsyncFinish dealFinish = null;
        //ObjectManagerd对用的中间
        public ResouceObj resObj = null;

        //-------------------------------------
        //加载完成的回调
        public OnAsyncObjFinish dealObjFinish = null;
        //回调参数
        public object param1 = null, param2 = null, param3 = null;

        public void Reset()
        {
            dealObjFinish = null;
            dealFinish = null;

            param1 = null;
            param2 = null;
            param3 = null;
        }
    }
    //资源加载完成回调
    public delegate void OnAsyncObjFinish(string path, Object obj, object parma1 = null, object param2 = null, object param3 = null);

    //实例化对象加载完成回调
    public delegate void OnAsyncFinish(string path, ResouceObj obj, object parma1 = null, object param2 = null, object param3 = null);

    public class ResourcesManager : Singleton<ResourcesManager>
    {
        protected long guid = 0;
        //Mono脚本
        protected MonoBehaviour startMono;



        //缓存使用的资源 
        public Dictionary<uint, ResouceItem> assetDic { get; set; } = new Dictionary<uint, ResouceItem>();
        //缓存引用计数为零的资源列表，达到缓存最大的时候释放这个列表最早没用的资源 
        protected CMapList<ResouceItem> noRefrenceAssetMapList = new CMapList<ResouceItem>();

        //中间类，回调类的类对象池
        protected SimpleObjectPool<AsyncLoadResParam> asyncLoadResParamPool = new SimpleObjectPool<AsyncLoadResParam>(() => new AsyncLoadResParam(), initCount: 50);
        protected SimpleObjectPool<AsyncCallBack> asyncCallBackPool = new SimpleObjectPool<AsyncCallBack>(() => new AsyncCallBack(), initCount: 50);

        //正在异步加载的资源列表
        protected List<AsyncLoadResParam>[] loadingAssetList = new List<AsyncLoadResParam>[(int)LoadResPriority.RES_NUM];
        //正在异步加载的Dic 
        protected Dictionary<uint, AsyncLoadResParam> loadingAssetDic = new Dictionary<uint, AsyncLoadResParam>();

        //最长连续卡折加载资源的时间，单位微秒
        const long MAXLOADRESTIME = 200000;

        //最大缓存个数
        private const int MAXCACHECOUNT = 500;
        ResourcesManager() { }

        public void Init(MonoBehaviour mono)
        {
            for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
            {
                loadingAssetList[i] = new List<AsyncLoadResParam>();
            }
            startMono = mono;
            startMono.StartCoroutine(AsyncLoadCor());
        }

        /// <summary>
        /// 创建唯一的GUID
        /// </summary>
        /// <returns></returns>
        public long CreatGuid()
        {
            return guid++;
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            List<ResouceItem> tempList = new List<ResouceItem>();
            foreach (ResouceItem item in assetDic.Values)
            {
                if (item.clear)
                {
                    tempList.Add(item);
                }
            }
            foreach (ResouceItem item in tempList)
            {
                DestroyResouceItem(item, true);
            }
            tempList.Clear();

        }
        /// <summary>
        /// 取消异步加载资源
        /// </summary>
        /// <returns></returns>
        public bool CancleLoad(ResouceObj res)
        {
            AsyncLoadResParam para = null;
            if (loadingAssetDic.TryGetValue(res.crc, out para) && loadingAssetList[(int)para.priority].Contains(para))
            {
                for (int i = para.callbackList.Count; i > 0; i--)
                {
                    AsyncCallBack tempCallBack = para.callbackList[i];
                    if (tempCallBack != null && res == tempCallBack.resObj)
                    {
                        tempCallBack.Reset();
                        asyncCallBackPool.Recycle(tempCallBack);
                        para.callbackList.Remove(tempCallBack);
                    }
                }
                if (para.callbackList.Count <= 0)
                {
                    para.Reset();
                    loadingAssetList[(int)para.priority].Remove(para);
                    asyncLoadResParamPool.Recycle(para);
                    loadingAssetDic.Remove(res.crc);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据ResObj增加引用计数
        /// </summary>
        /// <param name="resObj"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IncreaseResouceRef(ResouceObj resObj, int count = 1)
        {
            return resObj != null ? IncreaseResouceRef(resObj.crc, count) : 0;
        }
        /// <summary>
        /// 根据path增加引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IncreaseResouceRef(uint crc = 0, int count = 1)
        {
            ResouceItem item = null;
            if (!assetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount += count;
            item.lastUserTime = Time.realtimeSinceStartup;
            return item.RefCount;
        }
        /// <summary>
        /// 根据ResouceObj减少引用计数
        /// </summary>
        /// <param name="resObj"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResouceRef(ResouceObj resObj, int count = 0)
        {
            return resObj != null ? DecreaseResouceRef(resObj.crc, count) : 0;
        }
        /// <summary>
        /// 根据路径减少引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResouceRef(uint crc = 0, int count = 1)
        {
            ResouceItem item = null;
            if (!assetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount -= count;
            return item.RefCount;
        }
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="path"></param>
        public void PreloadRes(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            uint crc = CRC32.GetCRC32(path);
            ResouceItem item = GetCacheResouceItem(crc, 0);
            if (item != null)
            {
                return;
            }
            Object obj = null;
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResouceItem(crc);
                if (item != null && item.obj != null)
                {
                    obj = item.obj as Object;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResouceItem();
                        item.crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
                if (item != null && item.assetBundle != null)
                {
                    if (item.obj != null)
                    {
                        obj = item.obj;
                    }
                    else
                    {
                        obj = item.assetBundle.LoadAsset<Object>(item.assetName);
                    }
                }
            }
            CacheResouce(path, ref item, crc, obj);
            //跳场景不清空缓存
            item.clear = false;
            ReleaseResouce(obj, false);
        }

        /// <summary>
        /// 同步资源加载，外部直接调用，仅加载不需要实例化的资源,例如Texture,音频等
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <returns></returns>
        public ResouceObj LoadResouce(string path, ResouceObj resObj)
        {
            if (resObj == null)
            {
                return null;
            }
            uint crc = resObj.crc == 0 ? CRC32.GetCRC32(path) : resObj.crc;
            ResouceItem item = GetCacheResouceItem(crc);
            if (item != null)
            {
                resObj.resItem = item;
                return resObj;
            }
            Object obj = null;
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResouceItem(crc);
                if (item != null && item.obj != null)
                {
                    obj = item.obj as Object;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResouceItem();
                        item.crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
                if (item != null && item.assetBundle != null)
                {
                    if (item.obj != null)
                    {
                        obj = item.obj as Object;
                    }
                    else
                    {
                        obj = item.assetBundle.LoadAsset<Object>(item.assetName);
                    }
                }
            }
            CacheResouce(path, ref item, crc, obj);
            resObj.resItem = item;
            item.clear = resObj.clear;


            return resObj;
        }

        /// <summary>
        /// 同步资源加载，外部直接调用，仅加载不需要实例化的资源，例如Texture,音频等等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadResouce<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            uint crc = CRC32.GetCRC32(path);
            ResouceItem item = GetCacheResouceItem(crc);
            if (item != null)
            {
                return item.obj as T;
            }
            T obj = null;
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResouceItem(crc);
                if (item != null && item.assetBundle != null)
                {
                    if (item.obj != null)
                    {
                        obj = (T)item.obj;
                    }
                    else
                    {
                        obj = item.assetBundle.LoadAsset<T>(item.assetName);
                    }
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResouceItem();
                        item.crc = crc;
                    }
                    obj = LoadAssetByEditor<T>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
                if (item != null && item.assetBundle != null)
                {
                    if (item.obj != null)
                    {
                        obj = item.obj as T;
                    }
                    else
                    {
                        obj = item.assetBundle.LoadAsset<T>(item.assetName);
                    }
                }
            }
            CacheResouce(path, ref item, crc, obj);
            return obj;
        }
        /// <summary>
        /// 根据ResouceObj卸载资源
        /// </summary>
        /// <param name="resObj"></param>
        /// <param name="destroyObj"></param>
        /// <returns></returns>
        public bool ReleaseResouce(ResouceObj resObj, bool destroyObj = false)
        {
            if (resObj == null)
                return false;
            ResouceItem item = null;
            if (!assetDic.TryGetValue(resObj.crc, out item) || item == null)
            {
                Debug.LogError("AssetDic里不存在改资源:" + resObj.cloneObj.name + " 可能释放了多次");
            }
            GameObject.Destroy(resObj.cloneObj);
            item.RefCount--;
            DestroyResouceItem(item, destroyObj);
            return true;
        }

        /// <summary>
        /// 不需要实例化的资源卸载，根据对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destroyObj"></param>
        /// <returns></returns>
        public bool ReleaseResouce(Object obj, bool destroyObj = false)
        {
            if (obj == null)
                return false;
            ResouceItem item = null;
            foreach (ResouceItem res in assetDic.Values)
            {
                if (res.guid == obj.GetInstanceID())
                    item = res;
            }
            if (item == null)
            {
                Debug.LogError("AssetDic里不存在资源" + obj.name + "可能释放了多次");
                return false;
            }
            item.RefCount--;
            DestroyResouceItem(item, destroyObj);
            return true;
        }
        /// <summary>
        /// 不需要实例化的资源卸载,根据路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destroyObj"></param>
        /// <returns></returns>
        public bool ReleaseResouce(string path, bool destroyObj = false)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            uint crc = CRC32.GetCRC32(path);
            ResouceItem item = null;
            if (!assetDic.TryGetValue(crc, out item) || null == item)
            {
                Debug.LogError("AssetDic里不存在资源" + path + "可能释放了多次");
            }

            item.RefCount--;
            DestroyResouceItem(item, destroyObj);
            return true;
        }
        /// <summary>
        /// 缓存加载的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="crc"></param>
        /// <param name="obj"></param>
        /// <param name="addrefcount"></param>
        void CacheResouce(string path, ref ResouceItem item, uint crc, Object obj, int addrefcount = 1)
        {
            //缓存太多,清除最早没有使用的资源
            WashOut();

            if (item == null)
            {
                Debug.LogError("ResourceItem is null,path:" + path);
            }
            if (obj == null)
            {
                Debug.LogError("Resource Fail:" + path);
            }
            item.obj = obj;
            item.guid = obj.GetInstanceID();
            item.lastUserTime = Time.realtimeSinceStartup;
            item.RefCount += addrefcount;
            ResouceItem oldItem = null;

            if (assetDic.TryGetValue(item.crc, out oldItem))
            {
                assetDic[item.crc] = item;
            }
            else
            {
                assetDic.Add(item.crc, item);
            }
        }
        /// <summary>
        /// 缓存太多,清除最早没有使用的资源
        /// </summary>
        protected void WashOut()
        {
            //当大于缓存个数时，进行一半释放
            while (noRefrenceAssetMapList.Size() >= MAXCACHECOUNT)
            {
                for (int i = 0; i < MAXCACHECOUNT / 2; i++)
                {
                    ResouceItem item = noRefrenceAssetMapList.Back();
                    DestroyResouceItem(item, true);
                }
            }

        }
        /// <summary>
        /// 回收一个资源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="destroy"></param>
        protected void DestroyResouceItem(ResouceItem item, bool destroyCache = false)
        {
            if (item == null || item.RefCount > 0)
                return;
            if (!destroyCache)
            {
                noRefrenceAssetMapList.InsertToHead(item);
                return;
            }
            if (!assetDic.Remove(item.crc))
            {
                return;
            }

            noRefrenceAssetMapList.Remove(item);
            // 释放assetbundle引用
            AssetBundleManager.Instance.ReleaseAsset(item);

            //清空资源对应的对象池
            ObjectManager.Instance.ClearPoolObject(item.crc);

            if (item.obj != null)
            {
                item.obj = null;
#if UNITY_EDITOR
                Resources.UnloadUnusedAssets();
#endif
            }
        }
#if UNITY_EDITOR
        protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
        /// <summary>
        /// 从资源池获取缓存资源
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="addrefcount"></param>
        /// <returns></returns>
        ResouceItem GetCacheResouceItem(uint crc, int addrefcount = 1)
        {
            ResouceItem item = null;
            if (assetDic.TryGetValue(crc, out item))
            {
                if (item != null)
                {
                    item.RefCount += addrefcount;
                    item.lastUserTime = Time.realtimeSinceStartup;
                }
            }
            return item;
        }
        /// <summary>
        /// 异步加载资源（仅仅是不需要实例化的资源，例如音频，图片等）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dealFinish"></param>
        /// <param name="priority"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="crc"></param>
        public void AsyncLoadResouce(string path, OnAsyncObjFinish dealFinish, LoadResPriority priority, bool isSprite = false, object param1 = null, object param2 = null, object param3 = null, uint crc = 0)
        {
            if (crc == 0)
            {
                crc = CRC32.GetCRC32(path);
            }
            ResouceItem item = GetCacheResouceItem(crc);
            if (item != null)
            {
                if (dealFinish != null)
                {
                    dealFinish(path, item.obj, param1, param2, param3);
                }
                return;
            }
            //判断是否在加载中
            AsyncLoadResParam para = null;
            if (!loadingAssetDic.TryGetValue(crc, out para) || para == null)
            {
                para = asyncLoadResParamPool.Allocate();
                para.crc = crc;
                para.path = path;
                para.sprite = isSprite;
                para.priority = priority;
                loadingAssetDic.Add(crc, para);
                loadingAssetList[(int)priority].Add(para);
            }
            //往回调列表里面加回调
            AsyncCallBack callBack = asyncCallBackPool.Allocate();
            callBack.dealObjFinish = dealFinish;

            callBack.param1 = param1;
            callBack.param2 = param2;
            callBack.param3 = param3;
            para.callbackList.Add(callBack);
        }
        /// <summary>
        /// 针对ObjectManager异步加载的接口
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <param name="dealfinish"></param>
        /// <param name="priority"></param>
        public void AsyncLoadResouce(string path, ResouceObj resObj, OnAsyncFinish dealfinish, LoadResPriority priority)
        {
            ResouceItem item = GetCacheResouceItem(resObj.crc);
            if (item != null)
            {
                resObj.resItem = item;
                dealfinish?.Invoke(path, resObj);
            }
            //判断是否在加载中
            AsyncLoadResParam para = null;
            if (!loadingAssetDic.TryGetValue(resObj.crc, out para) || para == null)
            {
                para = asyncLoadResParamPool.Allocate();
                para.crc = resObj.crc;
                para.path = path;
                para.priority = priority;
                loadingAssetDic.Add(resObj.crc, para);
                loadingAssetList[(int)priority].Add(para);
            }
            //往回调列表里面加回调
            AsyncCallBack callBack = asyncCallBackPool.Allocate();
            callBack.dealFinish = dealfinish;
            callBack.resObj = resObj;
            para.callbackList.Add(callBack);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncLoadCor()
        {
            List<AsyncCallBack> callBackList = null;
            //上一次yild的时间
            long lastYildTime = System.DateTime.Now.Ticks;
            while (true)
            {
                bool haveYield = false;
                for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
                {
                    if (loadingAssetList[(int)LoadResPriority.RES_HIGHT].Count > 0)
                    {
                        i = (int)LoadResPriority.RES_HIGHT;
                    }
                    else if (loadingAssetList[(int)LoadResPriority.RES_MIDDLE].Count > 0)
                    {
                        i = (int)LoadResPriority.RES_MIDDLE;
                    }

                    List<AsyncLoadResParam> loadingList = loadingAssetList[i];

                    if (loadingList.Count <= 0)
                        continue;
                    AsyncLoadResParam loadingItem = loadingList[0];
                    loadingList.RemoveAt(0);
                    callBackList = loadingItem.callbackList;

                    Object obj = null;
                    ResouceItem item = null;
#if UNITY_EDITOR
                    if (!AppConfig.UseAssetBundle)
                    {
                        if (loadingItem.sprite)
                        {
                            obj = LoadAssetByEditor<Sprite>(loadingItem.path);
                        }
                        else
                        {
                            obj = LoadAssetByEditor<Object>(loadingItem.path);
                        }

                        //模拟异步加载
                        yield return new WaitForSeconds(0.5f);
                        item = AssetBundleManager.Instance.FindResouceItem(loadingItem.crc);
                        if (item == null)
                        {
                            item = new ResouceItem();
                            item.crc = loadingItem.crc;
                        }
                    }
#endif
                    if (obj == null)
                    {
                        item = AssetBundleManager.Instance.LoadResouceAssetBundle(loadingItem.crc);
                        if (item != null && item.assetBundle != null)
                        {
                            AssetBundleRequest abRequest = null;
                            if (loadingItem.sprite)
                            {
                                abRequest = item.assetBundle.LoadAssetAsync<Sprite>(item.assetName);
                            }
                            else
                                abRequest = item.assetBundle.LoadAssetAsync(item.assetName);
                            yield return abRequest;
                            if (abRequest.isDone)
                            {
                                obj = abRequest.asset;
                            }
                            lastYildTime = System.DateTime.Now.Ticks;
                        }
                    }
                    CacheResouce(loadingItem.path, ref item, loadingItem.crc, obj, callBackList.Count);
                    for (int j = 0; j < callBackList.Count; j++)
                    {
                        AsyncCallBack callBack = callBackList[j];
                        if (callBack != null && callBack.dealFinish != null && callBack.resObj != null)
                        {
                            ResouceObj tempResObj = callBack.resObj;
                            tempResObj.resItem = item;
                            callBack.dealFinish(loadingItem.path, tempResObj, tempResObj.param1, tempResObj.param2, tempResObj.param3);
                            callBack.dealFinish = null;
                            tempResObj = null;
                        }

                        if (callBack != null && callBack.dealObjFinish != null)
                        {
                            callBack.dealObjFinish(loadingItem.path, obj, callBack.param1, callBack.param2, callBack.param3);
                            callBack.dealObjFinish = null;
                        }
                        callBack.Reset();
                        asyncCallBackPool.Recycle(callBack);
                    }
                    obj = null;
                    callBackList.Clear();
                    loadingAssetDic.Remove(loadingItem.crc);

                    loadingItem.Reset();
                    asyncLoadResParamPool.Recycle(loadingItem);

                    if (System.DateTime.Now.Ticks - lastYildTime > MAXLOADRESTIME)
                    {
                        yield return null;
                        lastYildTime = System.DateTime.Now.Ticks;
                        haveYield = true;
                    }
                }
                if (!haveYield || System.DateTime.Now.Ticks - lastYildTime > MAXLOADRESTIME)
                {
                    lastYildTime = System.DateTime.Now.Ticks;
                    yield return null;
                }
                yield return null;
            }
        }


    }
   
}