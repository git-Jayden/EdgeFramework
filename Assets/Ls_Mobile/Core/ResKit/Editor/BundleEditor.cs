using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ls_Mobile
{
    public  class BundleEditor
    {
        static string bundleTargetPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();//Application.streamingAssetsPath;
        static string abBytePath = RealConfig.GetRealFram().aBBytePath;

        //key是ab包，value是路径,所有文件夹ab包dic
        static Dictionary<string, string> allFileDir = new Dictionary<string, string>();
        //过滤List
         static List<string> allFileAB = new List<string>();
        //单个prefab的ab包
         static Dictionary<string, List<string>> allPrefabDir = new Dictionary<string, List<string>>();

        //储存所有有效路径
         static List<string> configFil = new List<string>();

        [MenuItem("Ls_Mobile/Tool/BuildBundleForAndroid")]
        public static void Build()
        {
            DataEditor.AllXmlToBinary();
            allFileAB.Clear();
            allFileDir.Clear();
            allPrefabDir.Clear();
            configFil.Clear();
            ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ConStr.AbConfig);
            foreach (ABConfig.FileDirABName fileDir in abConfig.allFileDirAb)
            {
                //Debug.Log(fileDir.Path);
                if (allFileDir.ContainsKey(fileDir.abName))
                {
                    Debug.LogError("Ab包配置名字重复,请检查!");
                }
                else
                {
                    allFileDir.Add(fileDir.abName, fileDir.path);
                    allFileAB.Add(fileDir.path);
                    configFil.Add(fileDir.path);
                }
            }
            string[] allStr = AssetDatabase.FindAssets("t:Prefab", abConfig.allPrefabPath.ToArray());
            for (int i = 0; i < allStr.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
                EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / allStr.Length);
                configFil.Add(path);
                if (!ContainAllFileAB(path))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    string[] allDepentd = AssetDatabase.GetDependencies(path);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepentd.Length; j++)
                    {
                        // Debug.Log(allDepentd[j]);
                        if (!ContainAllFileAB(allDepentd[j]) && !allDepentd[j].EndsWith(".cs"))
                        {
                            allFileAB.Add(allDepentd[j]);
                            allDependPath.Add(allDepentd[j]);
                        }
                    }
                    if (allPrefabDir.ContainsKey(obj.name))
                        Debug.LogError("存在相同名字的Prefab!名字:" + obj.name);
                    else
                        allPrefabDir.Add(obj.name, allDependPath);
                }
            }
            foreach (string name in allFileDir.Keys)
            {
                SetABName(name, allFileDir[name]);
            }
            foreach (var name in allPrefabDir.Keys)
            {
                SetABName(name, allPrefabDir[name]);
            }


            BunildAssetBundle();


            string[] oldAbNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < oldAbNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAbNames[i], true);
                EditorUtility.DisplayProgressBar("清除Ab包名", "名字:" + oldAbNames, i * 1.0f / oldAbNames.Length);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
        static void SetABName(string name, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
              Debug.Log("不存在此路径文件:" + path);
            else
                assetImporter.assetBundleName = name;
        }
        static void SetABName(string name, List<string> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                SetABName(name, path[i]);
            }

        }
        static void BunildAssetBundle()
        {
            string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
            //Key为全路径，Value为包名
            Dictionary<string, string> resPathDic = new Dictionary<string, string>();
            for (int i = 0; i < allBundles.Length; i++)
            {
                string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
                for (int j = 0; j < allBundlePath.Length; j++)
                {
                    if (allBundlePath[j].EndsWith(".cs"))
                        continue;
                    Debug.Log("此AB包：" + allBundles[i] + "下面包含的资源文件路径：" + allBundlePath[j]);
                    resPathDic.Add(allBundlePath[j], allBundles[i]);
                    //if (ValiPath(allBundlePath[j]))
                    //{
                    //    resPathDic.Add(allBundlePath[j], allBundles[i]);
                    //}
                }
            }

            if (!Directory.Exists(bundleTargetPath))
            {
                Directory.CreateDirectory(bundleTargetPath);
            }
            DeleteAB();
            //生成自己的配置表
            WriteData(resPathDic);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(bundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            if (manifest == null)
            {
                Debug.LogError("AssetBundle 打包失败！");
            }
            else
            {
                Debug.Log("AssetBundle 打包完毕");
            }
        }
        static void WriteData(Dictionary<string, string> resPathDic)
        {
            AssetBundleConfig config = new AssetBundleConfig();
            config.abList = new List<ABBase>();
            foreach (var path in resPathDic.Keys)
            {
                if (!ValidPath(path))
                    continue;

                ABBase abBase = new ABBase();
                abBase.path = path;
                abBase.crc = CRC32.GetCRC32(path);
                abBase.abName = resPathDic[path];
                abBase.assetName = path.Remove(0, path.LastIndexOf("/") + 1);
                abBase.abDependce = new List<string>();
                string[] resDependce = AssetDatabase.GetDependencies(path);
                for (int i = 0; i < resDependce.Length; i++)
                {
                    string tempPath = resDependce[i];
                    if (tempPath == path || path.EndsWith(".cs"))
                        continue;
                    string abName = "";
                    if (resPathDic.TryGetValue(tempPath, out abName))
                    {
                        if (abName == resPathDic[path])
                            continue;
                        if (!abBase.abDependce.Contains(abName))
                        {
                            abBase.abDependce.Add(abName);
                        }
                    }
                }
                config.abList.Add(abBase);
            }
            //写入xml
            string xmlPath = Application.dataPath + "/AssetbundleCofig.xml";
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
            FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(config.GetType());
            xs.Serialize(sw, config);
            sw.Close();
            fileStream.Close();
            foreach (ABBase ab in config.abList)
            {
                ab.path = "";
            }
            //写二进制
            FileStream fs = new FileStream(abBytePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, config);
            fs.Close();
            AssetDatabase.Refresh();
            SetABName("assetbundleconfig", abBytePath);

        }
        /// <summary>
        /// 删除无用AB包
        /// </summary>
        static void DeleteAB()
        {
            string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
            DirectoryInfo direction = new DirectoryInfo(bundleTargetPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (ConatinABName(files[i].Name, allBundlesName) || files[i].Name.EndsWith(".mate") || files[i].Name.EndsWith(".manifest") || files[i].Name.EndsWith("assetbundleconfig"))
                    continue;
                else
                {
                    Debug.Log("此AB包已经被删或者改名字:" + files[i].Name);
                    if (File.Exists(files[i].FullName))
                    {
                        File.Delete(files[i].FullName);
                    }
                    if (File.Exists(files[i].FullName + ".manifest"))
                    {
                        File.Delete(files[i].FullName + ".manifest");
                    }
                }
            }
        }
        /// <summary>
        /// 遍历文件夹里的文件名与设置的所有AB包进行检查判断
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        static bool ConatinABName(string name, string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (name == strs[i])
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 是否包含在已经有的AB包里，用来做AB包冗余剔除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool ContainAllFileAB(string path)
        {
            for (int i = 0; i < allFileAB.Count; i++)
            {
                if (path == allFileAB[i] || (path.Contains(allFileAB[i]) && (path.Replace(allFileAB[i], "")[0] == '/')))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 是否有效路径
        /// </summary>
        /// <returns></returns>
        static bool ValidPath(string path)
        {
            for (int i = 0; i < configFil.Count; i++)
            {
                if (path.Contains(configFil[i]))
                    return true;

            }
            return false;
  
        }
    }
}

