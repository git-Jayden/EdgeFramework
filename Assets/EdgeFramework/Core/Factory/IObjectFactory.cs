/****************************************************
	文件：IObjectFactory.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 17:16   	
	Features：
*****************************************************/

namespace EdgeFramework
{
    /// <summary>
    /// 对象工厂接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectFactory<T>
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        T Create();
    }
}