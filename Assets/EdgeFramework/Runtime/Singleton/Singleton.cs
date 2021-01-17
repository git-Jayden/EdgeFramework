/****************************************************
	文件：Singleton.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:57   	
	Features：
*****************************************************/
namespace EdgeFramework
{
    public class Singleton<T>:ISingleton where T : Singleton<T>
    {
        private static T mInstance;
        static object mLock = new object();
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }
                return mInstance;
            }
         }
        public virtual void Dispose()
        {
            mInstance = null;
        }
        public Singleton() { }

        public virtual void OnSingletonInit()
        {
        }
    }
}
