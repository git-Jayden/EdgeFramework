/**
 * Tool generation, do not modify!!!
 */

using EdgeFramework.Sheet;
using EdgeFramework;
using System.Collections.Generic;

public partial class SheetManager : Singleton<SheetManager>
{
	SheetManager(){}
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
	public void InitExample()
	{
		var items = GetSheetInfo<ExampleList>("Example").Items;
		mExampleDict = new Dictionary<int, Example>();
		items.ForEach(item => mExampleDict[item.exampleInt] = item);
	}
	
	//MusicSheet
	private Dictionary<MusicEnum,MusicSheet> mMusicSheetDict;
	public MusicSheet GetMusicSheet(MusicEnum key)
	{
		if (mMusicSheetDict == null)
		{
			InitMusicSheet();
		}
		return mMusicSheetDict[key];
	}
	public void InitMusicSheet()
	{
		var items = GetSheetInfo<MusicSheetList>("MusicSheet").Items;
		mMusicSheetDict = new Dictionary<MusicEnum, MusicSheet>();
		items.ForEach(item => mMusicSheetDict[item.MusicType] = item);
	}
	
	//PreloadSheet
	private List<PreloadSheet> mPreloadSheetList;
	public List<PreloadSheet> GetPreloadSheetList()
	{
		if (mPreloadSheetList == null)
		{
			InitPreloadSheet();
		}
		return mPreloadSheetList;
	}
	public void InitPreloadSheet()
	{
		var items = GetSheetInfo<PreloadSheetList>("PreloadSheet").Items;
		mPreloadSheetList = items;
	}
	
	//SceneSheet
	private Dictionary<string,SceneSheet> mSceneSheetDict;
	public SceneSheet GetSceneSheet(string key)
	{
		if (mSceneSheetDict == null)
		{
			InitSceneSheet();
		}
		return mSceneSheetDict[key];
	}
	public void InitSceneSheet()
	{
		var items = GetSheetInfo<SceneSheetList>("SceneSheet").Items;
		mSceneSheetDict = new Dictionary<string, SceneSheet>();
		items.ForEach(item => mSceneSheetDict[item.Name] = item);
	}
	
	//SoundSheet
	private Dictionary<SoundEnum,SoundSheet> mSoundSheetDict;
	public SoundSheet GetSoundSheet(SoundEnum key)
	{
		if (mSoundSheetDict == null)
		{
			InitSoundSheet();
		}
		return mSoundSheetDict[key];
	}
	public void InitSoundSheet()
	{
		var items = GetSheetInfo<SoundSheetList>("SoundSheet").Items;
		mSoundSheetDict = new Dictionary<SoundEnum, SoundSheet>();
		items.ForEach(item => mSoundSheetDict[item.SoundType] = item);
	}
	
	//UIPanelSheet
	private Dictionary<UIPanelTypeEnum,UIPanelSheet> mUIPanelSheetDict;
	public UIPanelSheet GetUIPanelSheet(UIPanelTypeEnum key)
	{
		if (mUIPanelSheetDict == null)
		{
			InitUIPanelSheet();
		}
		return mUIPanelSheetDict[key];
	}
	public void InitUIPanelSheet()
	{
		var items = GetSheetInfo<UIPanelSheetList>("UIPanelSheet").Items;
		mUIPanelSheetDict = new Dictionary<UIPanelTypeEnum, UIPanelSheet>();
		items.ForEach(item => mUIPanelSheetDict[item.PanelType] = item);
	}
	
}