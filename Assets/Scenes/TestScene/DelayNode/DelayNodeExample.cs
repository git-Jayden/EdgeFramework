
using EdgeFramework;

using UnityEngine;

public class DelayNodeExample : MonoBehaviour
{
    void Start()
    {
        this.Delay(1.0f, () =>
        {
            Debug.Log("延时 1s");
        });

        var delay2s = DelayAction.Allocate(2.0f, () => { Debug.Log("延时 2s"); });
        this.ExecuteNode(delay2s);
    }

    private DelayAction mDelay3s = DelayAction.Allocate(3.0f, () => { Debug.Log("延时 3s"); });

    private void Update()
    {
        if (mDelay3s != null && !mDelay3s.Finished && mDelay3s.Execute(Time.deltaTime))
        {
            Debug.Log("Delay3s 执行完成");
        }
    }
}

