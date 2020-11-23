
using UnityEngine;
using UnityEditor;
using EdgeFramework;

namespace EdgeFramework
{
    public class OfflineDataEditor
    {
        [MenuItem("Assets/生成Transform数据")]
        public static void AssetCreateOfflineData()
        {
            Object[] arr = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);

            //GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < arr.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加离线数据", "正在修改:" + AssetDatabase.GetAssetPath(arr[i]) + "......", 1.0f / arr.Length * i);
                CreateOfflineData(AssetDatabase.GetAssetPath(arr[i]));
            }
            EditorUtility.ClearProgressBar();
        }
        public static void CreateOfflineData(string path)
        {
            GameObject obj = PrefabUtility.LoadPrefabContents(path);

            OfflineData offlineData = obj.GetComponent<OfflineData>();
            if (offlineData == null)
            {
                offlineData = obj.AddComponent<OfflineData>();
            }
            offlineData.BindData();
            EditorUtility.SetDirty(obj);
            Debug.Log("修改了" + obj.name + "prefab!");
            if (PrefabUtility.SaveAsPrefabAsset(obj, path))
                PrefabUtility.UnloadPrefabContents(obj);
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("Assets/生成UI Prefab数据")]
        public static void AssetCreateUIData()
        {
            Object[] arr = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);

            //GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < arr.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加UI离线数据", "正在修改:" + AssetDatabase.GetAssetPath(arr[i]) + "......", 1.0f / arr.Length * i);
                CreateUIData(AssetDatabase.GetAssetPath(arr[i]));
            }
            EditorUtility.ClearProgressBar();
        }
        [MenuItem("Assets/生成文件夹下UI Prefab数据")]
        public static void AllCreateUIData()
        {
            string[] strs = Selection.assetGUIDs;
            for (int i = 0; i < strs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(strs[i]);
                string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
                for (int j = 0; j < allStr.Length; j++)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[j]);
                    EditorUtility.DisplayProgressBar("添加UI离线数据", "正在扫描路径:" + prefabPath + "......", 1.0f / allStr.Length * j);
                    //GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (string.IsNullOrEmpty(prefabPath))
                        continue;
                    CreateUIData(prefabPath);
                }
                Debug.Log("UI数据全部生成完毕");
                EditorUtility.ClearProgressBar();
            }
            //string path = "Assets/GameData/Prefabs/UGUI";
        }
        public static void CreateUIData(string path)
        {
            GameObject obj = PrefabUtility.LoadPrefabContents(path);
            obj.layer = LayerMask.NameToLayer("UI");
            UIOfflineData uiData = obj.GetComponent<UIOfflineData>();
            if (uiData == null)
            {
                uiData = obj.AddComponent<UIOfflineData>();
            }
            uiData.BindData();
            EditorUtility.SetDirty(obj);
            Debug.Log("修改了" + obj.name + "prefab!");
            if (PrefabUtility.SaveAsPrefabAsset(obj, path))
                PrefabUtility.UnloadPrefabContents(obj);
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("Assets/生成特效数据")]
        public static void AssetCreateEffectData()
        {
            Object[] arr = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);
            //GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < arr.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加特效数据", "正在修改:" + AssetDatabase.GetAssetPath(arr[i]) + "......", 1.0f / arr.Length * i);
                CreateEffectData(AssetDatabase.GetAssetPath(arr[i]));
            }
            EditorUtility.ClearProgressBar();
        }
        [MenuItem("Assets/生成文件夹下特效数据")]
        public static void AllCreateEffectData()
        {
            string[] strs = Selection.assetGUIDs;
            for (int i = 0; i < strs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(strs[i]);
                string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
                for (int j = 0; j < allStr.Length; j++)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
                    EditorUtility.DisplayProgressBar("添加特效数据", "正在扫描路径:" + prefabPath + "......", 1.0f / allStr.Length * j);
                    //GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (string.IsNullOrEmpty(prefabPath))
                        continue;
                    CreateEffectData(prefabPath);
                }
                Debug.Log("特效数据全部生成完毕");
                EditorUtility.ClearProgressBar();
            }
            //string path = "Assets/GameData/Prefabs/UGUI";
        }
        public static void CreateEffectData(string path)
        {
            GameObject obj = PrefabUtility.LoadPrefabContents(path);
            if (obj != null)
            {
                EffectOfflineData effectData = obj.GetComponent<EffectOfflineData>();
                if (effectData == null)
                {
                    effectData = obj.AddComponent<EffectOfflineData>();
                }
                effectData.BindData();
                EditorUtility.SetDirty(obj);
                Debug.Log("修改了" + obj.name + "特效 prefab!");
                if (PrefabUtility.SaveAsPrefabAsset(obj, path))
                    PrefabUtility.UnloadPrefabContents(obj);
                Resources.UnloadUnusedAssets();
                AssetDatabase.Refresh();
            }

        }

    }
}
