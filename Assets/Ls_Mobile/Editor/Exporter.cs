using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Ls_Mobile
{
    public class Exporter
    {
#if UNITY_EDITOR

           [MenuItem("Ls_Mobile/ExportUnityPackage %e")]
        static void ExportClicked()
        {
            
            var assetPathName = "Assets/Ls_Mobile";
            var filePath = Application.dataPath+ "/../UnityPackage";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var fileName = filePath + "/Ls_Mobile" + DateTime.Now.ToString("yyyMMdd_HH") + ".unitypackage";
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            //GUIUtility.systemCopyBuffer = "Ls_SytleFrame_" + DateTime.Now.ToString("yyyMMdd_HH");//将字符串拷贝到剪切板
            //EditorApplication.ExecuteMenuItem("Ls_SytleFrame/ExportUnityPackage");--MenuItem的复用操作 表示可以执行按钮ExportUnityPackage的操作
            Application.OpenURL("file://"+ filePath);
        }
#endif
    }
}