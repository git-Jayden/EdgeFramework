
using Ls_Mobile;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
public class AudioManager : MonoBehaviour
{
    SoundManager mSound;


    private Dictionary<string, DataSound> dic = new Dictionary<string, DataSound>();

    public AudioManager(SoundManager sound)
    {
        //读取并解析Resources/Data/config_sound文本里面的所有内容
        mSound = sound;
        TextAsset text = ResouceManager.Instance.LoadResouce<TextAsset>("config/config_sound");
        //TextAsset text = ResourcesEx.GetResources<TextAsset>(BundleName.Data, "config/config_sound");
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
        OnSounChange(null);
    }
    public void OnClickSound(object[] objs = null)
    {
        //int r = Random.Range(1, 3);
      //  PlaySound(SoundKey.SFX_OK);
    }
    public void OnBackSound(object[] objs = null)
    {
        //int r = Random.Range(1, 3);
       // PlaySound(SoundKey.SFX_back);
    }
    public void OnSounChange(object[] objs = null)
    {
        mSound.SetVolume((int)DataSound.Channel.BGM, GameOption.Bgm);
        mSound.SetVolume((int)DataSound.Channel.SFX, GameOption.Sfx);
    }


    public AudioClip PlaySound(string key, string dir = null)
    {
        DataSound data = GetData(key);
        if (data == null) return null;
        string path = string.IsNullOrEmpty(dir) ? data.path : dir + "/" + data.path;
        AudioClip audio = ResouceManager.Instance.LoadResouce<AudioClip>("Sound/" + path);

        if (audio != null)
        {
            AudioSource source = mSound.Play((int)data.channel, audio, data.loop == DataSound.Type.Loop);
            return audio;
        }
        return null;
    }
    public void PlaySound(string key, int volume_percent, string dir = null)
    {
        DataSound data = GetData(key);
        string path = string.IsNullOrEmpty(dir) ? data.path : dir + "/" + data.path;
        AudioClip audio = ResouceManager.Instance.LoadResouce<AudioClip>("Sound/" + path);

        if (audio != null)
        {
            AudioSource source = mSound.Play((int)data.channel, audio, data.loop == DataSound.Type.Loop);
            source.volume = source.volume * (volume_percent / 100.0f);
        }
    }
    public void StopSound(string key)
    {
        DataSound data = GetData(key);
        mSound.Stop((int)data.channel, data.path);
    }


    public void StopChannelSound(DataSound.Channel channel)
    {
        mSound.StopChanel((int)channel);
    }

    public void PauseSound(string key)
    {
        DataSound data = GetData(key);
        mSound.Pause((int)data.channel, data.path);
    }
    public void SetVolume(string key, int volume)
    {
        DataSound data = GetData(key);
        mSound.SetVolume((int)data.channel, volume);
    }
    public void ContinueSound(string key)
    {
        DataSound data = GetData(key);
        mSound.Continue((int)data.channel, data.path);
    }
    public bool IsPlaying(DataSound.Channel channel, string key)
    {
        DataSound data = GetData(key);
        AudioClip audio = ResouceManager.Instance.LoadResouce<AudioClip>("Sound/" + data.path);

        return mSound.IsPlaying((int)channel, audio.name);
    }


    public  DataSound GetData(string key)
    {
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return null;
    }
}
