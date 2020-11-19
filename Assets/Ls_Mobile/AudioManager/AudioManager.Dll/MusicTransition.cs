using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ls_Mobile.AudioManager
{
    /// <summary>
    /// 音乐过渡效果的类型
    /// </summary>
    public enum MusicTransition
    {
        /// <summary>
        /// （无）立即播放下一个
        /// </summary>
        Swift,
        /// <summary>
        /// （淡入）淡出当前的音乐然后在下一个音乐中消失
        /// </summary>
        LinearFade,
        /// <summary>
        /// （无静音间隙）从当前音乐平滑过渡到下一个 交叉混合
        /// </summary>
        CrossFade
    }
}
