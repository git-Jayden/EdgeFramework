/****************************************************
	文件：TimeUtil.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:28   	
	Features：
*****************************************************/
using System;
namespace EdgeFramework
{
    public class TimeUtil 
    {
        /// 获取当前时间戳
        /// </summary>
        /// <param name="tenBits">为真时获取10位时间戳,为假时获取13位时间戳.</param>
        /// <returns></returns>
        public static int GetTimeStamp(bool tenBits = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (tenBits)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return (int)ret;
        }
    }
}