﻿/**
 * SheetManager的拓展功能
 */


using UnityEngine;
using EdgeFramework.Utils;
using System.Collections.Generic;
using EdgeFramework;

public partial class SheetManager : Singleton<SheetManager>
{
    public const string PREFIX = "Assets/ABResources/Data/Sheets/";
    public const string POSTFIX = ".bytes";
    /// <summary>
    /// 获得表格数据
    /// </summary>
    private T GetSheetInfo<T>(string fileName)
    {
          var text = ResourcesManager.Instance.LoadResouce<TextAsset>(StringUtil.Concat(PREFIX, fileName, POSTFIX));

        return ProtobufUtil.NDeserialize<T>(text.bytes);
    }

    //================================================================================
    //private Dictionary<int, List<Preload>> mPreloadSheetDict;
    ///// <summary>
    ///// 根据场景Id获得预加载内容
    ///// </summary>
    //public List<Preload> GetPreloadSheet(int sceneId)
    //{
    //    if (mPreloadSheetDict == null)
    //    {
    //        mPreloadSheetDict = new Dictionary<int, List<Preload>>();
    //        var preloadList = GetPreloadList();
    //        foreach (var item in preloadList)
    //        {
    //            if (!mPreloadSheetDict.ContainsKey(item.SceneId))
    //            {
    //                mPreloadSheetDict.Add(item.SceneId, new List<Preload>());
    //            }
    //            mPreloadSheetDict[item.SceneId].Add(item);
    //        }
    //    }
    //    List<Preload> list = null;
    //    mPreloadSheetDict.TryGetValue(sceneId, out list);
    //    return list;
    //}
    //================================================================================
}
