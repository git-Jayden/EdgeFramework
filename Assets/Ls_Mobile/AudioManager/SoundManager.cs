using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private Channel[] chanels;

    public SoundManager()
    {
    }
    public void Continue(int chanel, string res)
    {
        this.chanels[chanel].Continue(res);
    }
    public void Pause(int chanel, string res)
    {
        this.chanels[chanel].Pause(res);
    }

    public AudioSource Play(int chanel, int index, bool loop)
    {
        AudioSource audioSource = this.chanels[chanel].Play(index, loop);
        audioSource.transform.SetParent(base.transform);
        return audioSource;
    }
    public AudioSource Play(int chanel, AudioClip clip, bool loop)
    {
        AudioSource audioSource = this.chanels[chanel].Play(clip, loop);
        audioSource.transform.SetParent(base.transform);
        return audioSource;
    }
    public void SetVolume(int index, int volume)
    {
        this.chanels[index].SetVolume(volume);
    }

    public void Stop(int chanel, string res)
    {
        this.chanels[chanel].Stop(res);
    }

    public void StopChanel(int chanel)
    {
        this.chanels[chanel].Stop();
    }
    public bool IsPlaying(int chanel,string clipname)
    {
        if (string.IsNullOrEmpty(clipname))
            return false;
        for (int i = 0; i < this.chanels[chanel].list.Count; i++)
        {
            if (this.chanels[chanel].list[i].name == clipname)
            {
                if (this.chanels[chanel].list[i].isPlaying)
                    return true;
            }
        }
        return false;
    }
    private void Update()
    {
        for (int i = 0; i < (int)this.chanels.Length; i++)
        {
            this.chanels[i].Update();
        }
    }
}
