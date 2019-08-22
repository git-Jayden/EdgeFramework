
using com.ls_mobile.tool;

namespace Ls_Mobile.Example
{
    public class PoolDocument
    {
        class Fish
        {

        }

        private void Start()
        {
            #region SimpleObjectPool
            var pool = new SimpleObjectPool<Fish>(() => { return new Fish(); }, initCount: 50);


            pool.CurCount.LogInfo();

            var fish = pool.Allocate();

            pool.CurCount.LogInfo();

            pool.Recycle(fish);

            pool.CurCount.LogInfo();
            #endregion



            #region SafeObjectPool

            SafeObjectPool<Bullet>.Instance.Init(50, 25);

            var bullet = Bullet.Allocate();

            SafeObjectPool<Bullet>.Instance.CurCount.LogInfo();

            bullet.Recycle2Cache();

            SafeObjectPool<Bullet>.Instance.CurCount.LogInfo();

            #endregion
        }
        class Bullet : IPoolable, IPoolType
        {

            public void OnRecycled()
            {
                "回收了".LogInfo();
            }

            public bool IsRecycled { get; set; }

            public static Bullet Allocate()
            {
                return SafeObjectPool<Bullet>.Instance.Allocate();
            }

            public void Recycle2Cache()
            {
                SafeObjectPool<Bullet>.Instance.Recycle(this);
            }
        }
    }
}