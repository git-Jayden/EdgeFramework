/****************************************************
	文件：SheetManagerEx.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/12 11:49   	
	Features：SheetManager的拓展功能
*****************************************************/
using UnityEngine;


using System.Collections.Generic;
using EdgeFramework;
using EdgeFramework.Sheet;
using EdgeFramework.Res;

public partial class SheetManager : Singleton<SheetManager>
{
    public const string PREFIX = "Sheets/";
    public const string POSTFIX = ".bytes";
    /// <summary>
    /// 获得表格数据
    /// </summary>
    private T GetSheetInfo<T>(string fileName)
    {
        var text = ResourcesManager.Instance.LoadResouce<TextAsset>(Utility.TextHelp.Concat(PREFIX, fileName, POSTFIX));
        return Utility.ProtobufHelp.NDeserialize<T>(text.bytes);
    }

    //================================================================================
    private Dictionary<int, List<Preload>> mPreloadSheetDict;
    /// <summary>
    /// 根据场景Id获得预加载内容
    /// </summary>
    public List<Preload> GetPreloadSheet(int sceneId)
    {
        if (mPreloadSheetDict == null)
        {
            mPreloadSheetDict = new Dictionary<int, List<Preload>>();
            var preloadList = GetPreloadList();
            foreach (var item in preloadList)
            {
                if (!mPreloadSheetDict.ContainsKey(item.SceneId))
                {
                    mPreloadSheetDict.Add(item.SceneId, new List<Preload>());
                }
                mPreloadSheetDict[item.SceneId].Add(item);
            }
        }
        List<Preload> list = null;
        mPreloadSheetDict.TryGetValue(sceneId, out list);
        return list;
    }
    //================================================================================
}
