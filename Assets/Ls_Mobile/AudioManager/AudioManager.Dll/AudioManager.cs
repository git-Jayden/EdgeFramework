using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;


namespace Ls_Mobile.AudioManager
{
    
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [Header("背景音乐的属性")]
        [SerializeField]
        [Tooltip("背景音乐是否开启")]
        private bool _musicOn = true;

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("背景音乐音量 0-1")]
        private float _musicVolume = 1f;

        [SerializeField]
        [Tooltip("在初始化时是否使用当前设置背景音乐音量,默认为false,使用磁盘中上次保存的音量")]
        private bool _useMusicVolOnStart;

          [SerializeField]
        [Tooltip("背景音乐的目标群体。如果不使用任何内容，则保留未分配或空白")]
        private AudioMixerGroup _musicMixerGroup;

        [SerializeField]
        [Tooltip("音乐混音器组的参数名称")]
        private string _volumeOfMusicMixer = string.Empty;



        [Header("音效属性")]
        [SerializeField]
        [Space(3f)]
        [Tooltip("音效是否开启")]
        private bool _soundFxOn = true;

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("音效音量")]
        private float _soundFxVolume = 1f;

        [SerializeField]
        [Tooltip("在初始化时是否使用当前设置音效音量,默认为false,使用磁盘中上次保存的音量")]
        private bool _useSfxVolOnStart;

        [SerializeField]
        [Tooltip("声音效果的目标群体。如果不使用任何内容，则保留未分配或空白")]
        private AudioMixerGroup _soundFxMixerGroup;

        [SerializeField]
        [Tooltip("音效混音器组的参数名称")]
        private string _volumeOfSFXMixer = string.Empty;

        [SerializeField]
        [Space(3f)]
        [Tooltip("AudioManager的所有Audio Clip的列表")]
        private List<AudioClip> _playlist = new List<AudioClip>();


        private static AudioManager instance;

        private static object key;

        private static bool alive;

        private List<SoundEffect> sfxPool = new List<SoundEffect>();

        private static BackgroundMusic backgroundMusic;

        private static AudioSource musicSource;

        private static AudioSource crossfadeSource;

        private static float currentMusicVol;

        private static float currentSfxVol;

        private static float musicVolCap;

        private static float savedPitch;

        private static bool musicOn;

        private static bool sfxOn;
        //过渡时间
        private static float transitionTime;

        private readonly static string BgMusicVolKey;

        private readonly static string SoundFxVolKey;

        private readonly static string BgMusicMuteKey;

        private readonly static string SoundFxMuteKey;


        /// <summary>
        /// 获取当前背景音乐Clip。
        /// </summary>
        /// <value>当前的AudioClip。</value>
        public AudioClip CurrentMusicClip
        {
            get
            {
                return AudioManager.backgroundMusic.CurrentClip;
            }
        }
        /// <summary>
        /// 当前且仅运行AudioManager实例
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (!AudioManager.alive)
                {
                    UnityEngine.Debug.LogWarning(string.Concat(typeof(AudioManager), "' is already destroyed on application quit."));
                    return null;
                }
                if (AudioManager.instance == null)
                {
                    AudioManager.instance = UnityEngine.Object.FindObjectOfType<AudioManager>();
                    if (AudioManager.instance == null)
                    {
                        lock (AudioManager.key)
                        {
                            AudioManager.instance = (new GameObject()).AddComponent<AudioManager>();
                        }
                    }
                }
                return AudioManager.instance;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示此实例的背景音乐与音效是否全部静音。
        /// </summary>
        /// <value><c>true</c> if this instance is master mute; otherwise, <c>false</c>.</value>
        public bool IsMasterMute
        {
            get
            {
                if (this._musicOn)
                {
                    return false;
                }
                return !this._soundFxOn;
            }
            set
            {
                this.ToggleMute(value);
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示背景音乐是否打开。
        /// </summary>
        /// <value><c>true</c> if this instance is music on; otherwise, <c>false</c>.</value>
        public bool IsMusicOn
        {
            get
            {
                return this._musicOn;
            }
            set
            {
                this.ToggleBGMMute(value);
            }
        }
        /// <summary>
        /// 是否正在播放背景音乐
        /// </summary>
        public bool IsMusicPlaying
        {
            get
            {
                if (AudioManager.musicSource == null)
                {
                    return false;
                }
                return AudioManager.musicSource.isPlaying;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示音效是否打开。
        /// </summary>
        /// <value><c>true</c> if this instance is sound on; otherwise, <c>false</c>.</value>
        public bool IsSoundOn
        {
            get
            {
                return this._soundFxOn;
            }
            set
            {
                this.ToggleSFXMute(value);
            }
        }

        /// <summary>
        /// 获取或设置背景音乐音量。
        /// </summary>
        /// <value>The music volume.</value>
        public float MusicVolume
        {
            get
            {
                return this._musicVolume;
            }
            set
            {
                this.SetBGMVolume(value);
            }
        }

        /// <summary>
        /// 获取AudioManager的所有AudioClip列表
        /// </summary>
        public List<AudioClip> Playlist
        {
            get
            {
                return this._playlist;
            }
        }

        /// <summary>
        ///  获取所有正在播放的音效列表sfxPool  
        /// </summary>
        public List<SoundEffect> SoundFxPool
        {
            get
            {
                return this.sfxPool;
            }
        }

        /// <summary>
        /// 获取或设置所有音效的音量。
        /// </summary>
        /// <value>The sound volume.</value>
        public float SoundVolume
        {
            get
            {
                return this._soundFxVolume;
            }
            set
            {
                this.SetSFXVolume(value);
            }
        }

        static AudioManager()
        {
            AudioManager.key = new object();
            AudioManager.alive = true;
            AudioManager.musicSource = null;
            AudioManager.crossfadeSource = null;
            AudioManager.currentMusicVol = 0f;
            AudioManager.currentSfxVol = 0f;
            AudioManager.musicVolCap = 0f;
            AudioManager.savedPitch = 1f;
            AudioManager.musicOn = false;
            AudioManager.sfxOn = false;
            AudioManager.BgMusicVolKey = "BGMVol";
            AudioManager.SoundFxVolKey = "SFXVol";
            AudioManager.BgMusicMuteKey = "BGMMute";
            AudioManager.SoundFxMuteKey = "SFXMute";
        }

        /// <summary>
        /// Prevent calling the consructor of the AudioManager
        /// </summary>
        private AudioManager()
        {
        }

        /// <summary>
        /// 将AudioClip添加到playlist列表中
        /// </summary>
        /// <param name="clip">AudioClip</param>
        public void AddToPlaylist(AudioClip clip)
        {
            if (clip != null)
            {
                this._playlist.Add(clip);
            }
        }

        /// <summary>
        /// 继承Monobehavior函数。
        /// </summary>
        private void Awake()
        {
            if (AudioManager.instance == null)
            {
                AudioManager.instance = this;
                this.Initialise();
                return;
            }
            if (AudioManager.instance != this)
            {
               UnityEngine.Object.Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// 从磁盘中删除与声音选项相关的所有PlayerPrefs
        /// </summary>
        public void ClearAllPreferences()
        {
            PlayerPrefs.DeleteKey(AudioManager.BgMusicVolKey);
            PlayerPrefs.DeleteKey(AudioManager.SoundFxVolKey);
            PlayerPrefs.DeleteKey(AudioManager.BgMusicMuteKey);
            PlayerPrefs.DeleteKey(AudioManager.SoundFxMuteKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 根据一些内部属性创建带有2D音乐设置的背景音乐AudioSource
        /// </summary>
        /// <returns>具有2D功能的AudioSource</returns>
        private AudioSource ConfigureAudioSource(AudioSource audioSource)
        {
            audioSource.outputAudioMixerGroup = this._musicMixerGroup;
            audioSource.playOnAwake=false;
            audioSource.spatialBlend=0f;
            audioSource.rolloffMode= AudioRolloffMode.Linear;
            audioSource.loop=true;
            audioSource.volume=this.LoadBGMVolume();
            audioSource.mute=!this._musicOn;
            return audioSource;
        }

        /// <summary>
        /// 内部函数用于创建需要播放所有产生音效的GameObject。
        /// 初始化音效的一些特定属性。
        /// </summary>
        /// <param name="audio_clip">播放的AudioClip</param>
        /// <param name="location">音频剪辑的世界位置。</param>
        /// <returns>新创建的游戏对象与AudioClip和AudioSource</returns>
        private GameObject CreateSoundFx(AudioClip audio_clip, Vector2 location)
        {
            GameObject gameObject = new GameObject(audio_clip.name);
            gameObject.transform.position=location;
            gameObject.transform.SetParent(base.transform);
            gameObject.AddComponent<SoundEffect>();
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake=false;
            audioSource.spatialBlend=0f;
            audioSource.rolloffMode=0;
            audioSource.outputAudioMixerGroup=this._soundFxMixerGroup;
            audioSource.clip=audio_clip;
            audioSource.mute=!this._soundFxOn;
            return gameObject;
        }

        /// <summary>
        /// 在当前音乐上进行重叠播放，以产生从一种音乐到另一种音乐的平稳过渡。
        /// 随着当前音乐的减少，下一首音乐的增多，最终会相互重叠，使之黯然失色
        /// 简而言之，它隐藏了在淡入和淡入过程中可能出现的任何无声的间隙
        /// 也称为无间隙交叉混合播放。 
        /// </summary>
        private void CrossFadeBackgroundMusic()
        {
            if (AudioManager.backgroundMusic.MusicTransition == MusicTransition.CrossFade && AudioManager.musicSource.clip.name != AudioManager.backgroundMusic.NextClip.name)
            {
                AudioManager.transitionTime -= Time.deltaTime;
                AudioManager.musicSource.volume=Mathf.Lerp(0f, AudioManager.musicVolCap, AudioManager.transitionTime / AudioManager.backgroundMusic.TransitionDuration);
                AudioManager.crossfadeSource.volume=Mathf.Clamp01(AudioManager.musicVolCap - AudioManager.musicSource.volume);
                AudioManager.crossfadeSource.mute=AudioManager.musicSource.mute;
                if (AudioManager.musicSource.volume <= 0f)
                {
                    this.SetBGMVolume(AudioManager.musicVolCap);
                    this.PlayBackgroundMusic(AudioManager.backgroundMusic.NextClip, AudioManager.crossfadeSource.time, AudioManager.crossfadeSource.pitch);
                }
            }
        }

        /// <summary>
        /// 清除音频剪辑列表_playlist
        /// </summary>
        public void EmptyPlaylist()
        {
            this._playlist.Clear();
        }

        /// <summary>
        /// 逐渐增加或减少背景音乐的音量
        /// 淡出是通过逐渐减小当前音乐的音量来实现的，这样它就从原来的音量变成了绝对的静音
        /// 淡入是通过逐渐增加下一首音乐的音量来实现的，这样它就从绝对的寂静变成了原来的音量
        /// 背景音乐淡入淡出
        /// </summary>
        private void FadeOutFadeInBackgroundMusic()
        {
            if (AudioManager.backgroundMusic.MusicTransition == MusicTransition.LinearFade)
            {
                if (AudioManager.musicSource.clip.name != AudioManager.backgroundMusic.NextClip.name)
                {
                    AudioManager.transitionTime -= Time.deltaTime;
                    AudioManager.musicSource.volume=Mathf.Lerp(0f, AudioManager.musicVolCap, AudioManager.transitionTime / AudioManager.backgroundMusic.TransitionDuration);
                    if (AudioManager.musicSource.volume<= 0f)
                    {
                        float single = 0f;
                        AudioManager.transitionTime = single;
                        AudioManager.musicSource.volume=single;
                        this.PlayMusicFromSource(ref AudioManager.musicSource, AudioManager.backgroundMusic.NextClip, 0f, AudioManager.musicSource.pitch);
                    }
                }
                else
                {
                    AudioManager.transitionTime += Time.deltaTime;
                    AudioManager.musicSource.volume=Mathf.Lerp(0f, AudioManager.musicVolCap, AudioManager.transitionTime / AudioManager.backgroundMusic.TransitionDuration);
                    if (AudioManager.musicSource.volume >= AudioManager.musicVolCap)
                    {
                        this.SetBGMVolume(AudioManager.musicVolCap);
                        this.PlayBackgroundMusic(AudioManager.backgroundMusic.NextClip, AudioManager.musicSource.time, AudioManager.savedPitch);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 提供名称，从playlist中获取AudioClip引用
        /// </summary>
        /// <param name="clip_name">playlist列表池中AudioClip的名称 </param>
        /// <returns>如果找不到匹配的名称，则为空</returns>
        public AudioClip GetClipFromPlaylist(string clip_name)
        {
            for (int i = 0; i < this._playlist.Count; i++)
            {
                if (clip_name == this._playlist[i].name)
                {
                    return this._playlist[i];
                }
            }
            UnityEngine.Debug.LogWarning(string.Concat(clip_name, " does not exist in the playlist."));
            return null;
        }

        /// <summary>
        /// 返回sfxPool池中音效的索引(如果存在)。
        /// </summary>
        /// <param name="name">音效的名称。</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <returns>返回-1代表不存在</returns>
        public int IndexOfSoundFxPool(string name, bool singleton = false)
        {
            for (int i = 0; i < this.sfxPool.Count; i++)
            {
                if (this.sfxPool[i].Name == name && singleton == this.sfxPool[i].Singleton)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// AudioManager初始化
        /// </summary>
        private void Initialise()
        {
            base.gameObject.name="AudioManager";
            this._musicOn = this.LoadBGMMuteStatus();
            this._musicVolume = (this._useMusicVolOnStart ? this._musicVolume : this.LoadBGMVolume());
            this._soundFxOn = this.LoadSFXMuteStatus();
            this._soundFxVolume = (this._useSfxVolOnStart ? this._soundFxVolume : this.LoadSFXVolume());
            AudioManager.musicSource= base.gameObject.GetComponent<AudioSource>()?? base.gameObject.AddComponent<AudioSource>();
            AudioManager.musicSource = this.ConfigureAudioSource(AudioManager.musicSource);
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }

        /// <summary>
        /// 如果更改了音乐音量或音乐静音状态，则返回true
        /// </summary>
        private bool IsMusicAltered()
        {
            float single = 0f;
            bool flag = (AudioManager.musicOn != this._musicOn || AudioManager.musicOn != !AudioManager.musicSource.mute ? true : AudioManager.currentMusicVol != this._musicVolume);
            if (!(this._musicMixerGroup != null) || string.IsNullOrEmpty(this._volumeOfMusicMixer.Trim()))
            {
                return flag;
            }
            this._musicMixerGroup.audioMixer.GetFloat(this._volumeOfMusicMixer, out single);
            single = this.NormaliseVolume(single);
            if (flag)
            {
                return true;
            }
            return AudioManager.currentMusicVol != single;
        }

        /// <summary>
        /// 如果更改了音效音量或音效静音状态，则返回tru
        /// </summary>
        private bool IsSoundFxAltered()
        {
            float single = 0f;
            bool flag = (this._soundFxOn != AudioManager.sfxOn ? true : AudioManager.currentSfxVol != this._soundFxVolume);
            if (!(this._soundFxMixerGroup != null) || string.IsNullOrEmpty(this._volumeOfSFXMixer.Trim()))
            {
                return flag;
            }
            this._soundFxMixerGroup.audioMixer.GetFloat(this._volumeOfSFXMixer, out single);
            single = this.NormaliseVolume(single);
            if (flag)
            {
                return true;
            }
            return AudioManager.currentSfxVol != single;
        }

        /// <summary>
        /// 从URL加载音频剪辑。
        /// </summary>
        /// <returns>来自URL的AudioClip。</returns>
        /// <param name="audio_url">Audio URL.</param>
        /// <param name="audio_type">Audio type.</param>
        /// <param name="callback">Callback.</param>
        private IEnumerator LoadAudioClipFromUrl(string audio_url, AudioType audio_type, Action<AudioClip> callback)
        {
            using (UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip(audio_url, audio_type))
            {
                yield return audioClip.SendWebRequest();
                if (audioClip.isNetworkError)
                {
                    UnityEngine.Debug.Log(string.Format("Error downloading audio clip at {0} : ", audio_url, audioClip.error));
                }
                callback(DownloadHandlerAudioClip.GetContent(audioClip));
            }
        }

        /// <summary>
        /// 磁盘中保存的背景音乐是否为开启状态，返回true开启状态,false为静音
        /// </summary>
        /// <returns>如果背景音乐静音的PlayerPrefs Key存在，则从保存的首选项中返回该静音键的值;如果不存在，则返回defaut值 </returns>
        private bool LoadBGMMuteStatus()
        {
            if (!PlayerPrefs.HasKey(AudioManager.BgMusicMuteKey))
            {
                return this._musicOn;
            }
            return this.ToBool(PlayerPrefs.GetInt(AudioManager.BgMusicMuteKey));
        }

        /// <summary>
        /// 从磁盘中获取背景音乐的音量
        /// </summary>
        /// <returns></returns>
        private float LoadBGMVolume()
        {
            if (!PlayerPrefs.HasKey(AudioManager.BgMusicVolKey))
            {
                return this._musicVolume;
            }
            return PlayerPrefs.GetFloat(AudioManager.BgMusicVolKey);
        }

        /// <summary>
        /// 从Resources文件夹中加载AudioClip
        /// </summary>
        /// <param name="path">Resources文件夹中的AudioClip的路径名</param>
        /// <param name="add_to_playlist">是否将加载的AudioClip添加到播放列表中</param>
        /// <returns>资源文件夹中的Audioclip</returns>
        public AudioClip LoadClip(string path, bool add_to_playlist = false)
        {
            AudioClip audioClip = Resources.Load(path) as AudioClip;
            if (audioClip != null)
            {
                if (add_to_playlist)
                {
                    this.AddToPlaylist(audioClip);
                }
                return audioClip;
            }
            UnityEngine.Debug.LogError(string.Format("AudioClip '{0}' not found at location {1}", path, Path.Combine(Application.dataPath, string.Concat("/Resources/", path))));
            return null;
        }

        /// <summary>
        /// 从指定的url路径加载AudioClip。
        /// </summary>
        /// <param name="path">要下载的音频剪辑的url路径。例如:“http://www.my-server.com/audio.ogg”</param>
        /// <param name="audio_type">下载AudioClip的音频类型</param>
        /// <param name="add_to_playlist">是否将加载的AudioClip添加到播放列表中</param>
        /// <param name="callback">加载完成后的回调</param>
        public void LoadClip(string path, AudioType audio_type, bool add_to_playlist, Action<AudioClip> callback)
        {
            base.StartCoroutine(this.LoadAudioClipFromUrl(path, audio_type, (AudioClip downloadedContent) => {
                if (downloadedContent != null && add_to_playlist)
                {
                    this.AddToPlaylist(downloadedContent);
                }
                callback(downloadedContent);
            }));
        }

        /// <summary>
        /// 将Resources文件夹路径中的所有声音剪辑加载到资产列表池中
        /// </summary>
        /// <param name="path">目标文件夹的路径名。当使用空字符串时，函数将加载Resources文件夹中的所有音频剪辑内容</param>
        /// <param name="overwrite">是否清空原播放列表的内容.</param>
        public void LoadPlaylist(string path, bool overwrite)
        {
            AudioClip[] audioClipArray = Resources.LoadAll<AudioClip>(path);
            if (audioClipArray != null && (int)audioClipArray.Length > 0 && overwrite)
            {
                this._playlist.Clear();
            }
            for (int i = 0; i < (int)audioClipArray.Length; i++)
            {
                this._playlist.Add(audioClipArray[i]);
            }
        }

        /// <summary>
        ///  磁盘中保存的音效是否为开启状态，返回true开启状态,false为静音
        /// </summary>
        /// <returns>如果存在音效静音键，则从保存的首选项中返回该静音键的值;如果不存在，则返回defaut值</returns>
        private bool LoadSFXMuteStatus()
        {
            if (!PlayerPrefs.HasKey(AudioManager.SoundFxMuteKey))
            {
                return this._soundFxOn;
            }
            return this.ToBool(PlayerPrefs.GetInt(AudioManager.SoundFxMuteKey));
        }

        /// <summary>
        /// 从磁盘中获取音效的音量
        /// </summary>
        /// <returns></returns>
        private float LoadSFXVolume()
        {
            if (!PlayerPrefs.HasKey(AudioManager.SoundFxVolKey))
            {
                return this._soundFxVolume;
            }
            return PlayerPrefs.GetFloat(AudioManager.SoundFxVolKey);
        }

        /// <summary>
        /// 管理音效池中的每个音效
        /// 在OnUpdate期间调用
        /// </summary>
        private void ManageSoundEffects()
        {
            for (int i = this.sfxPool.Count - 1; i >= 0; i--)
            {
                SoundEffect item = this.sfxPool[i];
                if (item.Source.isPlaying && item.RemainTime != float.PositiveInfinity)
                {
                    item.RemainTime = item.RemainTime - Time.deltaTime;
                }
                if (item.RemainTime <= 0.01f)
                {
                    item.Source.Stop();
                    item.Callback?.Invoke();
                    UnityEngine.Object.Destroy(item.gameObject);
                    this.sfxPool.RemoveAt(i);
                    this.sfxPool.Sort((SoundEffect x, SoundEffect y) => x.name.CompareTo(y.name));
                    return;
                }
            }
        }

        /// <summary>
        /// 将音量标准化，使其可以在[0 - 1]范围内，以适应音乐源音量和AudioManager音量
        /// </summary>
        /// <returns>在0和1之间的正常音量.</returns>
        /// <param name="vol">Vol.</param>
        private float NormaliseVolume(float vol)
        {
            vol += 80f;
            vol /= 100f;
            return vol;
        }

        /// <summary>
        /// 当应用程序退出时，它会以随机顺序销毁对象。
        /// 原则上，当您的应用程序退出或退出时，您不应该调用AudioManager。
        /// 如果任何脚本在实例被销毁后调用它，它将创建一个有bug的ghost对象，该对象将停留在编辑器场景中
        /// 这样做是为了确保我们没有创建那个bug ghost对象。
        /// </summary>
        private void OnApplicationExit()
        {
            AudioManager.alive = false;
        }

        /// <summary>
        ///继承Monobehavior函数。 
        /// </summary>
        private void OnDestroy()
        {
            base.StopAllCoroutines();
            this.SaveAllPreferences();
        }

        /// <summary>
        /// 更新函数调用每一帧
        /// </summary>
        private IEnumerator OnUpdate()
        {
            float single = 0f;
            float single1 = 0f;
            while (AudioManager.alive)
            {
                this.ManageSoundEffects();
                if (this.IsMusicAltered())
                {
                    this.ToggleBGMMute(!AudioManager.musicOn);
                    if (AudioManager.currentMusicVol != this._musicVolume)
                    {
                        AudioManager.currentMusicVol = this._musicVolume;
                    }
                    if (this._musicMixerGroup != null && !string.IsNullOrEmpty(this._volumeOfMusicMixer))
                    {
                        this._musicMixerGroup.audioMixer.GetFloat(this._volumeOfMusicMixer, out single);
                        single = this.NormaliseVolume(single);
                        AudioManager.currentMusicVol = single;
                    }
                    this.SetBGMVolume(AudioManager.currentMusicVol);
                }
                if (this.IsSoundFxAltered())
                {
                    this.ToggleSFXMute(!AudioManager.sfxOn);
                    if (AudioManager.currentSfxVol != this._soundFxVolume)
                    {
                        AudioManager.currentSfxVol = this._soundFxVolume;
                    }
                    if (this._soundFxMixerGroup != null && !string.IsNullOrEmpty(this._volumeOfSFXMixer))
                    {
                        this._soundFxMixerGroup.audioMixer.GetFloat(this._volumeOfSFXMixer, out single1);
                        single1 = this.NormaliseVolume(single1);
                        AudioManager.currentSfxVol = single1;
                    }
                    this.SetSFXVolume(AudioManager.currentSfxVol);
                }
                if (AudioManager.crossfadeSource != null)
                {
                    this.CrossFadeBackgroundMusic();
                    yield return null;
                }
                else if (AudioManager.backgroundMusic.NextClip != null)
                {
                    this.FadeOutFadeInBackgroundMusic();
                    yield return null;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// 暂停游戏中的所有音效
        /// </summary>
        public void PauseAllSFX()
        {
            SoundEffect[] soundEffectArray = UnityEngine.Object.FindObjectsOfType<SoundEffect>();
            for (int i = 0; i < (int)soundEffectArray.Length; i++)
            {
                SoundEffect soundEffect = soundEffectArray[i];
                if (soundEffect.Source.isPlaying)
                {
                    soundEffect.Source.Pause();
                }
            }
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        public void PauseBGM()
        {
            if (AudioManager.musicSource.isPlaying)
            {
                AudioManager.musicSource.Pause();
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="playback_position"> AudioClip的播放位置(秒)</param>
        /// <param name="pitch">AudioClip的音调 Mathf.Clamp(pitch, -3f, 3f);</param>
        private void PlayBackgroundMusic(AudioClip clip, float playback_position, float pitch)
        {
            this.PlayMusicFromSource(ref AudioManager.musicSource, clip, playback_position, pitch);
            AudioManager.backgroundMusic.NextClip = null;
            AudioManager.backgroundMusic.CurrentClip = clip;
            //移除交叉淡入淡出的AudioSource
            if (AudioManager.crossfadeSource != null)
            {
                UnityEngine.Object.Destroy(AudioManager.crossfadeSource);
                AudioManager.crossfadeSource = null;
            }
        }

        /// <summary>
        /// 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip">需要播放的 AudioClip</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        /// <param name="transition_duration">转换所需的秒数</param>
        /// <param name="volume">音量</param>
        /// <param name="pitch">由于音频片段 (Audio Clip)减慢/加快而形成的音调变化量。值 1 为正常播放速度。 Mathf.Clamp(pitch, -3f, 3f)</param>
        /// <param name="playback_position">AudioClip的播放位置(秒)</param>
        public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration, float volume, float pitch, float playback_position = 0f)
        {
            if (clip == null || AudioManager.backgroundMusic.CurrentClip == clip)
            {
                return;
            }
            if (AudioManager.backgroundMusic.CurrentClip == null || transition_duration <= 0f)
            {
                transition = MusicTransition.Swift;
            }
            if (transition == MusicTransition.Swift)
            {
                this.PlayBackgroundMusic(clip, playback_position, pitch);
                this.SetBGMVolume(volume);
                return;
            }
            if (AudioManager.backgroundMusic.NextClip != null)
            {
                UnityEngine.Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
                return;
            }

            AudioManager.backgroundMusic.MusicTransition = transition;
            float transitionDuration = transition_duration;
            float single = transitionDuration;
            AudioManager.backgroundMusic.TransitionDuration = transitionDuration;
            AudioManager.transitionTime = single;
            AudioManager.musicVolCap = this._musicVolume;
            AudioManager.backgroundMusic.NextClip = clip;
            if (AudioManager.backgroundMusic.MusicTransition == MusicTransition.CrossFade)
            {
                if (AudioManager.crossfadeSource != null)
                {
                    UnityEngine.Debug.LogWarning("Trying to perform a transition on the background music while one is still active");
                    return;
                }
                AudioManager.crossfadeSource = this.ConfigureAudioSource(base.gameObject.AddComponent<AudioSource>());
                AudioManager.crossfadeSource.volume=Mathf.Clamp01(AudioManager.musicVolCap - AudioManager.currentMusicVol);
                AudioManager.crossfadeSource.priority=0;
                this.PlayMusicFromSource(ref AudioManager.crossfadeSource, AudioManager.backgroundMusic.NextClip, 0f, pitch);
            }
        }

        /// <summary>
        /// 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip">需要播放的 AudioClip</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        /// <param name="transition_duration">转换所需的秒数</param>
        /// <param name="volume">音量</param>
        public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration, float volume)
        {
            this.PlayBGM(clip, transition, transition_duration, volume, 1f, 0f);
        }

        /// <summary>
        /// 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip">需要播放的 AudioClip</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        /// <param name="transition_duration">转换所需的秒数</param>
        public void PlayBGM(AudioClip clip, MusicTransition transition, float transition_duration)
        {
            this.PlayBGM(clip, transition, transition_duration, this._musicVolume, 1f, 0f);
        }
        /// <summary>
        /// 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip">需要播放的 AudioClip</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        public void PlayBGM(AudioClip clip, MusicTransition transition)
        {
            this.PlayBGM(clip, transition, 1f, this._musicVolume, 1f, 0f);
        }

        /// <summary>
        /// 播放背景音乐 使用swift转换模式。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip">需要播放的 AudioClip</param>
        public void PlayBGM(AudioClip clip)
        {
            this.PlayBGM(clip, MusicTransition.Swift, 1f, this._musicVolume, 1f, 0f);
        }
        /// <summary>
        /// 根据Resource文件夹下路径 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip_path">Resource文件夹中目标剪辑的路径名</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        /// <param name="transition_duration">转换所需的秒数</param>
        /// <param name="volume">音量</param>
        /// <param name="pitch">由于音频片段 (Audio Clip)减慢/加快而形成的音调变化量。值 1 为正常播放速度。 Mathf.Clamp(pitch, -3f, 3f)</param>
        /// <param name="playback_position">AudioClip的播放位置(秒)</param>
        public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration, float volume, float pitch, float playback_position = 0f)
        {
            this.PlayBGM(this.LoadClip(clip_path, false), transition, transition_duration, volume, pitch, playback_position);
        }

        /// <summary>
        ///  播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip_path">Resource文件夹中目标剪辑的路径名</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        /// <param name="transition_duration">转换所需的秒数</param>
        /// <param name="volume">音量</param>
        public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration, float volume)
        {
            this.PlayBGM(this.LoadClip(clip_path, false), transition, transition_duration, volume, 1f, 0f);
        }
        /// <summary>
        /// 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip_path">Resource文件夹中目标剪辑的路径名</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        /// <param name="transition_duration">转换所需的秒数</param>
        public void PlayBGM(string clip_path, MusicTransition transition, float transition_duration)
        {
            this.PlayBGM(this.LoadClip(clip_path, false), transition, transition_duration, this._musicVolume, 1f, 0f);
        }

        /// <summary>
        /// 播放背景音乐。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip_path">Resource文件夹中目标剪辑的路径名</param>
        /// <param name="transition">音乐应该如何从现在转到下一个</param>
        public void PlayBGM(string clip_path, MusicTransition transition)
        {
            this.PlayBGM(this.LoadClip(clip_path, false), transition, 1f, this._musicVolume, 1f, 0f);
        }

        /// <summary>
        /// 播放背景音乐 使用swift转换模式。
        /// 一次只能播放一种背景音乐。
        /// </summary>
        /// <param name="clip_path">Resource文件夹中目标剪辑的路径名</param>
        public void PlayBGM(string clip_path)
        {
            this.PlayBGM(this.LoadClip(clip_path, false), MusicTransition.Swift, 1f, this._musicVolume, 1f, 0f);
        }

        /// <summary>
        /// 播放指定AudioClip。
        /// 如果AudioClip为空，则创建并分配音频源组件。
        /// </summary>
        /// <param name="audio_source">需要播放到哪个audio source中</param>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="playback_position">AudioClip 播放位置(秒)</param>
        /// <param name="pitch">音调的大小</param>
        private void PlayMusicFromSource(ref AudioSource audio_source, AudioClip clip, float playback_position, float pitch)
        {
            try
            {
                audio_source.clip=clip;
                audio_source.time=playback_position;
                float single = Mathf.Clamp(pitch, -3f, 3f);
                pitch = single;
                audio_source.pitch=single;
                audio_source.Play();
            }
            catch (NullReferenceException nullReferenceException)
            {
                UnityEngine.Debug.LogError(nullReferenceException.Message);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(exception.Message);
            }
        }

        /// <summary>
        /// 在世界空间的指定位置播放一次音效，并在声音结束后调用指定的回调函数
        /// </summary>
        /// <returns>An AudioSource</returns>
        /// <param name="clip">需要播放的AudioSource</param>
        /// <param name="location">播放位置，在世界空间中的位置</param>
        /// <param name="volume">播放的音量0-1</param>
        /// <param name="pitch">音调大小</param>
        /// <param name="callback">回调函数</param>
        public AudioSource PlayOneShot(AudioClip clip, Vector2 location, float volume, float pitch = 1f, Action callback = null)
        {
            if (clip == null)
            {
                return null;
            }


            GameObject gameObject = this.CreateSoundFx(clip, location);
            AudioSource component = gameObject.GetComponent<AudioSource>();
            component.loop = false;
            component.volume = this._soundFxVolume * volume;
            component.pitch = pitch;
            SoundEffect soundEffect = gameObject.GetComponent<SoundEffect>();
            soundEffect.Singleton = false;
            soundEffect.Source = component;
            soundEffect.OriginalVolume = volume;
            float _length = clip.length;
            float single = _length;
            soundEffect.RemainTime = _length;
            soundEffect.Duration = single;
            soundEffect.Callback = callback;
            this.sfxPool.Add(soundEffect);
            component.Play();
            return component;
        }

        /// <summary>
        /// 在世界空间中的某个位置播放一次音效
        /// </summary>
        /// <returns>An AudioSource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="location">播放位置，在世界空间中的位置</param>
        /// <param name="callback">回调函数</param>
        public AudioSource PlayOneShot(AudioClip clip, Vector2 location, Action callback = null)
        {
            return this.PlayOneShot(clip, location, this._soundFxVolume, 1f, callback);
        }

        /// <summary>
        /// 播放一次音效，并在声音结束后调用指定的回调函数
        /// </summary>
        /// <returns>An AudioSource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="callback">回调函数</param>
        public AudioSource PlayOneShot(AudioClip clip, Action callback = null)
        {
            return this.PlayOneShot(clip, Vector2.zero, this._soundFxVolume, 1f, callback);

        }

        /// <summary>
        /// 在世界空间中的给定位置播放音效，持续指定时间，并在时间结束后调用指定的回调函数。
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="location">播放位置，在世界空间中的位置</param>
        /// <param name="duration">播放音效所持续的时间(S)</param>
        /// <param name="volume">音量大小</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <param name="pitch">音调</param>
        /// <param name="callback">回调函数</param>
        public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
        {
            if (duration <= 0f || clip == null)
            {
                return null;
            }
            int num = this.IndexOfSoundFxPool(clip.name, true);
            if (num >= 0)
            {
                SoundEffect item = this.sfxPool[num];
                float single = duration;
                float single1 = single;
                item.RemainTime = single;
                item.Duration = single1;
                this.sfxPool[num] = item;
                return this.sfxPool[num].Source;
            }
            GameObject gameObject = null;
            AudioSource component = null;
            gameObject = this.CreateSoundFx(clip, location);
            component = gameObject.GetComponent<AudioSource>();
            component.loop=duration > clip.length;
            component.volume=this._soundFxVolume * volume;
            component.pitch=pitch;
            SoundEffect soundEffect = gameObject.GetComponent<SoundEffect>();
            soundEffect.Singleton = singleton;
            soundEffect.Source = component;
            soundEffect.OriginalVolume = volume;
            float single2 = duration;
            float single3 = single2;
            soundEffect.RemainTime = single2;
            soundEffect.Duration = single3;
            soundEffect.Callback = callback;
            this.sfxPool.Add(soundEffect);
            component.Play();
            return component;
        }

        /// <summary>
        /// 在世界空间中的给定位置播放音效持续一段时间，并在时间结束后调用指定的回调函数
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="location">播放位置，在世界空间中的位置</param>
        /// <param name="duration">播放音效所持续的时间(S)</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <param name="callback">回调函数</param>
        public AudioSource PlaySFX(AudioClip clip, Vector2 location, float duration, bool singleton = false, Action callback = null)
        {
            return this.PlaySFX(clip, location, duration, this._soundFxVolume, singleton, 1f, callback);
        }

        /// <summary>
        /// 播放的音效持续一段时间，并在时间结束后调用指定的回调函数
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="duration">持续播放的时长</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <param name="callback">回调函数</param>
        public AudioSource PlaySFX(AudioClip clip, float duration, bool singleton = false, Action callback = null)
        {
            return this.PlaySFX(clip, Vector2.zero, duration, this._soundFxVolume, singleton, 1f, callback);
        }

        /// <summary>
        /// 将声音片段添加到资产列表池
        /// </summary>
        /// <param name="clip">Sound clip data</param>
        public void RemoveFromPlaylist(AudioClip clip)
        {
            if (clip != null && this.GetClipFromPlaylist(clip.name))
            {
                this._playlist.Remove(clip);
                this._playlist.Sort((AudioClip x, AudioClip y) => x.name.CompareTo(y.name));
            }
        }

        /// <summary>
        /// 在世界空间中的给定位置重复播放指定次数的音效，并在声音结束后调用指定的回调函数。
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="location">播放位置，在世界空间中的位置</param>
        /// <param name="repeat">连续播放多少次。若要永久循环，请将其设置为负数</param>
        /// <param name="volume">音量</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <param name="pitch">音调</param>
        /// <param name="callback">回调函数</param>
        public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, float volume, bool singleton = false, float pitch = 1f, Action callback = null)
        {
            float single;
            float single1;
            if (clip == null)
            {
                return null;
            }
            if (repeat == 0)
            {
                return this.PlayOneShot(clip, location, volume, pitch, callback);
            }
            int num = this.IndexOfSoundFxPool(clip.name, true);
            if (num >= 0)
            {
                SoundEffect item = this.sfxPool[num];
                SoundEffect soundEffect = item;
                SoundEffect soundEffect1 = item;
                single1 = (repeat > 0 ? clip.length * (float)repeat : float.PositiveInfinity);
                float single2 = single1;
                soundEffect1.RemainTime = single1;
                soundEffect.Duration = single2;
                this.sfxPool[num] = item;
                return this.sfxPool[num].Source;
            }
            GameObject gameObject = this.CreateSoundFx(clip, location);
            AudioSource component = gameObject.GetComponent<AudioSource>();
            component.loop=repeat != 0;
            component.volume=this._soundFxVolume * volume;
            component.pitch=pitch;
            SoundEffect component1 = gameObject.GetComponent<SoundEffect>();
            component1.Singleton = singleton;
            component1.Source = component;
            component1.OriginalVolume = volume;
            SoundEffect soundEffect2 = component1;
            SoundEffect soundEffect3 = component1;
            single = (repeat > 0 ? clip.length * (float)repeat : float.PositiveInfinity);
            float single3 = single;
            soundEffect3.RemainTime = single;
            soundEffect2.Duration = single3;
            component1.Callback = callback;
            this.sfxPool.Add(component1);
            component.Play();
            return component;
        }

        /// <summary>
        /// 在世界空间中的给定位置重复音效达指定的次数，并在声音结束后调用指定的回调函数。
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="location">播放位置，在世界空间中的位置</param>
        /// <param name="repeat">连续播放多少次。若要永久循环，请将其设置为负数</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <param name="callback">音效播放完后的回调。</param>
        public AudioSource RepeatSFX(AudioClip clip, Vector2 location, int repeat, bool singleton = false, Action callback = null)
        {
            return this.RepeatSFX(clip, location, repeat, this._soundFxVolume, singleton, 1f, callback);
        }

        /// <summary>
        /// 重复播放指定次数的音效，并在音效结束后调用回调函数。
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="clip">需要播放的AudioClip</param>
        /// <param name="repeat">连续播放多少次。若要永久循环，请将其设置为负数</param>
        /// <param name="singleton">是否只允许一个音效的实例被激活。单例</param>
        /// <param name="callback">音效播放完后的回调。</param>
        public AudioSource RepeatSFX(AudioClip clip, int repeat, bool singleton = false, Action callback = null)
        {
            return this.RepeatSFX(clip, Vector2.zero, repeat, this._soundFxVolume, singleton, 1f, callback);
        }

        /// <summary>
        /// 恢复游戏中的所有音效
        /// </summary>
        public void ResumeAllSFX()
        {
            SoundEffect[] soundEffectArray = UnityEngine.Object.FindObjectsOfType<SoundEffect>();
            for (int i = 0; i < (int)soundEffectArray.Length; i++)
            {
                SoundEffect soundEffect = soundEffectArray[i];
                if (!soundEffect.Source.isPlaying)
                {
                    soundEffect.Source.UnPause();
                }
            }
        }

        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        public void ResumeBGM()
        {
            if (!AudioManager.musicSource.isPlaying)
            {
                AudioManager.musicSource.UnPause();
            }
        }

        /// <summary>
        /// 将所有已修改的声音选项或首选项写入磁盘
        /// </summary>
        public void SaveAllPreferences()
        {
            PlayerPrefs.SetFloat(AudioManager.SoundFxVolKey, this._soundFxVolume);
            PlayerPrefs.SetFloat(AudioManager.BgMusicVolKey, this._musicVolume);
            PlayerPrefs.SetInt(AudioManager.SoundFxMuteKey, (this._soundFxOn ? 1 : 0));
            PlayerPrefs.SetInt(AudioManager.BgMusicMuteKey, (this._musicOn ? 1 : 0));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 将背景音乐的音量和静音状态存储到磁盘。
        /// 请注意，当此脚本被销毁时，所有首选项都会自动保存
        /// </summary>
        public void SaveBGMPreferences()
        {
            PlayerPrefs.SetInt(AudioManager.BgMusicMuteKey, (this._musicOn ? 1 : 0));
            PlayerPrefs.SetFloat(AudioManager.BgMusicVolKey, this._musicVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        ///将音效的音量和静音状态存储到磁盘。
        /// 请注意，当此脚本被销毁时，所有首选项都会自动保存
        /// </summary>
        public void SaveSFXPreferences()
        {
            PlayerPrefs.SetInt(AudioManager.SoundFxMuteKey, (this._soundFxOn ? 1 : 0));
            PlayerPrefs.SetFloat(AudioManager.SoundFxVolKey, this._soundFxVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置背景音乐音量。
        /// </summary>
        /// <param name="volume">音量大小</param>
        private void SetBGMVolume(float volume)
        {
            try
            {
                volume = Mathf.Clamp01(volume);
                this._musicVolume = volume;
                AudioManager.currentMusicVol = volume;
                AudioManager.musicSource.volume= volume;
                if (this._musicMixerGroup != null && !string.IsNullOrEmpty(this._volumeOfMusicMixer.Trim()))
                {
                    float single = -80f + volume * 100f;
                    this._musicMixerGroup.audioMixer.SetFloat(this._volumeOfMusicMixer, single);
                }
            }
            catch (NullReferenceException nullReferenceException)
            {
                UnityEngine.Debug.LogError(nullReferenceException.Message);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(exception.Message);
            }
        }

        /// <summary>
        /// 设置音效音量。
        /// </summary>
        /// <param name="volume">New volume for all the sound effects.</param>
        private void SetSFXVolume(float volume)
        {
            try
            {
                volume = Mathf.Clamp01(volume);
                this._soundFxVolume = volume;
                AudioManager.currentSfxVol = volume;
                SoundEffect[] soundEffectArray = UnityEngine.Object.FindObjectsOfType<SoundEffect>();
                for (int i = 0; i < (int)soundEffectArray.Length; i++)
                {
                    SoundEffect soundEffect = soundEffectArray[i];
                    soundEffect.Source.volume=this._soundFxVolume * soundEffect.OriginalVolume;
                    soundEffect.Source.mute=!this._soundFxOn;
                }
                if (this._soundFxMixerGroup != null && !string.IsNullOrEmpty(this._volumeOfSFXMixer.Trim()))
                {
                    float single = -80f + volume * 100f;
                    this._soundFxMixerGroup.audioMixer.SetFloat(this._volumeOfSFXMixer, single);
                }
            }
            catch (NullReferenceException nullReferenceException)
            {
                UnityEngine.Debug.LogError(nullReferenceException.Message);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(exception.Message);
            }
        }

        /// <summary>
        /// 继承Monobehavior函数。
        /// </summary>
        private void Start()
        {
            if (AudioManager.musicSource != null)
            {
                base.StartCoroutine(this.OnUpdate());
            }
        }

        /// <summary>
        /// 停止游戏中的所有音效
        /// </summary>
        public void StopAllSFX()
        {
            SoundEffect[] soundEffectArray = UnityEngine.Object.FindObjectsOfType<SoundEffect>();
            for (int i = 0; i < (int)soundEffectArray.Length; i++)
            {
                SoundEffect soundEffect = soundEffectArray[i];
                if (soundEffect.Source)
                {
                    soundEffect.Source.Stop();
                    UnityEngine.Object.Destroy(soundEffect.gameObject);
                }
            }
            this.sfxPool.Clear();
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBGM()
        {
            if (AudioManager.musicSource.isPlaying)
            {
                AudioManager.musicSource.Stop();
            }
        }

        /// <summary>
        /// 将整数值转换为布尔表示值
        /// </summary>
        private bool ToBool(int integer)
        {
            if (integer != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 切换背景音乐静音。
        /// </summary>
        /// <param name="flag">true为静音.</param>
        private void ToggleBGMMute(bool flag)
        {
            this._musicOn = flag;
            AudioManager.musicOn = flag;
            AudioManager.musicSource.mute= !flag;
        }

        /// <summary>
        /// 切换控制背景音乐和音效静音
        /// </summary>
        /// <param name="flag">true为静音.</param>
        private void ToggleMute(bool flag)
        {
            this.ToggleBGMMute(flag);
            this.ToggleSFXMute(flag);
        }

        /// <summary>
        /// 切换音效静音。
        /// </summary>
        /// <param name="flag">true为静音</param>
        private void ToggleSFXMute(bool flag)
        {
            this._soundFxOn = flag;
            AudioManager.sfxOn = flag;
            SoundEffect[] soundEffectArray = UnityEngine.Object.FindObjectsOfType<SoundEffect>();
            for (int i = 0; i < (int)soundEffectArray.Length; i++)
            {
                soundEffectArray[i].Source.mute= !flag;
            }
        }
    }
}
