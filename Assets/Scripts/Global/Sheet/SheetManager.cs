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
	
	//Music
	private Dictionary<MusicEnum,Music> mMusicDict;
	public Music GetMusic(MusicEnum key)
	{
		if (mMusicDict == null)
		{
			InitMusic();
		}
		return mMusicDict[key];
	}
	public void InitMusic()
	{
		var items = GetSheetInfo<MusicList>("Music").Items;
		mMusicDict = new Dictionary<MusicEnum, Music>();
		items.ForEach(item => mMusicDict[item.MusicType] = item);
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
	public void InitPreload()
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
	public void InitScene()
	{
		var items = GetSheetInfo<SceneList>("Scene").Items;
		mSceneDict = new Dictionary<int, Scene>();
		items.ForEach(item => mSceneDict[item.Id] = item);
	}
	
	//Sound
	private Dictionary<SoundEnum,Sound> mSoundDict;
	public Sound GetSound(SoundEnum key)
	{
		if (mSoundDict == null)
		{
			InitSound();
		}
		return mSoundDict[key];
	}
	public void InitSound()
	{
		var items = GetSheetInfo<SoundList>("Sound").Items;
		mSoundDict = new Dictionary<SoundEnum, Sound>();
		items.ForEach(item => mSoundDict[item.SoundType] = item);
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
	public void InitUIPanel()
	{
		var items = GetSheetInfo<UIPanelList>("UIPanel").Items;
		mUIPanelDict = new Dictionary<UIPanelTypeEnum, UIPanel>();
		items.ForEach(item => mUIPanelDict[item.PanelType] = item);
	}
	
}