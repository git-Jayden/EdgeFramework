/****************************************************
	文件：MenumanagerEditor.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/12 10:42   	
	Features：
*****************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;
using EdgeFramework;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EdgeFrameworkEditor
{
    class MenumanagerEditor
    {
        #region OpenTool
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Tools + "/AbConfig _F6", false)]
        public static void MenuOpenAbConfig()
        {
            ABConfig instance = ABConfig.LoadAbConfig();
            if (instance == null)
            {

                // Create Resources folder if it doesn't exist.
                EdgeFrameworkConst.ResourcesFolder.CreateDirIfNotExists();
                // Now create the asset inside the Resources folder.
                instance = ABConfig.Instance; // this will create a new instance of the EMSettings scriptable.
                AssetDatabase.CreateAsset(instance, EdgeFrameworkConst.AbConfigPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("AbConfig.asset was created at " + EdgeFrameworkConst.AbConfigPath);
            }
            Selection.activeObject = instance;
        }

        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Tools + "/OpenPanelExcel _F7", false)]
        public static void MenuOpenPanelJson()
        {

            string panelpath = Application.dataPath + "/../Excels/xlsx/UIPanel.xlsx";

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = panelpath;
            p.Start();
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        }
        private const string DLLPATH = "Assets/ABResources/Data/HotFix/Hotfix.dll";
        private const string PDBPATH = "Assets/ABResources/Data/HotFix/Hotfix.pdb";
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Tools + "/修改热更dll为bytes _F7", false)]
        public static void ChangeDllName()
        {
            if (File.Exists(DLLPATH))
            {
                string targetPath = DLLPATH + ".bytes";
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Move(DLLPATH, targetPath);
            }
            if (File.Exists(PDBPATH))
            {
                string targetPath = PDBPATH + ".bytes";
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Move(PDBPATH, targetPath);
            }
            AssetDatabase.Refresh();
        }
        #endregion

        #region AssetBundle
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.AssetsBundle + "/BuildBundle _F8")]
        public static void NormalBuild()
        {
            BundleEditor.Build();
        }
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.AssetsBundle + "/CopyBundleToStreamAssets _F10")]
        public static void CopyBundle()
        {
            string StreamingAssets = Application.streamingAssetsPath + "/AssetBundle/";
            string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            if (Directory.Exists(StreamingAssets))
                Utility.FileHelper.DeleteDir(StreamingAssets);
            else
                Directory.CreateDirectory(StreamingAssets);
            Utility.FileHelper.CopyFileTo(abPath, StreamingAssets);
        }
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.AssetsBundle + "/DeleteStreamAssets _F11")]
        public static void DeleteBundle()
        {
            string StreamingAssets = Application.streamingAssetsPath + "/AssetBundle/";
            Utility.FileHelper.DeleteDir(StreamingAssets);
        }
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.AssetsBundle + "/BuildApp _F12")]
        public static void Build()
        {
            BundleEditor.Build();
            BuildApp.Build();
        }

        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.AssetsBundle + "/打包热更包")]
        public static void OpenHotfix()
        {
            BundleHotFix.Init();
        }
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.AssetsBundle + "/SaveVersion")]
        public static void SaveVersion()
        {
            BuildApp.SaveVersion(PlayerSettings.bundleVersion, PlayerSettings.applicationIdentifier);
        }
        #endregion

        #region Encryption

        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Encryption + "/ 加密AB包")]
        public static void EncryptAB()
        {
            DirectoryInfo directory = new DirectoryInfo(BundleEditor.BundleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    AES.AESFileEncrypt(files[i].FullName, EdgeFrameworkConst.AESKEY);
                }
            }
            Debug.Log("加密完成！");
        }

        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Encryption + "/解密AB包")]
        public static void DecrptyAB()
        {
            DirectoryInfo directory = new DirectoryInfo(BundleEditor.BundleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    AES.AESFileDecrypt(files[i].FullName, "liangsheng");
                }
            }
            Debug.Log("解密完成！");
        }
        #endregion

        #region  Sheet
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Sheet + "/ExportBytes")]
        private static void ExportBytes()
        {
            SheetEditor.ExportBytes();
        }
        [MenuItem(EdgeFrameworkConst.ProductName + "/" + EdgeFrameworkConst.Sheet + "/ExportLua")]
        private static void ExportLua()
        {
            SheetEditor.ExportLua();
        }
        #endregion

        private static string START_SCENE = "Assets/Scenes/StartScene.unity";
        /// <summary>
        /// 游戏开始快捷键
        /// </summary>
        [MenuItem(EdgeFrameworkConst.ProductName + "/Play _F5")]
        private static void Play()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            else
            {
                AssetDatabase.Refresh();
                Scene activeScene = EditorSceneManager.GetActiveScene();
                if (!string.IsNullOrEmpty(activeScene.path) && (activeScene.path != START_SCENE)) EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene(START_SCENE);
                EditorApplication.isPlaying = true;
            }
        }
    }

}