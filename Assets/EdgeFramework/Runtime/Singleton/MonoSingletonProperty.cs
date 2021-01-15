/****************************************************
	文件：MonoSingletonProperty.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/14 9:57   	
	Features：
*****************************************************/
using UnityEngine;

namespace EdgeFramework
{
    public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
    {
        private static T mInstance = null;

        public static T Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        public static void Dispose()
        {
            if (MonoSingletonCreator.IsUnitTestMode)
            {
                Object.DestroyImmediate(mInstance.gameObject);
            }
            else
            {
                Object.Destroy(mInstance.gameObject);
            }

            mInstance = null;
        }
    }
}
