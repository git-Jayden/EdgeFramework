/****************************************************
	文件：HotPatchManager.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:57   	
	Features：
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
namespace EdgeFramework.Res
{
    public class HotPatchManager : Singleton<HotPatchManager>
    {
        MonoBehaviour mono;
        private string unPackPath = Application.persistentDataPath + "/Origin";
        //下载下来AssetBundle路径
        private string downLoadPath = Application.persistentDataPath + "/DownLoad";
        string curVersion;
        public string CurVersion
        {
            get { return curVersion; }
        }
        //当前版本的包名
        string curPackName;
        //服务器下载下来的配置数据存储路径
        string serverXmlPath = Application.persistentDataPath + "/ServerInfo.xml";
        //缓存上一次服务器上下载下来的配置(将与本地的配置对比)
        private string localXmlPath = Application.persistentDataPath + "/LocalInfo.xml";
        //下载下来的ServerInfo
        ServerInfo serverInfo;

        ServerInfo localInfo;
        //下载下来当前版本的VersionInfo
        VersionInfo gameVersion;

        //当前热更Patches
        private Pathces currentPatches;
        public Pathces CurrentPatches
        {
            get { return currentPatches; }
        }

        //所有热更的东西
        private Dictionary<string, Patch> hotFixDic = new Dictionary<string, Patch>();
        //所有需要下载的东西 下载列表
        private List<Patch> downLoadList = new List<Patch>();
        //所有需要下载的东西的Dic
        private Dictionary<string, Patch> downLoadDic = new Dictionary<string, Patch>();
        //服务器上的资源名对应的MD5，用于下载后MD5校验 key:Ab包名 value:MD5码
        private Dictionary<string, string> downLoadMD5Dic = new Dictionary<string, string>();

        //计算需要解压的文件
        private List<string> unPackedList = new List<string>();
        //原包记录的MD5码
        private Dictionary<string, ABMD5Base> packedMd5 = new Dictionary<string, ABMD5Base>();

        //服务器列表获取错误回调
        public Action ServerInfoError;
        //文件下载出错回调
        public Action<string> ItemError;
        //下载完成回调
        public Action LoadOver;
        //储存已经下载的资源
        public List<Patch> alreadyDownList = new List<Patch>();
        //是否开始下载
        public bool StartDownload = false;
        //尝试重新下载次数
        private int tryDownCount = 0;
        private const int downLoadCount = 4;
        //当前正在下载的资源
        private DownLoadAssetBundle curDownload = null;


        // 需要下载的资源总个数
        public int LoadFileCount { get; set; } = 0;
        // 需要下载资源的总大小 KB
        public float LoadSumSize { get; set; } = 0;
        //是否开始解压
        public bool StartUnPack = false;
        //解压文件总大小
        public float UnPackSumSize { get; set; } = 0;
        //已解压大小
        public float AlreadyUnPackSize { get; set; } = 0;
        HotPatchManager() { }
        public void Init(MonoBehaviour mono)
        {
            this.mono = mono;
            ReadMD5();
        }
        /// <summary>
        /// 读取本地资源MD5码
        /// </summary>
        void ReadMD5()
        {
            packedMd5.Clear();
            TextAsset md5 = Resources.Load<TextAsset>("ABMD5");
            if (md5 == null)
            {
                //Debug.LogError("未读取到本地MD5");
                return;
            }

            using (MemoryStream stream = new MemoryStream(md5.bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ABMD5 abmd5 = bf.Deserialize(stream) as ABMD5;
                foreach (ABMD5Base abmd5Base in abmd5.ABMD5List)
                {
                    packedMd5.Add(abmd5Base.Name, abmd5Base);
                }
            }
        }

        /// <summary>
        /// 计算需要解压的文件
        /// </summary>
        /// <returns></returns>
        public bool ComputeUnPackFile()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            if (!Directory.Exists(unPackPath))
            {
                Directory.CreateDirectory(unPackPath);
            }
            unPackedList.Clear();
            if (packedMd5.Count <= 0)
                return false;
            string streamingRootPath = Application.streamingAssetsPath + "/AssetBundle";
            if (!Directory.Exists(streamingRootPath))
                return false;
            foreach (string fileName in packedMd5.Keys)
            {
                string filePath = unPackPath + "/" + fileName;
                if (File.Exists(filePath))
                {
                    string md5 =Utility.FileHelper.GetMD5HashFromFile(filePath);
                    if (packedMd5[fileName].Md5 != md5)
                    {
                        unPackedList.Add(fileName);
                    }
                }
                else
                {
                    unPackedList.Add(fileName);
                }
            }

            foreach (string fileName in unPackedList)
            {
                if (packedMd5.ContainsKey(fileName))
                {
                    UnPackSumSize += packedMd5[fileName].Size;
                }
            }

            return unPackedList.Count > 0;
#else
            return false;
#endif
        }
        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        public float GetUnpackProgress()
        {
            return AlreadyUnPackSize / UnPackSumSize;
        }
        /// <summary>
        /// 开始解压文件
        /// </summary>
        /// <param name="callBack"></param>
        public void StartUnackFile(Action callBack)
        {
            StartUnPack = true;
            mono.StartCoroutine(UnPackToPersistentDataPath(callBack));
        }
        /// <summary>
        /// 将包里的原始资源解压到本地
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator UnPackToPersistentDataPath(Action callBack)
        {
            foreach (string fileName in unPackedList)
            {
                string streamingRootPath = Application.streamingAssetsPath + "/AssetBundle";
                if (!Directory.Exists(streamingRootPath))
                    Directory.CreateDirectory(streamingRootPath);
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(streamingRootPath + "/" + fileName);
                unityWebRequest.timeout = 30;
                yield return unityWebRequest.SendWebRequest();
                if (unityWebRequest.isNetworkError)
                {
                    Debug.Log("UnPack Error" + unityWebRequest.error);
                }
                else
                {
                    byte[] bytes = unityWebRequest.downloadHandler.data;
                    EdgeFramework.Utility.FileHelper.WriteAllBytes(unPackPath + "/" + fileName, bytes);
         
                }

                if (packedMd5.ContainsKey(fileName))
                {
                    AlreadyUnPackSize += packedMd5[fileName].Size;
                }

                unityWebRequest.Dispose();
            }

            if (callBack != null)
            {
                callBack();
            }

            StartUnPack = false;
        }

        /// <summary>
        /// 计算AB包路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ComputeABPath(string name)
        {
            Patch patch = null;
            hotFixDic.TryGetValue(name, out patch);
            if (patch != null)
            {
                return downLoadPath + "/" + name;
            }
            return "";
        }
        /// <summary>
        /// 检测版本是否热更
        /// </summary>
        /// <param name="hotCallBack"></param>
        public void CheckVersion(Action<bool> hotCallBack = null)
        {
            tryDownCount = 0;
            hotFixDic.Clear();
            ReadVersion();
            mono.StartCoroutine(ReadXml(() =>
            {
                if (serverInfo == null)
                {
                    ServerInfoError?.Invoke();
                    return;
                }
                foreach (VersionInfo version in serverInfo.GameVersion)
                {
                    if (version.Version == curVersion)
                    {
                        gameVersion = version;
                        break;
                    }
                }
                GetHotAB();
                if (CheckLocalAndServerPatch())
                {
                    ComputeDownload();
                    if (File.Exists(serverXmlPath))
                    {
                        if (File.Exists(localXmlPath))
                        {
                            File.Delete(localXmlPath);
                        }
                        File.Move(serverXmlPath, localXmlPath);
                    }
                }
                else
                {
                    ComputeDownload();
                }
                LoadFileCount = downLoadList.Count;
                LoadSumSize = downLoadList.Sum(x => x.Size);
                hotCallBack?.Invoke(downLoadList.Count > 0);
            }));
        }

        /// <summary>
        /// 检查本地热更信息与服务器热更信息比较
        /// </summary>
        /// <returns></returns>
        bool CheckLocalAndServerPatch()
        {
            if (!File.Exists(localXmlPath))
                return true;

            localInfo =Utility.SerializeHelper.DeserializeXML(localXmlPath, typeof(ServerInfo)) as ServerInfo;

            VersionInfo localGameVesion = null;
            if (localInfo != null)
            {
                foreach (VersionInfo version in localInfo.GameVersion)
                {
                    if (version.Version == curVersion)
                    {
                        localGameVesion = version;
                        break;
                    }
                }
            }

            if (localGameVesion != null && gameVersion.Pathces != null && localGameVesion.Pathces != null && gameVersion.Pathces.Length > 0 && gameVersion.Pathces[gameVersion.Pathces.Length - 1].Version != localGameVesion.Pathces[localGameVesion.Pathces.Length - 1].Version)
                return true;

            return false;
        }
        /// <summary>
        /// 读取打包时的版本
        /// </summary>
        void ReadVersion()
        {
            TextAsset versionTex = Resources.Load<TextAsset>("Version");
            if (versionTex == null)
            {
                Debug.LogError("未读到本地版本");
                return;
            }
            string[] all = versionTex.text.Split('\n');
            if (all.Length > 0)
            {
                string[] infoList = all[0].Split(';');
                if (infoList.Length >= 2)
                {
                    curVersion = infoList[0].Split('|')[1];
                    curPackName = infoList[1].Split('|')[1];
                }
            }
        }
        IEnumerator ReadXml(Action callBack)
        {
            string xmlUrl = AppConfig.HTTPServerIP+ "/ServerInfo.xml";
            UnityWebRequest webRequest = UnityWebRequest.Get(xmlUrl);
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.LogError("Download Error" + webRequest.error);
            }
            else
            {
                EdgeFramework.Utility.FileHelper.WriteAllBytes(serverXmlPath, webRequest.downloadHandler.data);

                if (File.Exists(serverXmlPath))
                {

                    serverInfo = Utility.SerializeHelper.DeserializeXML(serverXmlPath, typeof(ServerInfo)) as ServerInfo;

                }
                else
                {
                    Debug.LogError("热更配置读取错误!!!");
                }
            }
            callBack?.Invoke();
        }
        /// <summary>
        /// 获取所有热更包信息
        /// </summary>
        void GetHotAB()
        {
            if (gameVersion != null && gameVersion.Pathces != null && gameVersion.Pathces.Length > 0)
            {
                Pathces lastPatches = gameVersion.Pathces[gameVersion.Pathces.Length - 1];
                if (lastPatches != null && lastPatches.Files != null)
                {
                    foreach (Patch patch in lastPatches.Files)
                    {
                        hotFixDic.Add(patch.Name, patch);
                    }
                }
            }
        }
        /// <summary>
        /// 计算下载的资源
        /// </summary>
        void ComputeDownload()
        {
            downLoadList.Clear();
            downLoadDic.Clear();
            downLoadMD5Dic.Clear();
            if (gameVersion != null && gameVersion.Pathces != null && gameVersion.Pathces.Length > 0)
            {
                currentPatches = gameVersion.Pathces[gameVersion.Pathces.Length - 1];
                if (currentPatches.Files != null && currentPatches.Files.Count > 0)
                {
                    foreach (Patch patch in currentPatches.Files)
                    {
                        if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && (patch.Platform.Contains("StandaloneWindows64") || patch.Platform.Contains("StandaloneWindows")))
                        {
                            AddDownLoadList(patch);
                        }
                        else if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("Android"))
                        {
                            AddDownLoadList(patch);
                        }
                        else if ((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("IOS"))
                        {
                            AddDownLoadList(patch);

                        }
                    }
                }
            }
        }
        //添加assetbundle到下载列表
        void AddDownLoadList(Patch patch)
        {
            string filePath = downLoadPath + "/" + patch.Name;
            //存在这个文件时进行对比看是否与服务器MD5码一致，不一致放到下载队列，如果不存在直接放入下载队列
            if (File.Exists(filePath))
            {
                string md5 = Utility.FileHelper.GetMD5HashFromFile(filePath); 
                if (patch.Md5 != md5)
                {
                    downLoadList.Add(patch);
                    downLoadDic.Add(patch.Name, patch);
                    downLoadMD5Dic.Add(patch.Name, patch.Md5);
                }
            }
            else
            {
                downLoadList.Add(patch);
                downLoadDic.Add(patch.Name, patch);
                downLoadMD5Dic.Add(patch.Name, patch.Md5);
            }
        }

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public float GetProgress()
        {
            return GetLoadSize() / LoadSumSize;
        }
        /// <summary>
        /// 获取已经下载总大小
        /// </summary>
        /// <returns></returns>
        public float GetLoadSize()
        {
            float alreadySize = alreadyDownList.Sum(x => x.Size);
            float curAlreadySize = 0;
            if (curDownload != null)
            {
                Patch patch = FindPatchByGamePath(curDownload.FileName);
                if (patch != null && !alreadyDownList.Contains(patch))
                {
                    curAlreadySize = curDownload.GetProcess() * patch.Size;
                }
            }

            return alreadySize + curAlreadySize;
        }
        /// <summary>
        /// 开始下载AB包
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public IEnumerator StartDownLoadAB(Action callBack, List<Patch> allPatch = null)
        {
            alreadyDownList.Clear();
            StartDownload = true;
            if (allPatch == null)
            {
                allPatch = downLoadList;
            }
            if (!Directory.Exists(downLoadPath))
            {
                Directory.CreateDirectory(downLoadPath);
            }
            List<DownLoadAssetBundle> downLoadAssetBundles = new List<DownLoadAssetBundle>();
            foreach (Patch patch in allPatch)
            {
                downLoadAssetBundles.Add(new DownLoadAssetBundle(patch.Url, downLoadPath));
            }
            foreach (DownLoadAssetBundle downLoad in downLoadAssetBundles)
            {
                curDownload = downLoad;
                yield return mono.StartCoroutine(downLoad.Download());
                Patch patch = FindPatchByGamePath(downLoad.FileName);
                if (patch != null)
                {
                    alreadyDownList.Add(patch);
                }
                downLoad.Destory();
            }
            //MD5码校验,如果校验没通过，自动重新下载没通过的文件，重复下载计数，达到一定次数后，反馈某某文件下载失败
            VerifyMD5(downLoadAssetBundles, callBack);
        }
        /// <summary>
        /// Md5码校验 下载下来的文件md5码与服务器中的md5进行校验 如果不一致(代表没有完全下载),重新下载
        /// </summary>
        /// <param name="downLoadAssets"></param>
        /// <param name="callBack"></param>
        void VerifyMD5(List<DownLoadAssetBundle> downLoadAssets, Action callBack)
        {
            List<Patch> downLoadList = new List<Patch>();
            foreach (DownLoadAssetBundle downLoad in downLoadAssets)
            {
                string md5 = "";
                if (downLoadMD5Dic.TryGetValue(downLoad.FileName, out md5))
                {
                    if (Utility.FileHelper.GetMD5HashFromFile(downLoad.SaveFilePath)!= md5)
                    {
                        Debug.Log(string.Format("此文件{0}MD5校验失败，即将重新下载", downLoad.FileName));
                        Patch patch = FindPatchByGamePath(downLoad.FileName);
                        if (patch != null)
                        {
                            downLoadList.Add(patch);
                        }
                    }
                }

            }
            if (downLoadList.Count <= 0)
            {
                downLoadMD5Dic.Clear();
                if (callBack != null)
                {
                    StartDownload = false;
                    callBack();
                }
                if (LoadOver != null)
                {
                    LoadOver();
                }
            }
            else
            {
                if (tryDownCount >= downLoadCount)
                {
                    string allName = "";
                    StartDownload = false;
                    foreach (Patch patch in downLoadList)
                    {
                        allName += patch.Name + ";";
                    }
                    Debug.LogError("资源重复下载4次MD5校验都失败，请检查资源" + allName);
                    if (ItemError != null)
                    {
                        ItemError(allName);
                    }
                }
                else
                {
                    tryDownCount++;
                    downLoadMD5Dic.Clear();
                    foreach (Patch patch in downLoadList)
                    {
                        downLoadMD5Dic.Add(patch.Name, patch.Md5);
                    }
                    //自动重新下载校验失败的文件
                    mono.StartCoroutine(StartDownLoadAB(callBack, downLoadList));
                }
            }

        }
        /// <summary>
        /// 根据名字查找对象的热更Patch
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Patch FindPatchByGamePath(string name)
        {
            Patch patch = null;
            downLoadDic.TryGetValue(name, out patch);
            return patch;
        }
    }
}
