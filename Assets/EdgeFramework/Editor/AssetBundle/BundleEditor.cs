using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using EdgeFramework;


namespace EdgeFrameworkEditor
{

    public class BundleEditor
    {
        //打包的AssetBundle路径
        public static string bundleTargetPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();//Application.streamingAssetsPath;                                                                                                                               //打包的AssetBundle版本路径
        private static string versionMd5Path = Application.dataPath + "/../Version/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        private static string hotPath = Application.dataPath + "/../Hot/" + EditorUserBuildSettings.activeBuildTarget.ToString();
 


        //key是ab包，value是路径,所有文件夹ab包的dic
        static Dictionary<string, string> allFileDir = new Dictionary<string, string>();
        //过滤List 
        static List<string> allFileAB = new List<string>();
        //单个prefab的ab包
        static Dictionary<string, List<string>> allPrefabDir = new Dictionary<string, List<string>>();
        //储存所有有效路径
        static List<string> configFil = new List<string>();
        //储存读出来MD5信息
        private static Dictionary<string, ABMD5Base> packedMd5 = new Dictionary<string, ABMD5Base>();
        public static void Build(bool hotfix = false, string abmd5Path = "", string hotCount = "1", string des = "")
        {
            if (string.IsNullOrEmpty(Constants.ABBYTEPATH))
            {
                Debug.LogError("RealFramConfig中未配置abBytePath路径！！！");
                return;
            }
            // DataEditor.AllXmlToBinary();
            allFileAB.Clear();
            allFileDir.Clear();
            allPrefabDir.Clear();
            configFil.Clear();
            ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(Constants.AbConfigPath);
            if (abConfig.allFileDirAb.Count <= 0 && abConfig.allPrefabPath.Count <= 0)
            {
                Debug.LogError("请在菜单栏EdgeFramework->OpenTool->AbConfig中配置需要打包AssetBundle的文件");
                return;
            }
            if (abConfig.allFileDirAb.Count > 0)
            {
                foreach (ABConfig.FileDirABName fileDir in abConfig.allFileDirAb)
                {
                    //Debug.Log(fileDir.Path);
                    if (allFileDir.ContainsKey(fileDir.abName))
                    {
                        Debug.LogError("Ab包配置名字重复,请检查!");
                    }
                    else
                    {
                        string datapath = Application.dataPath;
                        datapath = datapath.Replace("Assets", "");
                        if (!Directory.Exists(datapath + fileDir.path))
                        {
                            Debug.LogError("All File Dir Ab中不存在" + fileDir.abName + "路径," + fileDir.path);
                        }
                        else
                        {
                            allFileDir.Add(fileDir.abName, fileDir.path);
                            allFileAB.Add(fileDir.path);
                            configFil.Add(fileDir.path);
                        }
                    }
                }
            }
            if (abConfig.allPrefabPath.Count > 0)
            {
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
                            Debug.LogError("存在相同名字的Prefab!Prefab:" + path);
                        else
                            allPrefabDir.Add(obj.name, allDependPath);
                    }
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
            if (hotfix)
            {
                ReadMd5Com(abmd5Path, hotCount, des);
            }
            else
            {
                WriteABMD5();
            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

        }
        static void WriteABMD5()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(bundleTargetPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            ABMD5 abmd5 = new ABMD5();
            abmd5.ABMD5List = new List<ABMD5Base>();
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    ABMD5Base abmd5Base = new ABMD5Base();
                    abmd5Base.Name = files[i].Name;
                    abmd5Base.Md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                    abmd5Base.Size = files[i].Length / 1024.0f;
                    abmd5.ABMD5List.Add(abmd5Base);
                }
            }
            string ABMD5Path = Application.dataPath + "/Resources/ABMD5.bytes";
            SerializeHelper.SerializeBinary(ABMD5Path, abmd5);
    
            //将打版的版本拷贝到外部进行储存
            if (!Directory.Exists(versionMd5Path))
            {
                Directory.CreateDirectory(versionMd5Path);
            }
            string targetPath = versionMd5Path + "/ABMD5_" + PlayerSettings.bundleVersion + ".bytes";
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            File.Copy(ABMD5Path, targetPath);
        }
        static void ReadMd5Com(string abmd5Path, string hotCount,string des)
        {
            packedMd5.Clear();
            using (FileStream fileStream = new FileStream(abmd5Path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ABMD5 abmd5 = new ABMD5();
                abmd5.ABMD5List = new List<ABMD5Base>();
               if (fileStream.Length!=0)
                    abmd5 = bf.Deserialize(fileStream) as ABMD5;
                foreach (ABMD5Base abmd5Base in abmd5.ABMD5List)
                {

                    packedMd5.Add(abmd5Base.Name, abmd5Base);
                }
            }

            List<string> changeList = new List<string>();
            DirectoryInfo directory = new DirectoryInfo(bundleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    string name = files[i].Name;
                    string md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                    ABMD5Base abmd5Base = null;
                    if (!packedMd5.ContainsKey(name))
                    {
                        changeList.Add(name);
                    }
                    else
                    {
                        if (packedMd5.TryGetValue(name, out abmd5Base))
                        {
                            if (md5 != abmd5Base.Md5)
                            {
                                changeList.Add(name);
                            }
                        }
                    }
                }
            }

            CopyABAndGeneratXml(changeList, hotCount,des);
        }
        /// <summary>
        /// 拷贝筛选的AB包及自动生成服务器配置表
        /// </summary>
        /// <param name="changeList"></param>
        /// <param name="hotCount"></param>
        static void CopyABAndGeneratXml(List<string> changeList, string hotCount,string des)
        {
            if (!Directory.Exists(hotPath))
            {
                Directory.CreateDirectory(hotPath);
            }
            DeleteAllFile(hotPath);
            foreach (string str in changeList)
            {
                if (!str.EndsWith(".manifest"))
                {
                    File.Copy(bundleTargetPath + "/" + str, hotPath + "/" + str);
                }
            }

            //生成服务器Patch
            DirectoryInfo directory = new DirectoryInfo(hotPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            Pathces pathces = new Pathces();
            pathces.Version = int.Parse(hotCount);
            pathces.Files = new List<Patch>();
            for (int i = 0; i < files.Length; i++)
            {
                Patch patch = new Patch();
                patch.Md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                patch.Name = files[i].Name;
                patch.Size = files[i].Length / 1024.0f;
                patch.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
                patch.Url = AppConfig.HTTPServerIP+"/AssetBundle/" + PlayerSettings.bundleVersion + "/" + hotCount + "/" + files[i].Name;
                pathces.Files.Add(patch);
            }
            SerializeHelper.SerializeXML(hotPath + "/Patch.xml", pathces);
        }

        static void SetABName(string name, List<string> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                SetABName(name, path[i]);
            }

        }
        static void SetABName(string name, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
                Debug.Log("不存在此路径文件:" + path);
            else
                assetImporter.assetBundleName = name;
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
                    resPathDic.Add(allBundlePath[j], allBundles[i]);
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
            //删除manifest文件
            DeleteMainfest();
            //加密AB包
            MenumanagerEditor.EncryptAB();
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
            //EdgeFramework.Utils.FileUtil
            string DirPath="";
            if (Constants.ABBYTEPATH.Contains("."))
                DirPath= Constants.ABBYTEPATH.Substring(0, Constants.ABBYTEPATH.LastIndexOf('.'));
            DirPath =  DirPath.Substring(0, DirPath.LastIndexOf('/'));
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            //写二进制
            FileStream fs = new FileStream(Constants.ABBYTEPATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, config);
            fs.Close();
            AssetDatabase.Refresh();
            SetABName("assetbundleconfig", Constants.ABBYTEPATH);

        }
        static void DeleteMainfest()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(bundleTargetPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".manifest"))
                {
                    File.Delete(files[i].FullName);
                }
            }
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


        /// <summary>
        /// 删除指定文件目录下的所有文件
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static void DeleteAllFile(string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo directory = new DirectoryInfo(fullPath);
                FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    File.Delete(files[i].FullName);
                }
            }
        }
    }
}