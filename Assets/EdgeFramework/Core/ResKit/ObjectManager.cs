using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EdgeFramework
{
    public class ObjectManager : Singleton<ObjectManager>
    {

        public Transform recyclePoolTrs;
        //场景节点 
        public Transform sceneTrs;
        //对象池 
        protected Dictionary<uint, List<ResouceObj>> objectPoolDic = new Dictionary<uint, List<ResouceObj>>();
        //暂存ResObj的Dic
        protected Dictionary<int, ResouceObj> resouceObjDic = new Dictionary<int, ResouceObj>();

        //ResouceObj的类对象池  
        protected SimpleObjectPool<ResouceObj> resouceObjPool = new SimpleObjectPool<ResouceObj>(() => new ResouceObj(), initCount: 1000);
        //根据异步的guid储存ResouceObj,来判断是否正在异步加载
        protected Dictionary<long, ResouceObj> asyncResObjs = new Dictionary<long, ResouceObj>();
        ObjectManager() { }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rectcleTrs">回收节点</param>
        /// <param name="sceneTrs">场景默认节点</param>
        public void Init(Transform rectcleTrs, Transform sceneTrs)
        {
            recyclePoolTrs = rectcleTrs;
            this.sceneTrs = sceneTrs;
        }
        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearCache()
        {
            List<uint> tempList = new List<uint>();
            foreach (uint key in objectPoolDic.Keys)
            {
                List<ResouceObj> st = objectPoolDic[key];
                for (int i = st.Count - 1; i >= 0; i--)
                {
                    ResouceObj resObj = st[i];
                    if (!System.Object.ReferenceEquals(resObj.cloneObj, null) && resObj.clear)
                    {
                        GameObject.Destroy(resObj.cloneObj);
                        resouceObjDic.Remove(resObj.cloneObj.GetInstanceID());
                        resObj.Reset();
                        resouceObjPool.Recycle(resObj);
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
                if (objectPoolDic.ContainsKey(temp))
                {
                    objectPoolDic.Remove(temp);
                }
            }
            tempList.Clear();
        }

        public void ClearPoolObject(uint crc)
        {
            List<ResouceObj> st = null;
            if (!objectPoolDic.TryGetValue(crc, out st) || st == null)
                return;
            for (int i = st.Count - 1; i >= 0; i--)
            {
                ResouceObj resObj = st[i];
                if (resObj.clear)
                {
                    st.Remove(resObj);
                    int tempID = resObj.cloneObj.GetInstanceID();
                    GameObject.Destroy(resObj.cloneObj);
                    resObj.Reset();
                    resouceObjDic.Remove(tempID);
                    resouceObjPool.Recycle(resObj);
                }
            }
            if (st.Count <= 0)
            {
                objectPoolDic.Remove(crc);
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
            ResouceObj resObj = null;
            resouceObjDic.TryGetValue(obj.GetInstanceID(), out resObj);
            if (resObj != null)
            {
                data = resObj.offlineData;
            }
            return data;
        }


        /// <summary>
        ///  从对象池取对象
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        protected ResouceObj GetObjectFromPool(uint crc)
        {
            List<ResouceObj> st = null;
            if (objectPoolDic.TryGetValue(crc, out st) && st != null && st.Count > 0)
            {
                ResourcesManager.Instance.IncreaseResouceRef(crc);
                ResouceObj resObj = st[0];
                st.RemoveAt(0);
                GameObject obj = resObj.cloneObj;
                if (!ReferenceEquals(obj, null))
                {
                    if (!ReferenceEquals(resObj.offlineData, null))
                    {
                        resObj.offlineData.ResetProp();
                    }
                    resObj.already = false;
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
            ResouceObj resObj = null;
            if (asyncResObjs.TryGetValue(guid, out resObj) && ResourcesManager.Instance.CancleLoad(resObj))
            {
                asyncResObjs.Remove(guid);
                resObj.Reset();
                resouceObjPool.Recycle(resObj);
            }
        }
        /// <summary>
        /// 是否正在异步加载
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool IsingAsyncLoad(long guid)
        {
            return asyncResObjs[guid] != null;
        }
        /// <summary>
        /// 该对象是否是对象池创建的
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsObjectManagerCreat(GameObject obj)
        {
            ResouceObj resObj = resouceObjDic[obj.GetInstanceID()];
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
                GameObject obj = InstantiateObject(path, false, bClear: clear);
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
        /// <param name="path"></param>
        /// <param name="bClear"></param>
        /// <returns></returns>
        public GameObject InstantiateObject(string path, bool setSceneObj = false, bool bClear = true)
        {
            uint crc = CRC32.GetCRC32(path);
            ResouceObj resouceObj = GetObjectFromPool(crc);
            if (resouceObj == null)
            {
                resouceObj = resouceObjPool.Allocate();
                resouceObj.crc = crc;
                resouceObj.clear = bClear;
              
                //ResouceManager提供加载方法
                resouceObj = ResourcesManager.Instance.LoadResouce(path, resouceObj);


                if (resouceObj.resItem.obj != null)
                {
                    resouceObj.cloneObj = GameObject.Instantiate(resouceObj.resItem.obj) as GameObject;
                    resouceObj.offlineData = resouceObj.cloneObj.GetComponent<OfflineData>();
                    resouceObj.offlineData.ResetProp();
                }

            }
            if (setSceneObj)
            {
                resouceObj.cloneObj.transform.SetParent(sceneTrs, false);
            }
            int tempID = resouceObj.cloneObj.GetInstanceID();
            if (!resouceObjDic.ContainsKey(tempID))
            {
                resouceObjDic.Add(tempID, resouceObj);
            }

            return resouceObj.cloneObj;
        }
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="deaFinish"></param>
        /// <param name="priority"></param>
        /// <param name="SetSceneObject"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="bClear"></param>
        public long InstantiateObjectAsync(string path, OnAsyncObjFinish deaFinish, LoadResPriority priority, bool SetSceneObject = false,
            object param1 = null, object param2 = null, object param3 = null, bool bClear = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }
            uint crc = CRC32.GetCRC32(path);
            ResouceObj resObj = GetObjectFromPool(crc);
            if (resObj != null)
            {
                if (SetSceneObject)
                {
                    resObj.cloneObj.transform.SetParent(sceneTrs, false);
                }
                deaFinish?.Invoke(path, resObj.cloneObj, param1, param2, param3);
                return resObj.guid;
            }
            long guid = ResourcesManager.Instance.CreatGuid();

            resObj = resouceObjPool.Allocate();
            resObj.crc = crc;
            resObj.setSceneParent = SetSceneObject;
            resObj.clear = bClear;
            resObj.dealFinish = deaFinish;
            resObj.param1 = param1;
            resObj.param2 = param2;
            resObj.param3 = param3;
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
        void OnLoadResouceObjFinish(string path, ResouceObj resObj, object param1 = null, object param2 = null, object param3 = null)
        {
            if (resObj == null)
                return;
            if (resObj.resItem.obj == null)
            {
#if UNITY_EDITOR
                Debug.LogError("异步资源加载的资源为空:" + path);
#endif
            }
            else
            {
                resObj.cloneObj = GameObject.Instantiate(resObj.resItem.obj) as GameObject;
                resObj.offlineData = resObj.cloneObj.GetComponent<OfflineData>();
            }
            //加载完成就从正在加载的异步中移除
            if (asyncResObjs.ContainsKey(resObj.guid))
            {
                asyncResObjs.Remove(resObj.guid);
            }
            if (resObj.cloneObj != null && resObj.setSceneParent)
            {
                resObj.cloneObj.transform.SetParent(sceneTrs, false);
            }
            if (resObj.dealFinish != null)
            {
                int tempID = resObj.cloneObj.GetInstanceID();
                if (!resouceObjDic.ContainsKey(tempID))
                {
                    resouceObjDic.Add(tempID, resObj);
                }
                //else
                //{
                //    resouceObjDic[tempID] = resObj;
                //}
                resObj.dealFinish(path, resObj.cloneObj, resObj.param1, resObj.param2, resObj.param3);
            }
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="maxCacheCount"></param>
        /// <param name="destroyCache"></param>
        /// <param name="recycleParent"></param>
        public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destroyCache = false, bool recycleParent = true)
        {
            if (obj == null)
                return;
            ResouceObj resObj = null;
            int tempID = obj.GetInstanceID();
            if (!resouceObjDic.TryGetValue(tempID, out resObj))
            {
                Debug.LogError(obj.name + "对象不是ObjectManager创建的!");
                return;
            }
            if (resObj == null)
            {
                Debug.LogError("缓存的ResouceObj为空");
                return;
            }
            if (resObj.already)
            {
                Debug.LogError("该对象已经放回对象池了,检测自己是否清空引用");
                return;
            }
#if UNITY_EDITOR
            obj.name += "(Recycle)";
#endif
            List<ResouceObj> st = null;
            if (maxCacheCount == 0)
            {
                resouceObjDic.Remove(tempID);
                ResourcesManager.Instance.ReleaseResouce(resObj, destroyCache);
                resObj.Reset();
                resouceObjPool.Recycle(resObj);
            }
            else //回收到对象池
            {
                if (!objectPoolDic.TryGetValue(resObj.crc, out st) || st == null)
                {
                    st = new List<ResouceObj>();
                    objectPoolDic.Add(resObj.crc, st);
                }
                if (resObj.cloneObj)
                {
                    if (recycleParent)
                    {
                        resObj.cloneObj.transform.SetParent(recyclePoolTrs);
                    }
                    else
                    {
                        resObj.cloneObj.SetActive(false);
                    }
                }
                if (maxCacheCount < 0 || st.Count < maxCacheCount)
                {
                    st.Add(resObj);
                    resObj.already = true;
                    ResourcesManager.Instance.DecreaseResouceRef(resObj);

                }
                else
                {
                    resouceObjDic.Remove(tempID);
                    ResourcesManager.Instance.ReleaseResouce(resObj, destroyCache);
                    resObj.Reset();
                    resouceObjPool.Recycle(resObj);
                }
            }
        }

    }
}