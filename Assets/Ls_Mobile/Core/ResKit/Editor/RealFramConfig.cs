using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Ls_Mobile
{
    //[CreateAssetMenu(fileName ="RealFramConfig",menuName ="CreateRealFramConfig",order =0)]
    public class RealFramConfig : ScriptableObject
    {
        //打包时生成AB包配置表的二进制路径
        public string aBBytePath;
        //xml文件夹路径
        public string xmlPath;
        //二进制文件夹路径
        public string binaryPath;
        //脚本文件夹路径
        public string scriptsPath;
    }
    [CustomEditor(typeof(RealFramConfig))]
    public class RealFramConfigInspector : Editor
    {
        public SerializedProperty aBBytePath;
        public SerializedProperty xmlPath;
        public SerializedProperty binaryPath;
        public SerializedProperty scriptsPath;

        private void OnEnable()
        {
            aBBytePath = serializedObject.FindProperty("aBBytePath");
            xmlPath = serializedObject.FindProperty("xmlPath");
            binaryPath = serializedObject.FindProperty("binaryPath");
            scriptsPath = serializedObject.FindProperty("scriptsPath");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(aBBytePath, new GUIContent("ab包二进制路径"));
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(xmlPath, new GUIContent("Xml路径"));
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(binaryPath, new GUIContent("二进制路径"));
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(scriptsPath, new GUIContent("配置表脚本路径"));
            GUILayout.Space(5);
            serializedObject.ApplyModifiedProperties();
        }
    }
    public class RealConfig
    {
        public static RealFramConfig GetRealFram()
        {
            RealFramConfig realConfig = AssetDatabase.LoadAssetAtPath<RealFramConfig>(ConStr.RealFramPath);
            return realConfig;
        }
    }
}