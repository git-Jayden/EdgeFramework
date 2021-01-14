
/****************************************************
	文件：IPool.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 17:13   	
	Features：
*****************************************************/
namespace EdgeFramework
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// 分配对象
        /// </summary>
        /// <returns></returns>
        T Allocate();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Recycle(T obj);
    }
}