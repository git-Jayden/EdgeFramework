using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
namespace Ls_Mobile
{
    public class BundleHotFix : EditorWindow
    {
        [MenuItem("Ls_Mobile/Tool/打包热更包")]
        static void Init()
        {
            BundleHotFix window = (BundleHotFix)GetWindow(typeof(BundleHotFix), false, "热更包界面", true);
            window.Show();
        }
        string md5Path = "";
        string hotCount = "1";
        OpenFileName openFileName = null;

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            md5Path = EditorGUILayout.TextField("ABMD5路径： ", md5Path, GUILayout.Width(350), GUILayout.Height(20));
            if (GUILayout.Button("选择版本ABMD5文件", GUILayout.Width(150), GUILayout.Height(30)))
            {
                openFileName = new OpenFileName();
                openFileName.structSize = Marshal.SizeOf(openFileName);
                openFileName.filter = "ABMD5文件(*.bytes)\0*.bytes";
                openFileName.file = new string(new char[256]);
                openFileName.maxFile = openFileName.file.Length;
                openFileName.fileTitle = new string(new char[64]);
                openFileName.maxFileTitle = openFileName.fileTitle.Length;
                openFileName.initialDir = (Application.dataPath + "/../Version").Replace("/", "\\");//默认路径
                openFileName.title = "选择MD5窗口";
                openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
                if (LocalDialog.GetSaveFileName(openFileName))
                {
                    Debug.Log(openFileName.file);
                    md5Path = openFileName.file;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            hotCount = EditorGUILayout.TextField("热更补丁版本：", hotCount, GUILayout.Width(350), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("开始打热更包", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (!string.IsNullOrEmpty(md5Path) && md5Path.EndsWith(".bytes"))
                {
                    BundleEditor.Build(true, md5Path, hotCount);
                }
            }
        }
    }
   
}