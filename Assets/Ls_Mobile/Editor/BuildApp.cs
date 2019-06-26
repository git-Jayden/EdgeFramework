using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Ls_Mobile {
    public class BuildApp
    {
        private static string appName = PlayerSettings.productName;
        public static string androidPath = Application.dataPath + "/../BuildTarget/Android/";
        public static string iOSPath = Application.dataPath + "/../BuildTarget/IOS/";
        public static string windowsPath = Application.dataPath + "/../BuildTarget/Windows/";

        [MenuItem("Ls_Mobile/Tool/BuildApp()")]
        public static void Build()
        {
            //打Ab包
            BundleEditor.Build();
             //生成可执行程序
            string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            Copy(abPath, Application.streamingAssetsPath);
            string savePath = "";
            if (!Directory.Exists(androidPath))
                Directory.CreateDirectory(androidPath);
            if (!Directory.Exists(iOSPath))
                Directory.CreateDirectory(iOSPath);
            if (!Directory.Exists(windowsPath))
                Directory.CreateDirectory(windowsPath);

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                savePath = androidPath + appName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now)+ ".apk";
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
           DeleteDir(Application.streamingAssetsPath);
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
                Debug.LogError("无法复制:" + scrPath + "  到" + targetPath);
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
    }
}