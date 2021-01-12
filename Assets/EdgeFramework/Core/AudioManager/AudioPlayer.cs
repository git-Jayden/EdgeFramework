
/****************************************************
	文件：AudioPlayer.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:54   	
	Features：
*****************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using EdgeFramework.Audio;


namespace EdgeFramework
{
    
    public class DataSound
    {
        public enum ChannelEnum
        {
            BGM,
            SFX,
        }
        public enum TypeEnum
        {
            Loop,
            Single,
        }
        public ChannelEnum Channel;
        public string Name;
        //音效参数
        public TypeEnum Loop;
        public int repeat;//循环播放次数 负数代表一直循环
        public bool single;//是否只允许一个音效播放


        //背景音乐参数
        public MusicTransition MusTransition;//音乐过渡效果类型
        public float Volume;//初始音量
        public float Duration;//过渡所需的时间（以秒为单位）

        public DataSound(string[] sp)
        {
            Channel=(ChannelEnum)System.Enum.Parse(typeof(ChannelEnum), sp[1]);
            switch (Channel)
            {
                case ChannelEnum.BGM:
                    MusTransition = (MusicTransition)System.Enum.Parse(typeof(MusicTransition), sp[2]);
                    Volume = float.Parse(sp[3]);
                    Duration = float.Parse(sp[4]);
             
                    break;
                case ChannelEnum.SFX:
                    Loop = (TypeEnum)System.Enum.Parse(typeof(TypeEnum), sp[2]);
                    repeat = int.Parse(sp[3]);
                    single = bool.Parse(sp[4]);
                    break;
                default:
                    break;
            }
            Name = sp[5];
        }
    }
    public class AudioPlayer :Singleton<AudioPlayer>
    {
        AudioPlayer() { }
        private string AudioRootPath = "Assets/ABResources/Audio/";
        private Dictionary<string, DataSound> mDic = new Dictionary<string, DataSound>();
        /// <summary>
        /// 解析AudioConfig
        /// </summary>
        public void ParseAudioConfig()
        {
            //TextAsset text = ResourcesManager.Instance.LoadResouce<TextAsset>(ABAddress.AudioConfig);
            //StringReader reader = new StringReader(text.text);
            //////（1）Key值（2）类型BGM代表背景音乐SFX代表音效（3）播放类型Loop代表循环播放，Single代表单次播放（4）音乐音效在ABResources/Audio路径下的名字
            //string line = "";
            //while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            //{
            //    if (!line.StartsWith("<-公共->")&& !line.StartsWith("//"))//确定此字符串实例的开头是否与指定的字符串匹配。
            //    {
            //        string[] sp = line.Split(',');
            //        // Debug.Log("Read Sound:" + line);
            //        string key = sp[0];
            //        mDic.Add(key, new DataSound(sp));
            //    }
            //}
            //text = null;
            //ResourcesManager.Instance.ReleaseResouce(ABAddress.AudioConfig,true);
        }
        /// <summary>
        /// 根据key获取DataSound
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private DataSound GetDataSound(string key)
        {
            DataSound dataSound = null;
            if (mDic.TryGetValue(key,out dataSound))
                return dataSound;
            return null;
        }
        /// <summary>
        /// 根据key获取路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetPath(string key)
        {
            string name = "";
            DataSound dataSound = null;
            if (mDic.TryGetValue(key, out dataSound))
                name= dataSound.Name;
            return AudioRootPath+ name+".mp3";
        }
        /// <summary>
        /// 根据key获取播放类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private DataSound.TypeEnum GetPlayType(string key)
        {
            DataSound.TypeEnum playType = DataSound.TypeEnum.Single;
            DataSound dataSound = null;
            if (mDic.TryGetValue(key, out dataSound))
                playType = dataSound.Loop;
            return playType;
        }
     

        public void PlaySound(string key)
        {
            AudioClip clip = AudioManager.Instance.LoadClip(GetPath(key), true);
            DataSound dataSound = GetDataSound(key);
            switch (dataSound.Loop)
            {
                case DataSound.TypeEnum.Loop:
                    AudioManager.Instance.RepeatSFX(clip, dataSound.repeat, dataSound.single);
                    break;
                case DataSound.TypeEnum.Single:
                    AudioManager.Instance.PlayOneShot(clip);
                    break;
                default:
                    break;
            }
        }
        public void PlayBGM(string key)
        {
            DataSound dataSound = GetDataSound(key);
            AudioManager.Instance.PlayBGM(GetPath(key), dataSound.MusTransition,dataSound.Duration,dataSound.Volume);
        }
        public void StopAllSFX()
        {
            AudioManager.Instance.StopAllSFX();
        }
        public void PauseBGM()
        {
            AudioManager.Instance.PauseBGM();
        }
        public void ResumeBGM()
        {
            AudioManager.Instance.ResumeBGM();
        }
    }
}
