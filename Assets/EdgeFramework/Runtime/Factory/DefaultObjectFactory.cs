/****************************************************
	文件：DefaultObjectFactory.cs
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
    /// 默认对象工厂：相关对象是通过New 出来的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }
}