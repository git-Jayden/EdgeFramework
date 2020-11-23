using System;
using UnityEngine;

namespace EdgeFramework.AudioManager
{
    /// <summary>
    /// 音效的结构和特性
    /// </summary>
    [Serializable]
    public class SoundEffect : MonoBehaviour
    {

        /// <summary>
        /// 获取或设置音效的源。
        /// </summary>
        /// <value>The source.</value>
        public AudioSource Source { get; set; }

        /// <summary>
        /// 获取或设置音效结束播放时触发回调。
        /// </summary>
        /// <value>The callback.</value>
        public Action Callback { get; set; }

        /// <summary>
        /// 获取或设置音效以秒为单位播放的持续时间。
        /// </summary>
        /// <value>The duration.</value>
        public float Duration { get; set; }

        /// <summary>
        /// 获取或设置声效的原始音量。
        /// </summary>
        /// <value>The original volume.</value>
        public float OriginalVolume { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示该<see cref="T:Papae.UnitySDK.Managers.SoundEffect" />是一个单例。
        /// 这意味着只允许一个声效的实例被激活。
        /// </summary>
        /// <value><c>true</c> if repeat; otherwise, <c>false</c>.</value>
        public bool Singleton { get; set; }

        /// <summary>
        /// 获取或设置音效以秒为单位播放的剩余时间。
        /// </summary>
        /// <value>The duration.</value>
        public float RemainTime { get; set; }
        /// <summary>
        /// 获取音效的总长度(以秒为单位)。
        /// </summary>
        /// <value>The length.</value>
        public float Length
        {
            get
            {
                return this.Source.clip.length;
            }
        }

        /// <summary>
        /// 获取或设置音效的Name。
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.Source.clip.name;
            }
        }

        /// <summary>
        /// 获取音效播放播放进度0-1之间 可用于进度条
        /// </summary>
        /// <value>The normalised time.</value>
        public float Progress
        {
            get
            {
                return this.RemainTime / this.Duration;
            }
        }


        /// <summary>
        /// 获取播放位置(以秒为单位)播放到哪个位置了。
        /// </summary>
        /// <value>The playback position.</value>
        public float PlaybackPosition
        {
            get
            {
                return this.Source.time;
            }
        }






        public SoundEffect()
        {
        }
    }
}
