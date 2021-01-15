using UnityEngine;
using EdgeFramework;


public class SimpleObjectPoolTests
{
    public class Fish
    {
    }


    public void SimpleObjectPool_Test()
    {
        var fishPool = new SimpleObjectPool<Fish>(() => new Fish(), null, 100);

        Debug.Log(fishPool.CurCount);


        var fishOne = fishPool.Allocate();

        Debug.Log(fishPool.CurCount);

        fishPool.Recycle(fishOne);

        Debug.Log(fishPool.CurCount);

        for (var i = 0; i < 10; i++)
        {
            fishPool.Allocate();
        }

        Debug.Log(fishPool.CurCount);
    }
}

public class SafeObjectPoolTests
{
    class Msg : IPoolable
    {
        public void OnRecycled()
        {
            Debug.Log("OnRecycled");
        }

        public bool IsRecycled { get; set; }
    }


    public void SafeObjectPool_Test()
    {

        var msgPool = SafeObjectPool<Msg>.Instance;

        msgPool.Init(100, 50); // max count:100 init count: 50

        Debug.Log(msgPool.CurCount);

        var fishOne = msgPool.Allocate();

        Debug.Log(msgPool.CurCount);

        msgPool.Recycle(fishOne);
        Debug.Log(msgPool.CurCount);

        for (var i = 0; i < 10; i++)
        {
            msgPool.Allocate();
        }
        Debug.Log(msgPool.CurCount);
    }
}
