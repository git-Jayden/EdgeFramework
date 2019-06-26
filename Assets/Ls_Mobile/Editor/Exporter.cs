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
            //EditorApplication.ExecuteMenuItem("Ls_SytleFrame/ExportUnityPackage");--MenuItem的复用操作
            var assetPathName = "Assets/Ls_StyleFrame";
            var fileName = Application.dataPath + "/Ls_StyleFrame" + DateTime.Now.ToString("yyyMMdd_HH") + ".unitypackage";
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            //GUIUtility.systemCopyBuffer = "Ls_SytleFrame_" + DateTime.Now.ToString("yyyMMdd_HH");
            Application.OpenURL("file://"+Path.Combine(Application.dataPath, "../Assets/"));
        }
#endif
    }
}