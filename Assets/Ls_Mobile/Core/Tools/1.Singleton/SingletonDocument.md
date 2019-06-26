#### 单例 MonoSingleton 和 Singleton

***

  - ##### ***单例 MonoSingleton 和 Singleton***

```c#
//所要单例的Mono类继承 Singleton<所要单例的Mono类>
//注意:执行顺序为Awake-OnSingletonInit-Start-OnDestroy
public class SingletonExample : Singleton<SingletonExample>{}
// 移除实例
SingletonExample.Instance.Dispose();
//还可以重写OnSingletonInit方法
public override void OnSingletonInit(){}
//-----还可以作为属性来使用 需继承ISingleton
internal class SingletonExample : ISingleton
{
  public static SingletonExample Instance
  {
    get { return SingletonProperty<SingletonExample>.Instance; }
  }
}


//所要单例的Mono类继承 MonoSingleton<所要单例的Mono类>就可以使用
//MonoSingletonExample.Instance; 
public class MonoSingletonExample : MonoSingleton<MonoSingletonExample>{} 
//同样也可以使用属性 需要继承MonoBehaviour,ISingleton
internal class MonoSingletonExample : MonoBehaviour,ISingleton
{
  public static MonoSingletonExample Instance
  {
    get { return MonoSingletonProperty<MonoSingletonExample>.Instance; }
  }
}
//还可以这样使用 在DontDestroyOnLoad里面创建了一个AudioManager，
//在AudioManager内写入播放音效的方法外界就可以AudioManager.Instance.(播放方法)就可以了
 [LMonoSingletonPath("[Audio]/AudioManager")]
 public class AudioManager : MonoBehaviour, ISingleton
{
  public static AudioManager Instance
  {
    get { return LMonoSingletonProperty<AudioManager>.Instance; }
  }
}

```