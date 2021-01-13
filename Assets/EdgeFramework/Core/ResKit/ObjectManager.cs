/****************************************************
	文件：ObjectManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 17:00   	
	Features：
*****************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace EdgeFramework.Res
{
    public class ObjectManager : Singleton<ObjectManager>
    {

        public Transform RecyclePoolTrs;
        //场景节点 
        public Transform SceneTrs;
        //对象池 
        private Dictionary<uint, List<TResouceObj>> mObjectPoolDic = new Dictionary<uint, List<TResouceObj>>();
        //暂存ResObj的Dic
        private Dictionary<int, TResouceObj> mResouceObjDic = new Dictionary<int, TResouceObj>();

        //ResouceObj的类对象池  
        private SimpleObjectPool<TResouceObj> mResouceObjPool = new SimpleObjectPool<TResouceObj>(() => new TResouceObj(), initCount: 1000);
        //根据异步的guid储存ResouceObj,来判断是否正在异步加载
        private Dictionary<long, TResouceObj> mAsyncResObjs = new Dictionary<long, TResouceObj>();
        ObjectManager() { }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rectcleTrs">回收节点</param>
        /// <param name="sceneTrs">场景默认节点</param>
        public void Init(Transform rectcleTrs, Transform sceneTrs)
        {
            RecyclePoolTrs = rectcleTrs;
            this.SceneTrs = sceneTrs;
        }
        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearCache()
        {
            List<uint> tempList = new List<uint>();
            foreach (uint key in mObjectPoolDic.Keys)
            {
                List<TResouceObj> st = mObjectPoolDic[key];
                for (int i = st.Count - 1; i >= 0; i--)
                {
                    TResouceObj resObj = st[i];
                    if (!System.Object.ReferenceEquals(resObj.CloneObj, null) && resObj.Clear)
                    {
                        GameObject.Destroy(resObj.CloneObj);
                        mResouceObjDic.Remove(resObj.CloneObj.GetInstanceID());
                        resObj.Reset();
                        mResouceObjPool.Recycle(resObj);
                        st.Remove(resObj);
                    }
                }
                if (st.Count <= 0)
                {
                    tempList.Add(key);
                }
            }
            for (int i = 0; i < tempList.Count; i++)
            {
                uint temp = tempList[i];
                if (mObjectPoolDic.ContainsKey(temp))
                {
                    mObjectPoolDic.Remove(temp);
                }
            }
            tempList.Clear();
        }

        public void ClearPoolObject(uint crc)
        {
            List<TResouceObj> st = null;
            if (!mObjectPoolDic.TryGetValue(crc, out st) || st == null)
                return;
            for (int i = st.Count - 1; i >= 0; i--)
            {
                TResouceObj resObj = st[i];
                if (resObj.Clear)
                {
                    st.Remove(resObj);
                    int tempID = resObj.CloneObj.GetInstanceID();
                    GameObject.Destroy(resObj.CloneObj);
                    resObj.Reset();
                    mResouceObjDic.Remove(tempID);
                    mResouceObjPool.Recycle(resObj);
                }
            }
            if (st.Count <= 0)
            {
                mObjectPoolDic.Remove(crc);
            }
        }

        /// <summary>
        /// 根据实例化对象直接获取离线数据
        /// </summary>
        /// <param name="obj">GameObject</param>
        /// <returns></returns>
        public OfflineData FindOfflineData(GameObject obj)
        {
            OfflineData data = null;
            TResouceObj resObj = null;
            mResouceObjDic.TryGetValue(obj.GetInstanceID(), out resObj);
            if (resObj != null)
            {
                data = resObj.OffData;
            }
            return data;
        }


        /// <summary>
        ///  从对象池取对象
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        protected TResouceObj GetObjectFromPool(uint crc)
        {
            List<TResouceObj> st = null;
            if (mObjectPoolDic.TryGetValue(crc, out st) && st != null && st.Count > 0)
            {
                ResourcesManager.Instance.IncreaseResouceRef(crc);
                TResouceObj resObj = st[0];
                st.RemoveAt(0);
                GameObject obj = resObj.CloneObj;
                if (!ReferenceEquals(obj, null))
                {
                    if (!ReferenceEquals(resObj.OffData, null))
                    {
                        resObj.OffData.ResetProp();
                    }
                    resObj.Already = false;
#if UNITY_EDITOR
                    if (obj.name.EndsWith("(Recycle)"))
                    {
                        obj.name = obj.name.Replace("(Recycle)", "");
                    }
#endif
                }
                return resObj;
            }
            return null;
        }

        /// <summary>
        /// 取消异步加载
        /// </summary>
        /// <param name="guid"></param>
        public void CancleLoad(long guid)
        {
            TResouceObj resObj = null;
            if (mAsyncResObjs.TryGetValue(guid, out resObj) && ResourcesManager.Instance.CancleLoad(resObj))
            {
                mAsyncResObjs.Remove(guid);
                resObj.Reset();
                mResouceObjPool.Recycle(resObj);
            }
        }
        /// <summary>
        /// 是否正在异步加载
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool IsingAsyncLoad(long guid)
        {
            return mAsyncResObjs[guid] != null;
        }
        /// <summary>
        /// 该对象是否是对象池创建的
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsObjectManagerCreat(GameObject obj)
        {
            TResouceObj resObj = mResouceObjDic[obj.GetInstanceID()];
            return resObj == null ? false : true;
        }
        /// <summary>
        /// 预加载Gamobject
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">预加载个数</param>
        /// <param name="clear">跳场景是否清除</param>
        public void PreloadGameObject(string path, int count = 1, bool clear = false)
        {
            List<GameObject> tempGameObjectList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = InstantiateObject(path, false,clear);
                tempGameObjectList.Add(obj);
            }

            for (int i = 0; i < count; i++)
            {
                GameObject obj = tempGameObjectList[i];
                ReleaseObject(obj);
                obj = null;
            }

            tempGameObjectList.Clear();
        }
        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="setSceneObj">是否设置Parent为SceneTrs</param>
        /// <param name="bClear">跳场景是否清除</param>
        /// <returns></returns>
        public GameObject InstantiateObject(string path, bool setSceneParent = false, bool bClear = true)
        {
            uint crc = CRC32.GetCRC32(path);
            TResouceObj resouceObj = GetObjectFromPool(crc);
            if (resouceObj == null)
            {
                resouceObj = mResouceObjPool.Allocate();
                resouceObj.Crc = crc;
                resouceObj.Clear = bClear;
              
                //ResouceManager提供加载方法
                resouceObj = ResourcesManager.Instance.LoadResouce(path, resouceObj);


                if (resouceObj.ResItem.Obj != null)
                {
                    resouceObj.CloneObj = GameObject.Instantiate(resouceObj.ResItem.Obj) as GameObject;
                    resouceObj.OffData = resouceObj.CloneObj.GetComponent<OfflineData>();
                    resouceObj.OffData.ResetProp();
                }

            }
            if (setSceneParent)
            {
                resouceObj.CloneObj.transform.SetParent(SceneTrs, false);
            }
            int tempID = resouceObj.CloneObj.GetInstanceID();
            if (!mResouceObjDic.ContainsKey(tempID))
            {
                mResouceObjDic.Add(tempID, resouceObj);
            }

            return resouceObj.CloneObj;
        }
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="deaFinish">资源加载完成回调</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="setSceneParent">是否设置Parent为SceneTrs</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="bClear">跳场景是否清除</param>
        public long InstantiateObjectAsync(string path, OnAsyncObjFinish deaFinish, LoadResPriority priority, bool setSceneParent = false,
            object param1 = null, object param2 = null, object param3 = null, bool bClear = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }
            uint crc = CRC32.GetCRC32(path);
            TResouceObj resObj = GetObjectFromPool(crc);
            if (resObj != null)
            {
                if (setSceneParent)
                {
                    resObj.CloneObj.transform.SetParent(SceneTrs, false);
                }
                deaFinish?.Invoke(path, resObj.CloneObj, param1, param2, param3);
                return resObj.Guid;
            }
            long guid = ResourcesManager.Instance.CreatGuid();

            resObj = mResouceObjPool.Allocate();
            resObj.Crc = crc;
            resObj.SetSceneParent = setSceneParent;
            resObj.Clear = bClear;
            resObj.DealFinish = deaFinish;
            resObj.Param1 = param1;
            resObj.Param2 = param2;
            resObj.Param3 = param3;
            //调用ResouceManager的异步加载接口
            ResourcesManager.Instance.AsyncLoadResouce(path, resObj, OnLoadResouceObjFinish, priority);
            return guid;
        }
        /// <summary>
        /// 资源加载完成回调
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="resObj">中间类</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        void OnLoadResouceObjFinish(string path, TResouceObj resObj, object param1 = null, object param2 = null, object param3 = null)
        {
            if (resObj == null)
                return;
            if (resObj.ResItem.Obj == null)
            {
#if UNITY_EDITOR
                Debug.LogError("异步资源加载的资源为空:" + path);
#endif
            }
            else
            {
                resObj.CloneObj = GameObject.Instantiate(resObj.ResItem.Obj) as GameObject;
                resObj.OffData = resObj.CloneObj.GetComponent<OfflineData>();
            }
            //加载完成就从正在加载的异步中移除
            if (mAsyncResObjs.ContainsKey(resObj.Guid))
            {
                mAsyncResObjs.Remove(resObj.Guid);
            }
            if (resObj.CloneObj != null && resObj.SetSceneParent)
            {
                resObj.CloneObj.transform.SetParent(SceneTrs, false);
            }
            if (resObj.DealFinish != null)
            {
                int tempID = resObj.CloneObj.GetInstanceID();
                if (!mResouceObjDic.ContainsKey(tempID))
                {
                    mResouceObjDic.Add(tempID, resObj);
                }
                //else
                //{
                //    resouceObjDic[tempID] = resObj;
                //}
                resObj.DealFinish(path, resObj.CloneObj, resObj.Param1, resObj.Param2, resObj.Param3);
            }
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="maxCacheCount">最大的缓存数量，负数不限缓存数量，0代表不缓存</param>
        /// <param name="destroyCache">是否将ResouceItem从缓存引用计数的资源列表中移除</param>
        /// <param name="recycleParent">是否设置Parent为RecyclePoolTrs</param>
        public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destroyCache = false, bool recycleParent = true)
        {
            if (obj == null)
                return;
            TResouceObj resObj = null;
            int tempID = obj.GetInstanceID();
            if (!mResouceObjDic.TryGetValue(tempID, out resObj))
            {
                Debug.LogError(obj.name + "对象不是ObjectManager创建的!");
                return;
            }
            if (resObj == null)
            {
                Debug.LogError("缓存的ResouceObj为空");
                return;
            }
            if (resObj.Already)
            {
                Debug.LogError("该对象已经放回对象池了,检测自己是否清空引用");
                return;
            }
#if UNITY_EDITOR
            obj.name += "(Recycle)";
#endif
            List<TResouceObj> st = null;
            if (maxCacheCount == 0)
            {
                mResouceObjDic.Remove(tempID);
                ResourcesManager.Instance.ReleaseResouce(resObj, destroyCache);
                resObj.Reset();
                mResouceObjPool.Recycle(resObj);
            }
            else //回收到对象池
            {
                if (!mObjectPoolDic.TryGetValue(resObj.Crc, out st) || st == null)
                {
                    st = new List<TResouceObj>();
                    mObjectPoolDic.Add(resObj.Crc, st);
                }
                if (resObj.CloneObj)
                {
                    if (recycleParent)
                    {
                        resObj.CloneObj.transform.SetParent(RecyclePoolTrs);
                    }
                    else
                    {
                        resObj.CloneObj.SetActive(false);
                    }
                }
                if (maxCacheCount < 0 || st.Count < maxCacheCount)
                {
                    st.Add(resObj);
                    resObj.Already = true;
                    ResourcesManager.Instance.DecreaseResouceRef(resObj);
                }
                else
                {
                    mResouceObjDic.Remove(tempID);
                    ResourcesManager.Instance.ReleaseResouce(resObj, destroyCache);
                    resObj.Reset();
                    mResouceObjPool.Recycle(resObj);
                }
            }
        }

    }
}