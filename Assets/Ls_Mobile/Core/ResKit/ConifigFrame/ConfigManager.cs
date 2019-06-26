using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ls_Mobile{
    public class ConfigManager : Singleton<ConfigManager>
    {
        /// <summary>
        /// 储存所有已经加载的配置表
        /// </summary>
        protected Dictionary<string, ExcelBase> allExcelData = new Dictionary<string, ExcelBase>();

        public T LoadData<T>(string path) where T : ExcelBase
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (allExcelData.ContainsKey(path))
            {
                Debug.LogError("重复加载相同配置文件" + path);
                return allExcelData[path] as T;
            }
            
           T data = BinarySerializeOpt.BinaryDeSerialize<T>(path);

#if UNITY_EDITOR
            if (data == null)
            {
                Debug.Log(path + "不存在，从xml加载数据了！");
                string xmlPath = path.Replace("Binary", "Xml").Replace(".bytes", ".xml");
                data = BinarySerializeOpt.XmlDeserialize<T>(xmlPath);
            }
#endif
            if (data != null)
            {
                data.Init();
            }
            allExcelData.Add(path, data);
            return data;
        }
        /// <summary>
        /// 根据路径查找数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T FindData<T>(string path) where T : ExcelBase
        {
            if (string.IsNullOrEmpty(path))
                return null;

            ExcelBase excelBase = null;
            if (allExcelData.TryGetValue(path, out excelBase))
            {
                return excelBase as T;
            }
            else
            {
                excelBase = LoadData<T>(path);
            }

            return (T)excelBase;
        }

    }
    public class CFG
    {
        //配置表路径
        public const string TABLE_MONSTER = "Assets/GameData/Data/Binary/MonsterData.bytes";
        public const string TABLE_BUFF = "Assets/GameData/Data/Binary/BuffData.bytes";
    }
}
