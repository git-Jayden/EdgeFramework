/****************************************************
	文件：ABConfig.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 17:01   	
	Features：
*****************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFrameworkEditor
{
    [CreateAssetMenu(fileName = "AbConfig", menuName = "CreateABConfig", order = 0)]
    public class ABConfig : ScriptableObject
    {
        private static ABConfig sInstance;
        public static ABConfig Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = LoadAbConfig();

                    if (sInstance == null)
                    {
#if !UNITY_EDITOR
                        Debug.LogError("Ls_Mobile AbConfig not found! " +
                            "Please go to menu Ls_Mobile > Settings to setup the plugin.");
#endif
                        sInstance = CreateInstance<ABConfig>();   // Create a dummy scriptable object for temporary use.
                    }
                }

                return sInstance;
            }
        }
        public static ABConfig LoadAbConfig()
        {
            return Resources.Load("AbConfig") as ABConfig;
        }
        //单个文件所在文件夹路径，会遍历这个文件夹下所有Prefab,所有的Prefab名字不能重复,必须保证名字唯一性
        public List<string> allPrefabPath = new List<string>();
        public List<FileDirABName> allFileDirAb = new List<FileDirABName>();

        [System.Serializable]
        public struct FileDirABName
        {
            public string abName;
            public string path;
        }
    }
}
