using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Channel
{
    private float volume = 1f;

    [SerializeField]
    private AudioSource prefab;

    [SerializeField]
    private AudioClip[] clips;

    public List<AudioSource> list = new List<AudioSource>();

    public Channel()
    {
    }
    public void Continue(string res)
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            AudioSource item = this.list[i];
            if (item.clip.name == res)
            {
                item.Play();
            }
        }
    }
    public void Pause(string res)
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            AudioSource item = this.list[i];
            if (item.clip.name == res)
            {
                item.Pause();
            }
        }
    }
    public AudioSource Play(int index, bool loop)
    {
        AudioSource audioSource = UnityEngine.Object.Instantiate(this.prefab);
        audioSource.clip=this.clips[index];
        audioSource.Play();
        audioSource.volume=this.volume;
        audioSource.loop=loop;
        this.list.Add(audioSource);
        audioSource.name=this.clips[index].name;
        return audioSource;
    }
    public AudioSource Play(AudioClip clip, bool loop)
    {
        AudioSource audioSource = UnityEngine.Object.Instantiate(this.prefab);
        audioSource.clip=clip;
        audioSource.Play();
        audioSource.volume=this.volume;
        audioSource.loop=loop;
        this.list.Add(audioSource);
        audioSource.name=clip.name;
        return audioSource;
    }
    public void SetVolume(int volume)
    {
        float single = (float)volume * 0.01f;
        for (int i = 0; i < this.list.Count; i++)
        {
            this.list[i].volume=single;
        }
        this.volume = single;
    }
    public void Stop()
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            AudioSource item = this.list[i];
            if (item != null)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
        this.list.Clear();
    }
    public void Stop(string res)
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            AudioSource item = this.list[i];
            if (item.clip.name == res)
            {
                this.list.Remove(item);
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
    }
    public void Update()
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            AudioSource item = this.list[i];
            if (!item.isPlaying && !item.loop)
            {
                this.list.Remove(item);
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
    }

}
