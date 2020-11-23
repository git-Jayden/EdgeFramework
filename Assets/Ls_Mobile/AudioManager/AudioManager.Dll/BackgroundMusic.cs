using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EdgeFramework.AudioManager
{

    /// <summary>
    /// 背景音乐属性
    /// </summary>
    [Serializable]
        public struct BackgroundMusic
        {
        /// <summary>
        /// 背景音乐的当前Clip。
        /// </summary>
        public AudioClip CurrentClip;

        /// <summary>
        /// 下一个将要播放的Clip。
        /// </summary>
        public AudioClip NextClip;

        /// <summary>
        /// 音乐过渡类型
        /// </summary>
        public MusicTransition MusicTransition;

        /// <summary>
        /// 过渡的持续时间。
        /// </summary>
        public float TransitionDuration;
        }
    
}
