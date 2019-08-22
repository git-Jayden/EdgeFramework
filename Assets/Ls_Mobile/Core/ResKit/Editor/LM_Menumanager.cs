using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Ls_Mobile
{
    public class LM_Menumanager
    {
        #region Setting
        [MenuItem(Constants.ProductName + "/" + Constants.Settings + "/AbConfig", false)]
        public static void MenuOpenAbConfig()
        {
            ABConfig instance = ABConfig.LoadAbConfig();
            if (instance == null)
            {

                // Create Resources folder if it doesn't exist.
                Constants.ResourcesFolder.CreateDirIfNotExists();
                // Now create the asset inside the Resources folder.
                instance = ABConfig.Instance; // this will create a new instance of the EMSettings scriptable.
                AssetDatabase.CreateAsset(instance, Constants.AbConfigPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("AbConfig.asset was created at " + Constants.AbConfigPath);
            }
            Selection.activeObject = instance;
        }
        [MenuItem(Constants.ProductName + "/" + Constants.Settings + "/RealFramConfig", false)]
        public static void MenuOpenRealFramConfig()
        {
            RealFramConfig instance = RealFramConfig.LoadRealFramConfig();
            if (instance == null)
            {

                // Create Resources folder if it doesn't exist.
                Constants.ResourcesFolder.CreateDirIfNotExists();
                // Now create the asset inside the Resources folder.
                instance = RealFramConfig.Instance; // this will create a new instance of the EMSettings scriptable.
                AssetDatabase.CreateAsset(instance, Constants.RealFramConfigPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("RealFramConfig.asset was created at " + Constants.RealFramConfigPath);
            }
            Selection.activeObject = instance;
        }


        [MenuItem(Constants.ProductName + "/" + Constants.Settings + "/OpenPanelJson", false)]
        public static void MenuOpenPanelJson()
        {
            string datapath = Application.dataPath.Replace("Assets", "");
            string panelpath = datapath  + Constants.UIJsonPath;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = panelpath;
            p.Start();
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        }
        [MenuItem(Constants.ProductName + "/" + Constants.Settings + "/OpenAudioConfig", false)]
        public static void MenuOpenAudioConfig()
        {
            string datapath = Application.dataPath.Replace("Assets", "");
            string panelpath = datapath + Constants.AudioConfig;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = panelpath;
            p.Start();
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        }
        #endregion

        #region AssetBundle
        [MenuItem("Ls_Mobile/Tool/加密AB包")]
        public static void EncryptAB()
        {
            DirectoryInfo directory = new DirectoryInfo(BundleEditor.bundleTargetPath);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    AES.AESFileEncrypt(files[i].FullName, "liangsheng");
                }
            }
            Debug.Log("加密完成！");
        }

        [MenuItem("Ls_Mobile/Tool/解密AB包")]
        public static void DecrptyAB()
        {
            DirectoryInfo directory = new DirectoryInfo(BundleEditor.bundleTargetPath);
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

        [MenuItem("Ls_Mobile/Tool/BuildBundle")]
        public static void NormalBuild()
        {
            BundleEditor.Build();
        }
        #endregion
    }
}