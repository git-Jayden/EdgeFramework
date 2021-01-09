/**
 * Tool generation, do not modify!!!
 */

using EdgeFramework.Sheet;
using EdgeFramework;
using System.Collections.Generic;

public partial class SheetManager : Singleton<SheetManager>
{
	//Example
	private Dictionary<int,Example> mExampleDict;
	public Example GetExample(int key)
	{
		if (mExampleDict == null)
		{
			InitExample();
		}
		return mExampleDict[key];
	}
	private void InitExample()
	{
		var items = GetSheetInfo<ExampleList>("Example").Items;
		mExampleDict = new Dictionary<int, Example>();
		items.ForEach(item => mExampleDict[item.exampleInt] = item);
	}
	
	//Preload
	private List<Preload> mPreloadList;
	public List<Preload> GetPreloadList()
	{
		if (mPreloadList == null)
		{
			InitPreload();
		}
		return mPreloadList;
	}
	private void InitPreload()
	{
		var items = GetSheetInfo<PreloadList>("Preload").Items;
		mPreloadList = items;
	}
	
	//Scene
	private Dictionary<int,Scene> mSceneDict;
	public Scene GetScene(int key)
	{
		if (mSceneDict == null)
		{
			InitScene();
		}
		return mSceneDict[key];
	}
	private void InitScene()
	{
		var items = GetSheetInfo<SceneList>("Scene").Items;
		mSceneDict = new Dictionary<int, Scene>();
		items.ForEach(item => mSceneDict[item.Id] = item);
	}
	
	//UIPanel
	private Dictionary<UIPanelTypeEnum,UIPanel> mUIPanelDict;
	public UIPanel GetUIPanel(UIPanelTypeEnum key)
	{
		if (mUIPanelDict == null)
		{
			InitUIPanel();
		}
		return mUIPanelDict[key];
	}
	private void InitUIPanel()
	{
		var items = GetSheetInfo<UIPanelList>("UIPanel").Items;
		mUIPanelDict = new Dictionary<UIPanelTypeEnum, UIPanel>();
		items.ForEach(item => mUIPanelDict[item.PanelType] = item);
	}
	
}