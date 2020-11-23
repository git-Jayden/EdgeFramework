using UnityEngine;

namespace EdgeFramework.Util
{
    public class MathUtil
    {
        /// <summary>
        /// 输入百分比返回命中概率
        /// </summary>
        /// <param name="percent">百分比</param>
        /// <returns></returns>
        public static bool Percent(int percent)
        {
            return Random.Range(0, 100) < percent;
        }
        /// <summary>
        /// 从若干值中随机取出一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T GetRandomValueFrom<T>(params T[] values)
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
    }
}
