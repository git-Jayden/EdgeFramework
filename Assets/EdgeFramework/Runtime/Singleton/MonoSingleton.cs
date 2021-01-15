/****************************************************
	文件：MonoSingleton.cs
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
    public class MonoSingleton<T> : MonoBehaviour,ISingleton where T : MonoSingleton<T>
    {
        protected static T mInstance = null;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }
                return mInstance;
            }
        }

        public virtual void OnSingletonInit()
        {
         
        }
        public virtual void Dispose()
        {
            if (MonoSingletonCreator.IsUnitTestMode)
            {
                var curTrans = transform;
                do
                {
                    var parent = curTrans.parent;
                    DestroyImmediate(curTrans.gameObject);
                    curTrans = parent;
                } while (curTrans != null);

                mInstance = null;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        protected virtual void OnDestroy()
        {
            mInstance = null;
        }
    }
}
