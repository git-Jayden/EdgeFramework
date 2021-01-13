/****************************************************
	文件：ResourcesManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 17:01   	
	Features：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EdgeFramework.Res
{
    public enum LoadResPriority
    {
        RES_HIGHT = 0,//最高优先级
        RES_MIDDLE,//一般优先级
        RES_SLOW,//低优先级
        RES_NUM,//优先级数量
    }
    public class TResouceObj
    {
        //路径对应Crc
        public uint Crc = 0;
        //存ResourceItem
        public TResouceItem ResItem = null;
        //实例化出来的GameObject
        public GameObject CloneObj = null;
        //是否跳场景清除
        public bool Clear = true;
        //储存GUID
        public long Guid = 0;
        //是否已经放回对象池
        public bool Already = false;
        //--------------------------------

        //是否放到场景节点下
        public bool SetSceneParent = false;
        //示例化资源加载完成回调
        public OnAsyncObjFinish DealFinish = null;
        //异步参数
        public object Param1, Param2, Param3 = null;
        //离线数据
        public OfflineData OffData = null;

        public void Reset()
        {
            Crc = 0;
            CloneObj = null;
            Clear = true;
            Guid = 0;
            ResItem = null;
            Already = false;
            SetSceneParent = false;
            DealFinish = null;
            Param1 = null;
            Param2 = null;
            Param3 = null;
            OffData = null;

        }
    }
    public class TAsyncLoadResParam
    {
        public List<TAsyncCallBack> CallbackList = new List<TAsyncCallBack>();
        public uint Crc;
        public string Path;
        public bool IsSprite = false;
        public LoadResPriority Priority = LoadResPriority.RES_SLOW;

        public void Reset()
        {
            CallbackList.Clear();
            Crc = 0;
            Path = "";
            IsSprite = false;
            Priority = LoadResPriority.RES_SLOW;
        }
    }
    public class TAsyncCallBack
    {
        //加载完成的回调（针对ObjectManager）
        public OnAsyncFinish DealFinish = null;
        //ObjectManagerd对用的中间
        public TResouceObj ResObj = null;

        //-------------------------------------
        //加载完成的回调
        public OnAsyncObjFinish DealObjFinish = null;
        //回调参数
        public object Param1 = null, Param2 = null, Param3 = null;

        public void Reset()
        {
            DealObjFinish = null;
            DealFinish = null;

            Param1 = null;
            Param2 = null;
            Param3 = null;
        }
    }
    //资源加载完成回调
    public delegate void OnAsyncObjFinish(string path, Object obj, object parma1 = null, object param2 = null, object param3 = null);

    //实例化对象加载完成回调
    public delegate void OnAsyncFinish(string path, TResouceObj obj, object parma1 = null, object param2 = null, object param3 = null);

    public class ResourcesManager : Singleton<ResourcesManager>
    {
        private long mGuid = 0;
        //Mono脚本
        private MonoBehaviour mStartMono;



        //缓存使用的资源 
        private Dictionary<uint, TResouceItem> mAssetDic { get; set; } = new Dictionary<uint, TResouceItem>();
        //缓存引用计数为零的资源列表，达到缓存最大的时候释放这个列表最早没用的资源 
        private CMapList<TResouceItem> mNoRefrenceAssetMapList = new CMapList<TResouceItem>();

        //中间类，回调类的类对象池
        private SimpleObjectPool<TAsyncLoadResParam> mAsyncLoadResParamPool = new SimpleObjectPool<TAsyncLoadResParam>(() => new TAsyncLoadResParam(), initCount: 50);
        private SimpleObjectPool<TAsyncCallBack> mAsyncCallBackPool = new SimpleObjectPool<TAsyncCallBack>(() => new TAsyncCallBack(), initCount: 50);

        //正在异步加载的资源列表
        private List<TAsyncLoadResParam>[] mLoadingAssetList = new List<TAsyncLoadResParam>[(int)LoadResPriority.RES_NUM];
        //正在异步加载的Dic 
        private Dictionary<uint, TAsyncLoadResParam> mLoadingAssetDic = new Dictionary<uint, TAsyncLoadResParam>();

        //最长连续卡折加载资源的时间，单位微秒
        const long MAXLOADRESTIME = 200000;

        //最大缓存个数
        private const int MAXCACHECOUNT = 500;
        ResourcesManager() { }

        public void Init(MonoBehaviour mono)
        {
            for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
            {
                mLoadingAssetList[i] = new List<TAsyncLoadResParam>();
            }
            mStartMono = mono;
            mStartMono.StartCoroutine(AsyncLoadCor());
        }

        /// <summary>
        /// 创建唯一的GUID
        /// </summary>
        /// <returns></returns>
        public long CreatGuid()
        {
            return mGuid++;
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            List<TResouceItem> tempList = new List<TResouceItem>();
            foreach (TResouceItem item in mAssetDic.Values)
            {
                if (item.Clear)
                {
                    tempList.Add(item);
                }
            }
            foreach (TResouceItem item in tempList)
            {
                DestroyResouceItem(item, true);
            }
            tempList.Clear();

        }
        /// <summary>
        /// 取消异步加载资源
        /// </summary>
        /// <returns></returns>
        public bool CancleLoad(TResouceObj res)
        {
            TAsyncLoadResParam para = null;
            if (mLoadingAssetDic.TryGetValue(res.Crc, out para) && mLoadingAssetList[(int)para.Priority].Contains(para))
            {
                for (int i = para.CallbackList.Count; i > 0; i--)
                {
                    TAsyncCallBack tempCallBack = para.CallbackList[i];
                    if (tempCallBack != null && res == tempCallBack.ResObj)
                    {
                        tempCallBack.Reset();
                        mAsyncCallBackPool.Recycle(tempCallBack);
                        para.CallbackList.Remove(tempCallBack);
                    }
                }
                if (para.CallbackList.Count <= 0)
                {
                    para.Reset();
                    mLoadingAssetList[(int)para.Priority].Remove(para);
                    mAsyncLoadResParamPool.Recycle(para);
                    mLoadingAssetDic.Remove(res.Crc);
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
        public int IncreaseResouceRef(TResouceObj resObj, int count = 1)
        {
            return resObj != null ? IncreaseResouceRef(resObj.Crc, count) : 0;
        }
        /// <summary>
        /// 根据path增加引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IncreaseResouceRef(uint crc = 0, int count = 1)
        {
            TResouceItem item = null;
            if (!mAssetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount += count;
            item.LastUserTime = Time.realtimeSinceStartup;
            return item.RefCount;
        }
        /// <summary>
        /// 根据ResouceObj减少引用计数
        /// </summary>
        /// <param name="resObj"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResouceRef(TResouceObj resObj, int count = 0)
        {
            return resObj != null ? DecreaseResouceRef(resObj.Crc, count) : 0;
        }
        /// <summary>
        /// 根据路径减少引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResouceRef(uint crc = 0, int count = 1)
        {
            TResouceItem item = null;
            if (!mAssetDic.TryGetValue(crc, out item) || item == null)
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
            TResouceItem item = GetCacheResouceItem(crc, 0);
            if (item != null)
            {
                return;
            }
            Object obj = null;
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResouceItem(crc);
                if (item != null && item.Obj != null)
                {
                    obj = item.Obj as Object;
                }
                else
                {
                    if (item == null)
                    {
                        item = new TResouceItem();
                        item.Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
                if (item != null && item.AssetBundle != null)
                {
                    if (item.Obj != null)
                    {
                        obj = item.Obj;
                    }
                    else
                    {
                        obj = item.AssetBundle.LoadAsset<Object>(item.AssetName);
                    }
                }
            }
            CacheResouce(path, ref item, crc, obj);
            //跳场景不清空缓存
            item.Clear = false;
            ReleaseResouce(obj, false);
        }

        /// <summary>
        /// 同步资源加载，外部直接调用，仅加载不需要实例化的资源,例如Texture,音频等
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <returns></returns>
        public TResouceObj LoadResouce(string path, TResouceObj resObj)
        {
            if (resObj == null)
            {
                return null;
            }
            uint crc = resObj.Crc == 0 ? CRC32.GetCRC32(path) : resObj.Crc;
            TResouceItem item = GetCacheResouceItem(crc);
            if (item != null)
            {
                resObj.ResItem = item;
                return resObj;
            }
            Object obj = null;
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResouceItem(crc);
                if (item != null && item.Obj != null)
                {
                    obj = item.Obj as Object;
                }
                else
                {
                    if (item == null)
                    {
                        item = new TResouceItem();
                        item.Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
                if (item != null && item.AssetBundle != null)
                {
                    if (item.Obj != null)
                    {
                        obj = item.Obj as Object;
                    }
                    else
                    {
                        obj = item.AssetBundle.LoadAsset<Object>(item.AssetName);
                    }
                }
            }
            CacheResouce(path, ref item, crc, obj);
            resObj.ResItem = item;
            item.Clear = resObj.Clear;


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
            TResouceItem item = GetCacheResouceItem(crc);
            if (item != null)
            {
                return item.Obj as T;
            }
            T obj = null;
#if UNITY_EDITOR
            if (!AppConfig.UseAssetBundle)
            {
                item = AssetBundleManager.Instance.FindResouceItem(crc);
                if (item != null && item.AssetBundle != null)
                {
                    if (item.Obj != null)
                    {
                        obj = (T)item.Obj;
                    }
                    else
                    {
                        obj = item.AssetBundle.LoadAsset<T>(item.AssetName);
                    }
                }
                else
                {
                    if (item == null)
                    {
                        item = new TResouceItem();
                        item.Crc = crc;
                    }
                    obj = LoadAssetByEditor<T>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadResouceAssetBundle(crc);
                if (item != null && item.AssetBundle != null)
                {
                    if (item.Obj != null)
                    {
                        obj = item.Obj as T;
                    }
                    else
                    {
                        obj = item.AssetBundle.LoadAsset<T>(item.AssetName);
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
        /// <param name="destroyObj">是否将ResouceItem从缓存引用计数的资源列表中移除</param>
        /// <returns></returns>
        public bool ReleaseResouce(TResouceObj resObj, bool destroyObj = false)
        {
            if (resObj == null)
                return false;
            TResouceItem item = null;
            if (!mAssetDic.TryGetValue(resObj.Crc, out item) || item == null)
            {
                Debug.LogError("AssetDic里不存在该资源:" + resObj.CloneObj.name + " 可能释放了多次");
            }
            GameObject.Destroy(resObj.CloneObj);
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
            TResouceItem item = null;
            foreach (TResouceItem res in mAssetDic.Values)
            {
                if (res.Guid == obj.GetInstanceID())
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
            TResouceItem item = null;
            if (!mAssetDic.TryGetValue(crc, out item) || null == item)
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
        void CacheResouce(string path, ref TResouceItem item, uint crc, Object obj, int addrefcount = 1)
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
            item.Obj = obj;
            item.Guid = obj.GetInstanceID();
            item.LastUserTime = Time.realtimeSinceStartup;
            item.RefCount += addrefcount;
            TResouceItem oldItem = null;

            if (mAssetDic.TryGetValue(item.Crc, out oldItem))
            {
                mAssetDic[item.Crc] = item;
            }
            else
            {
                mAssetDic.Add(item.Crc, item);
            }
        }
        /// <summary>
        /// 缓存太多,清除最早没有使用的资源
        /// </summary>
        protected void WashOut()
        {
            //当大于缓存个数时，进行一半释放
            while (mNoRefrenceAssetMapList.Size() >= MAXCACHECOUNT)
            {
                for (int i = 0; i < MAXCACHECOUNT / 2; i++)
                {
                    TResouceItem item = mNoRefrenceAssetMapList.Back();
                    DestroyResouceItem(item, true);
                }
            }

        }
        /// <summary>
        /// 回收一个资源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="destroy"></param>
        protected void DestroyResouceItem(TResouceItem item, bool destroyCache = false)
        {
            if (item == null || item.RefCount > 0)
                return;
            if (!destroyCache)
            {
                mNoRefrenceAssetMapList.InsertToHead(item);
                return;
            }
            if (!mAssetDic.Remove(item.Crc))
            {
                return;
            }

            mNoRefrenceAssetMapList.Remove(item);
            // 释放assetbundle引用
            AssetBundleManager.Instance.ReleaseAsset(item);

            //清空资源对应的对象池
            ObjectManager.Instance.ClearPoolObject(item.Crc);

            if (item.Obj != null)
            {
                item.Obj = null;
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
        TResouceItem GetCacheResouceItem(uint crc, int addrefcount = 1)
        {
            TResouceItem item = null;
            if (mAssetDic.TryGetValue(crc, out item))
            {
                if (item != null)
                {
                    item.RefCount += addrefcount;
                    item.LastUserTime = Time.realtimeSinceStartup;
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
            TResouceItem item = GetCacheResouceItem(crc);
            if (item != null)
            {
                if (dealFinish != null)
                {
                    dealFinish(path, item.Obj, param1, param2, param3);
                }
                return;
            }
            //判断是否在加载中
            TAsyncLoadResParam para = null;
            if (!mLoadingAssetDic.TryGetValue(crc, out para) || para == null)
            {
                para = mAsyncLoadResParamPool.Allocate();
                para.Crc = crc;
                para.Path = path;
                para.IsSprite = isSprite;
                para.Priority = priority;
                mLoadingAssetDic.Add(crc, para);
                mLoadingAssetList[(int)priority].Add(para);
            }
            //往回调列表里面加回调
            TAsyncCallBack callBack = mAsyncCallBackPool.Allocate();
            callBack.DealObjFinish = dealFinish;

            callBack.Param1 = param1;
            callBack.Param2 = param2;
            callBack.Param3 = param3;
            para.CallbackList.Add(callBack);
        }
        /// <summary>
        /// 针对ObjectManager异步加载的接口
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resObj"></param>
        /// <param name="dealfinish"></param>
        /// <param name="priority"></param>
        public void AsyncLoadResouce(string path, TResouceObj resObj, OnAsyncFinish dealfinish, LoadResPriority priority)
        {
            TResouceItem item = GetCacheResouceItem(resObj.Crc);
            if (item != null)
            {
                resObj.ResItem = item;
                dealfinish?.Invoke(path, resObj);
            }
            //判断是否在加载中
            TAsyncLoadResParam para = null;
            if (!mLoadingAssetDic.TryGetValue(resObj.Crc, out para) || para == null)
            {
                para = mAsyncLoadResParamPool.Allocate();
                para.Crc = resObj.Crc;
                para.Path = path;
                para.Priority = priority;
                mLoadingAssetDic.Add(resObj.Crc, para);
                mLoadingAssetList[(int)priority].Add(para);
            }
            //往回调列表里面加回调
            TAsyncCallBack callBack = mAsyncCallBackPool.Allocate();
            callBack.DealFinish = dealfinish;
            callBack.ResObj = resObj;
            para.CallbackList.Add(callBack);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncLoadCor()
        {
            List<TAsyncCallBack> callBackList = null;
            //上一次yild的时间
            long lastYildTime = System.DateTime.Now.Ticks;
            while (true)
            {
                bool haveYield = false;
                for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
                {
                    if (mLoadingAssetList[(int)LoadResPriority.RES_HIGHT].Count > 0)
                    {
                        i = (int)LoadResPriority.RES_HIGHT;
                    }
                    else if (mLoadingAssetList[(int)LoadResPriority.RES_MIDDLE].Count > 0)
                    {
                        i = (int)LoadResPriority.RES_MIDDLE;
                    }

                    List<TAsyncLoadResParam> loadingList = mLoadingAssetList[i];

                    if (loadingList.Count <= 0)
                        continue;
                    TAsyncLoadResParam loadingItem = loadingList[0];
                    loadingList.RemoveAt(0);
                    callBackList = loadingItem.CallbackList;

                    Object obj = null;
                    TResouceItem item = null;
#if UNITY_EDITOR
                    if (!AppConfig.UseAssetBundle)
                    {
                        if (loadingItem.IsSprite)
                        {
                            obj = LoadAssetByEditor<Sprite>(loadingItem.Path);
                        }
                        else
                        {
                            obj = LoadAssetByEditor<Object>(loadingItem.Path);
                        }

                        //模拟异步加载
                        yield return new WaitForSeconds(0.5f);
                        item = AssetBundleManager.Instance.FindResouceItem(loadingItem.Crc);
                        if (item == null)
                        {
                            item = new TResouceItem();
                            item.Crc = loadingItem.Crc;
                        }
                    }
#endif
                    if (obj == null)
                    {
                        item = AssetBundleManager.Instance.LoadResouceAssetBundle(loadingItem.Crc);
                        if (item != null && item.AssetBundle != null)
                        {
                            AssetBundleRequest abRequest = null;
                            if (loadingItem.IsSprite)
                            {
                                abRequest = item.AssetBundle.LoadAssetAsync<Sprite>(item.AssetName);
                            }
                            else
                                abRequest = item.AssetBundle.LoadAssetAsync(item.AssetName);
                            yield return abRequest;
                            if (abRequest.isDone)
                            {
                                obj = abRequest.asset;
                            }
                            lastYildTime = System.DateTime.Now.Ticks;
                        }
                    }
                    CacheResouce(loadingItem.Path, ref item, loadingItem.Crc, obj, callBackList.Count);
                    for (int j = 0; j < callBackList.Count; j++)
                    {
                        TAsyncCallBack callBack = callBackList[j];
                        if (callBack != null && callBack.DealFinish != null && callBack.ResObj != null)
                        {
                            TResouceObj tempResObj = callBack.ResObj;
                            tempResObj.ResItem = item;
                            callBack.DealFinish(loadingItem.Path, tempResObj, tempResObj.Param1, tempResObj.Param2, tempResObj.Param3);
                            callBack.DealFinish = null;
                            tempResObj = null;
                        }

                        if (callBack != null && callBack.DealObjFinish != null)
                        {
                            callBack.DealObjFinish(loadingItem.Path, obj, callBack.Param1, callBack.Param2, callBack.Param3);
                            callBack.DealObjFinish = null;
                        }
                        callBack.Reset();
                        mAsyncCallBackPool.Recycle(callBack);
                    }
                    obj = null;
                    callBackList.Clear();
                    mLoadingAssetDic.Remove(loadingItem.Crc);

                    loadingItem.Reset();
                    mAsyncLoadResParamPool.Recycle(loadingItem);

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