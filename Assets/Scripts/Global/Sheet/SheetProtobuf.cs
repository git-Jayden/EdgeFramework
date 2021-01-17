/**
 * Tool generation, do not modify!!!
 */

using ProtoBuf;
using System.IO;
using System.Collections.Generic;

namespace EdgeFramework.Sheet
{
    public class BaseList
    {
        public void Export(string outFile)
        {
            using (MemoryStream m = new MemoryStream())
            {
                Serializer.Serialize(m, this);
                m.Position = 0;
                int length = (int)m.Length;
                var buffer = new byte[length];
                m.Read(buffer, 0, length);
                File.WriteAllBytes(outFile, buffer);
            }
        }
    }
    
	[ProtoContract]
	public class Example
	{
		[ProtoMember(1)]
		public int exampleInt;
		[ProtoMember(2)]
		public string exampleString;
		[ProtoMember(3)]
		public float exampleFloat;
		[ProtoMember(4)]
		public bool exampleBool;
		[ProtoMember(5)]
		public List<int> exampleArray1 = new List<int>();
		[ProtoMember(6)]
		public List<float> exampleArray2 = new List<float>();
		[ProtoMember(7)]
		public List<string> exampleArray3 = new List<string>();
		[ProtoMember(8)]
		public ExampleEnum exampleEnum;
		[ProtoMember(9)]
		public string exampleClient;
	}

	[ProtoContract]
	public class ExampleList : BaseList
	{
		[ProtoMember(1)]
		public List<Example> Items = new List<Example>();
	}

	[ProtoContract]
	public class MusicSheet
	{
		[ProtoMember(1)]
		public int Id;
		[ProtoMember(2)]
		public MusicEnum MusicType;
		[ProtoMember(3)]
		public int MusTransition;
		[ProtoMember(4)]
		public int Volume;
		[ProtoMember(5)]
		public float Duration;
		[ProtoMember(6)]
		public string Path;
	}

	[ProtoContract]
	public class MusicSheetList : BaseList
	{
		[ProtoMember(1)]
		public List<MusicSheet> Items = new List<MusicSheet>();
	}

	[ProtoContract]
	public class PreloadSheet
	{
		[ProtoMember(1)]
		public int SceneId;
		[ProtoMember(2)]
		public string ResPath;
	}

	[ProtoContract]
	public class PreloadSheetList : BaseList
	{
		[ProtoMember(1)]
		public List<PreloadSheet> Items = new List<PreloadSheet>();
	}

	[ProtoContract]
	public class SceneSheet
	{
		[ProtoMember(1)]
		public int Id;
		[ProtoMember(2)]
		public string Name;
	}

	[ProtoContract]
	public class SceneSheetList : BaseList
	{
		[ProtoMember(1)]
		public List<SceneSheet> Items = new List<SceneSheet>();
	}

	[ProtoContract]
	public class SoundSheet
	{
		[ProtoMember(1)]
		public int Id;
		[ProtoMember(2)]
		public SoundEnum SoundType;
		[ProtoMember(3)]
		public int Repeat;
		[ProtoMember(4)]
		public bool Single;
		[ProtoMember(5)]
		public string Path;
	}

	[ProtoContract]
	public class SoundSheetList : BaseList
	{
		[ProtoMember(1)]
		public List<SoundSheet> Items = new List<SoundSheet>();
	}

	[ProtoContract]
	public class UIPanelSheet
	{
		[ProtoMember(1)]
		public int Id;
		[ProtoMember(2)]
		public UIPanelTypeEnum PanelType;
		[ProtoMember(3)]
		public string Path;
	}

	[ProtoContract]
	public class UIPanelSheetList : BaseList
	{
		[ProtoMember(1)]
		public List<UIPanelSheet> Items = new List<UIPanelSheet>();
	}

	public enum ExampleEnum
	{
		Type1,
		Type2,
		Max
	}

	public enum MusicEnum
	{
		Lobby,
		Game,
		Max
	}

	public enum SoundEnum
	{
		CloseClick,
		EnterClick,
		Max
	}

	public enum UIPanelTypeEnum
	{
		ItemMessagePanel,
		KnapsackPanel,
		LoadingPanel,
		MainMenuPanel,
		ShopPanel,
		SkillPanel,
		SystemPanel,
		TaskPanel,
		Max
	}

}
