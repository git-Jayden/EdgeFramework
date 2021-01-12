/****************************************************
	文件：BuildApp.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 17:02   	
	Features：
*****************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using EdgeFramework;

namespace EdgeFrameworkEditor
{
    public class BuildApp 
    {
        private static string appName = PlayerSettings.productName;
        public static string androidPath = Application.dataPath + "/../BuildTarget/Android/";
        public static string iOSPath = Application.dataPath + "/../BuildTarget/IOS/";
        public static string windowsPath = Application.dataPath + "/../BuildTarget/Windows/";
        public static string streamingAssets = Application.streamingAssetsPath + "/AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        public static void Build()
        {
            SaveVersion(PlayerSettings.bundleVersion, PlayerSettings.applicationIdentifier);
            AppConfig.UseAssetBundle=true;
            //生成可执行程序
            string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            if(!Directory.Exists(streamingAssets))
                Directory.CreateDirectory(streamingAssets);
            Copy(abPath, streamingAssets);
            string savePath = "";
            if (!Directory.Exists(androidPath))
                Directory.CreateDirectory(androidPath);
            if (!Directory.Exists(iOSPath))
                Directory.CreateDirectory(iOSPath);
            if (!Directory.Exists(windowsPath))
                Directory.CreateDirectory(windowsPath);

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                savePath = androidPath + appName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".apk";
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                savePath = iOSPath + appName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows || EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
            {
                savePath = windowsPath + appName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}/{1}.exe", DateTime.Now, appName);
            }

            BuildPipeline.BuildPlayer(FindEnableEditorScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            DeleteDir(streamingAssets);
        }
      public   static void SaveVersion(string version, string package)
        {
            string content = "Version|" + version + ";PackageName|" + package + ";";
            string savePath = Application.dataPath + "/Resources/Version.txt";
            string oneLine = "";
            string all = "";
            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
                {
                    all = sr.ReadToEnd();
                    oneLine = all.Split('\r')[0];
                }
            }
            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    if (string.IsNullOrEmpty(all))
                    {
                        all = content;
                    }
                    else
                    {
                        all = all.Replace(oneLine, content);
                    }
                    sw.Write(all);
                    Debug.Log("Save Version Success,Version:"+ version+ ",PackageName:"+ package);
                }
            }
        }
        private static string[] FindEnableEditorScenes()
        {
            List<string> editorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                editorScenes.Add(scene.path);
            }
            return editorScenes.ToArray();
        }
        /// <summary>
        /// 将外层相应平台的的AssetBundle Copy到StreamAssets中
        /// </summary>
        /// <param name="scrPath"></param>
        /// <param name="targetPath"></param>
        private static void Copy(string scrPath, string targetPath)
        {
            try
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                string scrdir = Path.Combine(targetPath, Path.GetFileName(scrPath));
                if (Directory.Exists(scrPath))
                    scrdir += Path.DirectorySeparatorChar;
                if (!Directory.Exists(scrdir))
                    Directory.CreateDirectory(scrdir);
                string[] files = Directory.GetFileSystemEntries(scrPath);
                foreach (var file in files)
                {
                    if (Directory.Exists(file))
                        Copy(file, scrdir);
                    else
                    {
                        File.Copy(file, scrdir + Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("无法复制:" + scrPath + "  到" + targetPath+ "---"+e);
            }
        }
        /// <summary>
        /// 移除StreamAssets中的AssetBundle文件
        /// </summary>
        /// <param name="scrPath"></param>
        public static void DeleteDir(string scrPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(scrPath);
                FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo info in fileInfo)
                {
                    if (info is DirectoryInfo)
                    {
                        DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                        subdir.Delete(true);
                    }
                    else
                    {
                        File.Delete(info.FullName);
                    }
                }
            }
            catch (Exception e)
            {

                Debug.LogError(e);
            }
        }

        #region 打包机调用打包pc版本
        //[MenuItem("Ls_Mobile/Tool/BuildPC()")]
        public static void BuildPC()
        {
            //打Ab包
            MenumanagerEditor.NormalBuild();
            BuildSetting buildSetting = GetPCBuildSetting();
            string suffix = SetSetting(buildSetting);
            //生成可执行程序
            string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            //清空生成的文件夹
            DeleteDir(windowsPath);
            Copy(abPath, Application.streamingAssetsPath);
            if (!Directory.Exists(windowsPath))
                Directory.CreateDirectory(windowsPath);
            string dir = appName + "_PC" + suffix + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
            string name = string.Format("/{0}.exe", appName);
            string savePath = windowsPath + dir + name;

            BuildPipeline.BuildPlayer(FindEnableEditorScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            DeleteDir(Application.streamingAssetsPath);
            WriteBuildName(dir);
        }
        /// <summary>
        /// 根据jenkins的参数读取到BuildSetting里
        /// </summary>
        /// <returns></returns>
        static BuildSetting GetPCBuildSetting()
        {
            string[] parameters = Environment.GetCommandLineArgs();
            BuildSetting buildSetting = new BuildSetting();
            foreach (var str in parameters)
            {
                if (str.StartsWith("Version"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Version = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Build"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Build = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Name"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Name = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Debug"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1], out buildSetting.Debug);
                    }
                }
            }

            return buildSetting;
        }
        /// <summary>
        /// 根据读取的数据在unity里面设置对应
        /// </summary>
        static string SetSetting(BuildSetting setting)
        {
            string suffix = "_";
            if (!string.IsNullOrEmpty(setting.Version))
            {
                PlayerSettings.bundleVersion = setting.Version;
                suffix += setting.Version;
            }
            if (!string.IsNullOrEmpty(setting.Build))
            {
                PlayerSettings.macOS.buildNumber = setting.Build;
                suffix += setting.Build;
            }
            if (!string.IsNullOrEmpty(setting.Name))
            {
                PlayerSettings.productName = setting.Name;

            }
            if (setting.Debug)
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.connectProfiler = true;
                suffix += "_Debug";
            }
            else
            {
                EditorUserBuildSettings.development = false;
            }
            return suffix;
        }
        #endregion

        #region 打包Android

        public static void BuildAndroid()
        {
            //打ab包
            MenumanagerEditor.NormalBuild();
            PlayerSettings.Android.keystorePass = "keystorePass";
            PlayerSettings.Android.keyaliasPass = "keyaliasPass";
            PlayerSettings.Android.keyaliasName = "android.keystore";
            PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + "/Ls_Mobile.keystore";
            BuildSetting buildSetting = GetAndoridBuildSetting();
            string suffix = SetAndroidSetting(buildSetting);
            //生成可执行程序
            string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            //清空生成的文件夹
            DeleteDir(androidPath);
            Copy(abPath, Application.streamingAssetsPath);
            string savePath = androidPath + appName + "_Andorid" + suffix + string.Format("_{0:yyyy_MM_dd_HH_mm}.apk", DateTime.Now);
            BuildPipeline.BuildPlayer(FindEnableEditorScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            DeleteDir(Application.streamingAssetsPath);

        }
        static BuildSetting GetAndoridBuildSetting()
        {
            string[] parameters = Environment.GetCommandLineArgs();
            BuildSetting buildSetting = new BuildSetting();
            foreach (string str in parameters)
            {
                if (str.StartsWith("Place"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Place = (Place)Enum.Parse(typeof(Place), tempParam[1], true);
                    }
                }
                else if (str.StartsWith("Version"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Version = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Build"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Build = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Name"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Name = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Debug"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1], out buildSetting.Debug);
                    }
                }
                else if (str.StartsWith("MulRendering"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                    }
                }
                else if (str.StartsWith("IL2CPP"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1], out buildSetting.IL2CPP);
                    }
                }
            }
            return buildSetting;
        }
        static string SetAndroidSetting(BuildSetting setting)
        {
            string suffix = "_";
            if (setting.Place != Place.None)
            {
                //代表了渠道包
                string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbol + ";" + setting.Place.ToString());
                suffix += setting.Place.ToString();
            }

            if (!string.IsNullOrEmpty(setting.Version))
            {
                PlayerSettings.bundleVersion = setting.Version;
                suffix += setting.Version;
            }
            if (!string.IsNullOrEmpty(setting.Build))
            {
                PlayerSettings.Android.bundleVersionCode = int.Parse(setting.Build);
                suffix += "_" + setting.Build;
            }
            if (!string.IsNullOrEmpty(setting.Name))
            {
                PlayerSettings.productName = setting.Name;
                //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
            }

            if (setting.MulRendering)
            {
                PlayerSettings.MTRendering = true;
                suffix += "_MTR";
            }
            else
            {
                PlayerSettings.MTRendering = false;
            }

            if (setting.IL2CPP)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                suffix += "_IL2CPP";
            }
            else
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            }

            if (setting.Debug)
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.connectProfiler = true;
                suffix += "_Debug";
            }
            else
            {
                EditorUserBuildSettings.development = false;
            }
            return suffix;
        }
        #endregion

        #region 打包IOS
        public static void BuildIOS()
        {
            //打ab包
            BundleEditor.Build();
            BuildSetting buildSetting = GetIOSBuildSetting();
            string suffix = SetIOSSetting(buildSetting);

            //生成可执行程序
            string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            //清空生成的文件夹
            DeleteDir(iOSPath);
            Copy(abPath, Application.streamingAssetsPath);
            string name = appName + "_IOS" + suffix + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
            string savePath = iOSPath + name;
            BuildPipeline.BuildPlayer(FindEnableEditorScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            DeleteDir(Application.streamingAssetsPath);
            WriteBuildName(name);
        }

        static BuildSetting GetIOSBuildSetting()
        {
            string[] parameters = Environment.GetCommandLineArgs();
            BuildSetting buildSetting = new BuildSetting();
            foreach (string str in parameters)
            {
                if (str.StartsWith("Version"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Version = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Build"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Build = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Name"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        buildSetting.Name = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("MulRendering"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                    }
                }
                else if (str.StartsWith("DynamicBatching"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1], out buildSetting.DynamicBatching);
                    }
                }
            }
            return buildSetting;
        }

        static string SetIOSSetting(BuildSetting setting)
        {
            string suffix = "_";

            if (!string.IsNullOrEmpty(setting.Version))
            {
                PlayerSettings.bundleVersion = setting.Version;
                suffix += setting.Version;
            }
            if (!string.IsNullOrEmpty(setting.Build))
            {
                PlayerSettings.iOS.buildNumber = setting.Build;
                suffix += "_" + setting.Build;
            }
            if (!string.IsNullOrEmpty(setting.Name))
            {
                PlayerSettings.productName = setting.Name;
                //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
            }

            if (setting.MulRendering)
            {
                PlayerSettings.MTRendering = true;
                suffix += "_MTR";
            }
            else
            {
                PlayerSettings.MTRendering = false;
            }

            if (setting.DynamicBatching)
            {
                suffix += "_Dynamic";
            }
            else
            {

            }

            return suffix;
        }
        #endregion
        public static void WriteBuildName(string name)
        {
            string path = Application.dataPath + "/../buildname.txt";
            FileInfo fileInfo = new FileInfo(path);
            StreamWriter sw = fileInfo.CreateText();
            sw.WriteLine(name);
            sw.Close();
            sw.Dispose();
        }
    }
    public class BuildSetting
    {
        //版本号
        public string Version = "";
        //build次数
        public string Build = "";
        //程序名称
        public string Name = "";
        //是否debug
        public bool Debug = true;
        //渠道
        public Place Place = Place.None;
        //多线程渲染
        public bool MulRendering = true;
        //是否IL2CPP
        public bool IL2CPP = false;
        //是否开启动态合批
        public bool DynamicBatching = false;
    }
    public enum Place
    {
        None = 0,
        Xiaomi,
        Bilibili,
        Huawei,
        Meizu,
        Weixin,
    }
}