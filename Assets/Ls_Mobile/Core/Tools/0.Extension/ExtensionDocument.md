#### Extension

***

- ***ObjectExtension Object的扩展*** 

```c#
//扩展后可以这样使用  
public static void Example()
        {
           var gameObject = Resources.Load<GameObject>("playerPrefab");
            //创建实例后修改名字并销毁自身
            gameObject.Instantiate()
                .Name("ExtensionExample")
                .DestroySelf();
            //创建实例后销毁自身并返回自身
            gameObject.Instantiate()
                .DestroySelfGracefully();
            //创建实例后延迟销毁自身
            gameObject.Instantiate()
                .DestroySelfAfterDelay(1.0f);
            //创建实例后延迟销毁自身并返回自身
            gameObject.Instantiate()
                .DestroySelfAfterDelayGracefully(1.0f);
            //执行函数后修改名字又执行函数继续修改函数并DontDestroyOnLoad
            gameObject
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("TestObj")
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("ExtensionExample")
                .DontDestroyOnLoad();
        }
```
- ***TransformExtension Transform的扩展*** 

```c#
  public static void Example()
        {
            var selfScript = new GameObject().AddComponent<MonoBehaviour>();
            var transform = selfScript.transform;

            transform
                ._SetParent(null)//设置父物体
                .ResetLocalTransform()//重置局部Transform
                .LocalPosReset()//重置局部坐标
                .LocalRotationReset()//重置局部旋转
                .LocalScaleReset()//重置局部缩放
                .SetLocalPos(Vector3.zero)//设置局部localposition
                .SetLocalPos(0, 0, 0)//设置局部localposition XYZ
                .SetLocalPosXY(0, 0)//设置局部localposition XY
                .SetLocalPosX(0)//设置局部localposition X
                .SetLocalPosY(0)//设置局部localposition Y
                .SetLocalPosZ(0)//设置局部localposition Z
                .SetLocalRotation(Quaternion.identity)//设置局部旋转
                .SetLocalScale(Vector3.one)//设置局部缩放
                .SetLocalScaleX(1.0f)//设置局部X缩放 
                .SetLocalScaleY(1.0f)//设置局部Y缩放
                .ResetTransform()//重置Transform
                .PosReset()//重置坐标
                .RotationReset()//重置旋转
                .SetPos(Vector3.zero)//设置positon
                .SetPosX(0)//设置positon X
                .SetPosY(0)//设置positon Y
                .SetPosZ(0)//设置positon Z
                .SetRotation(Quaternion.identity)//设置Rotation
                .DestroyAllChild()//销毁物体包括子物体
                ._SetAsLastSibling()//设置物体层级最后一层
                ._SetAsFirstSibling()//设置物体层级第一层
                ._SetSiblingIndex(0);//设置物体层级

            selfScript
                 ._SetParent(null)
                .ResetLocalTransform()
                .LocalPosReset()
                .LocalRotationReset()
                .LocalScaleReset()
                .SetLocalPos(Vector3.zero)
                .SetLocalPos(0, 0, 0)
                .SetLocalPosXY(0, 0)
                .SetLocalPosX(0)
                .SetLocalPosY(0)
                .SetLocalPosZ(0)
                .SetLocalRotation(Quaternion.identity)
                .SetLocalScale(Vector3.one)
                .SetLocalScaleX(1.0f)
                .SetLocalScaleY(1.0f)
                .ResetTransform()
                .PosReset()
                .RotationReset()
                .SetPos(Vector3.zero)
                .SetPosX(0)
                .SetPosY(0)
                .SetPosZ(0)
                .SetRotation(Quaternion.identity)
                .DestroyAllChild()
                ._SetAsLastSibling()
                ._SetAsFirstSibling()
                ._SetSiblingIndex(0);
        }

```
- ***GameObjectExtension GameObject的扩展*** 

  ```  C#
         public static void Example()
          {
              var gameObject = new GameObject();
              var transform = gameObject.transform;
              var selfScript = gameObject.AddComponent<MonoBehaviour>();
              var boxCollider = gameObject.AddComponent<BoxCollider>();
  
              gameObject.Show(); // gameObject.SetActive(true)
              selfScript.Show(); // this.gameObject.SetActive(true)
              boxCollider.Show(); // boxCollider.gameObject.SetActive(true)
              gameObject.transform.Show(); // transform.gameObject.SetActive(true)
  
              gameObject.Hide(); // gameObject.SetActive(false)
              selfScript.Hide(); // this.gameObject.SetActive(false)
              boxCollider.Hide(); // boxCollider.gameObject.SetActive(false)
              transform.Hide(); // transform.gameObject.SetActive(false)
  
              selfScript.DestroyGameObj();
              boxCollider.DestroyGameObj();
              transform.DestroyGameObj();
  
              selfScript.DestroyGameObjGracefully();
              boxCollider.DestroyGameObjGracefully();
              transform.DestroyGameObjGracefully();
  
              selfScript.DestroyGameObjAfterDelay(1.0f);
              boxCollider.DestroyGameObjAfterDelay(1.0f);
              transform.DestroyGameObjAfterDelay(1.0f);
  
              selfScript.DestroyGameObjAfterDelayGracefully(1.0f);
              boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
              transform.DestroyGameObjAfterDelayGracefully(1.0f);
  
              gameObject.Layer(0);
              selfScript.Layer(0);
              boxCollider.Layer(0);
              transform.Layer(0);
  
              gameObject.Layer("Default");
              selfScript.Layer("Default");
              boxCollider.Layer("Default");
              transform.Layer("Default");
          };
  ```