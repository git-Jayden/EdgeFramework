using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ls_Mobile;
using com.ls_mobile.tool;
using System.IO;
using System;

public class GameSound 
{
    private SoundManager sound;
    public class DataSound
    {
        public enum Channel
        {
            BGM,
            SFX,
        }
        public enum Type
        {
            Loop,
            Single,
        }
        public Channel channel;
        public string path;
        public Type loop;


        public DataSound(string[] sp)
        {
            // TODO: Complete member initialization
            channel = (Channel)System.Enum.Parse(typeof(Channel), sp[1]);
            loop = (Type)System.Enum.Parse(typeof(Type), sp[2]);
            path = sp[3];

        }

    }
    private Dictionary<string, DataSound> dic = new Dictionary<string, DataSound>();

    public GameSound(SoundManager sound)
    {
        //读取并解析Resources/Data/config_sound文本里面的所有内容
        this.sound = sound;
        TextAsset text = ResouceManager.Instance.LoadResouce<TextAsset>(Constants.AudioConfig);//Resources.Load<TextAsset>(ConstStr.AudioPath/*"Data/config_sound"*/);

        StringReader reader = new StringReader(text.text);
        string line = "";
        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            if (!line.StartsWith("<-公共->"))//确定此字符串实例的开头是否与指定的字符串匹配。
            {
                string[] sp = line.Split(',');
                // Debug.Log("Read Sound:" + line);
                string key = sp[0];
                dic.Add(key, new DataSound(sp));
            }
        }
        OnSounChange((int)ShareEvent.SoundChange, null);

        LEventSystem.RegisterEvent(ShareEvent.SoundChange, OnSounChange);//注册修改音效
        LEventSystem.RegisterEvent(ShareEvent.ClickSound, OnClickSound);//注册点击按钮播放音效

    }

    private void OnSounChange(int key, object[] param)
    {
        sound.SetVolume((int)ChannelType.BGM, AudioPlayerPrefs.MusicVolum);
        sound.SetVolume((int)ChannelType.SFX, AudioPlayerPrefs.SoundVolum);
    }
    private void OnClickSound(int key, object[] param)
    {
        //int r = UnityEngine.Random.Range(1, 3);
        PlaySound(AudioConst.SFX_Button);
    }
    public AudioClip PlaySound(string key)
    {
        DataSound data = GetData(key);
        if (data == null) return null;
        string path = AudioConst.AudioPath+data.path;
        AudioClip audio = ResouceManager.Instance.LoadResouce<AudioClip>(path); //GameResources.GetAudioClip(path);
        if (audio != null)
        {
            AudioSource source = sound.Play((int)data.channel, audio, data.loop == DataSound.Type.Loop);
            return audio;
        }
        return null;
    }
    public void PlaySound(string key, int volume_percent)
    {
        DataSound data = GetData(key);
        string path = AudioConst.AudioPath + data.path;
        AudioClip audio = ResouceManager.Instance.LoadResouce<AudioClip>(path);// GameResources.GetAudioClip(path);
        if (audio != null)
        {
            AudioSource source = sound.Play((int)data.channel, audio, data.loop == DataSound.Type.Loop);
            source.volume = source.volume * (volume_percent / 100.0f);
            Debug.LogWarning("source.volume" + source.volume);
        }
    }
    public void StopSound(string key)
    {
        DataSound data = GetData(key);
        sound.Stop((int)data.channel, data.path);
    }
    public void StopChannel(ChannelType channel)
    {
        sound.StopChanel((int)channel);
    }
    public void PauseSound(string key)
    {
        DataSound data = GetData(key);
        sound.Pause((int)data.channel, data.path);
    }
    public void ContinueSound(string key)
    {
        DataSound data = GetData(key);
        sound.Continue((int)data.channel, data.path);
    }
    public bool IsPlaying(string key)
    {
        DataSound data = GetData(key);
    return  sound.IsPlaying((int)data.channel, data.path);
    }
    public DataSound GetData(string key)
    {
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return null;
    }
}
