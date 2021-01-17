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
        var text = ResourcesManager.Instance.LoadResouce<TextAsset>("Assets/ABResources/Data/" + Utility.TextHelper.Concat(PREFIX, fileName, POSTFIX));
        return Utility.ProtobufHelper.NDeserialize<T>(text.bytes);
    }

    //================================================================================
    private Dictionary<int, List<PreloadSheet>> mPreloadSheetsDict;
    /// <summary>
    /// 根据场景Id获得预加载内容
    /// </summary>
    public List<PreloadSheet> GetPreloadSheets(int sceneId)
    {
        if (mPreloadSheetsDict == null)
        {
            mPreloadSheetsDict = new Dictionary<int, List<PreloadSheet>>();
            var preloadList = GetPreloadSheetList();
            foreach (var item in preloadList)
            {
                if (!mPreloadSheetsDict.ContainsKey(item.SceneId))
                {
                    mPreloadSheetsDict.Add(item.SceneId, new List<PreloadSheet>());
                }
                mPreloadSheetsDict[item.SceneId].Add(item);
            }
        }
        List<PreloadSheet> list = null;
        mPreloadSheetsDict.TryGetValue(sceneId, out list);
        return list;
    }
    //================================================================================
}
