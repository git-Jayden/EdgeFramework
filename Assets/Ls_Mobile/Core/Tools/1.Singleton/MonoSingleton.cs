using UnityEngine;

namespace Ls_Mobile
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
