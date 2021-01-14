/****************************************************
	文件：DownLoadAssetBundle.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:56   	
	Features：
*****************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace EdgeFramework.Res
{
    public class DownLoadAssetBundle : DownLoadItem
    {
        UnityWebRequest webRequest;
        public DownLoadAssetBundle(string url, string path) : base(url, path)
        {

        }
        public override IEnumerator Download(Action callback = null)
        {
            webRequest = UnityWebRequest.Get(url);
            startDownLoad = true;
            //超时30秒后尝试中止。
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();
            startDownLoad = false;
            if (webRequest.isNetworkError)
            {
                Debug.LogError("Download Error" + webRequest.error);
            }
            else
            {
                byte[] bytes = webRequest.downloadHandler.data;


                Utility.FileHelper.WriteAllBytes(saveFilePath, bytes);
                callback?.Invoke();
            }
        }
        public override void Destory()
        {
            if (webRequest != null)
            {
                webRequest.Dispose();
                webRequest = null;
            }
        }
        public override long GetCurLength()
        {
            if (webRequest != null)
            {
                return (long)webRequest.downloadedBytes;
            }
            return 0;
        }
        public override long GetLength()
        {
            return 0;
        }
        public override float GetProcess()
        {
            if (webRequest != null)
            {
                return (long)webRequest.downloadProgress;
            }
            return 0;
        }
    }
}