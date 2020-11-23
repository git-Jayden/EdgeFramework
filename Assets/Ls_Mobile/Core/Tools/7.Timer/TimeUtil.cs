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