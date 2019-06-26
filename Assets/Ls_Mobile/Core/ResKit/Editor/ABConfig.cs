using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile
{
    [CreateAssetMenu(fileName = "AbConfig", menuName = "CreateABConfig", order = 0)]
    public class ABConfig : ScriptableObject
    {

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
