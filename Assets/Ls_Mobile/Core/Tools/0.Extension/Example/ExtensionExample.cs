using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile.Example
{
    public class ExtensionExample : MonoBehaviour
    {
        private void Start()
        {
            QuickStart();
            CSharpExtensions();
            UnityExtensions();
        }

        private static void QuickStart()
        {
            // traditional style
            var playerPrefab = Resources.Load<GameObject>("no prefab don't run");
            var playerObj = Instantiate(playerPrefab);

            playerObj.transform.SetParent(null);
            playerObj.transform.localRotation = Quaternion.identity;
            playerObj.transform.localPosition = Vector3.left;
            playerObj.transform.localScale = Vector3.one;
            playerObj.layer = 1;
            playerObj.layer = LayerMask.GetMask("Default");

            Debug.Log("playerPrefab instantiated");

            // Extension's Style,same as above 
            Resources.Load<GameObject>("playerPrefab")
                .Instantiate()
                .transform
                .Parent(null)
                .LocalRotationIdentity()
                .LocalPosition(Vector3.left)
                .LocalScaleIdentity()
                .Layer(1)
                .Layer("Default")
                .ApplySelfTo(_ => { Debug.Log("playerPrefab instantiated"); });
        }

        private static void CSharpExtensions()
        {
            ClassExtention.Example();
            FuncOrActionOrEventExtension.Example();
            GenericExtention.Example();
            IEnumerableExtension.Example();
            IOExtension.Example();
            OOPExtension.Example();
            ReflectionExtension.Example();
            StringExtention.Example();
        }

        private static void UnityExtensions()
        {
            BehaviourExtension.Example();
            CameraExtension.Example();
            ColorExtension.Example();
            GameObjectExtension.Example();
            GraphicExtension.Example();
            ImageExtension.Example();
            ObjectExtension.Example();
            UnityActionExtension.Example();

            #region RectTransform

            #endregion

            #region Selectable

            #endregion

            #region Toggle

            #endregion
        }
    }
    /// <summary>
    /// CEGO EXAMPLE:GameObject 链式调用支持 
    /// </summary>
    public class GameObjectExample : MonoBehaviour
    {
        private void Start()
        {
            gameObject
                // 1. gameObject.SetActive(true)
                .Show()
                // 2. gameObject.SetActive(false)
                .Hide()
                // 3. gameObject.name = "Yeah" (这是UnityEngine.Object的API)
                .Name("Yeah")
                // 4. gameObject.layer = 10
                .Layer(0)
                // 5. gameObject.layer = LayerMask.NameToLayer("Default);
                .Layer("Default")
                // 6. Destroy(gameObject) (这是UnityEngine.Object的API)
                .DestroySelf();

            // 这里到会断掉，因为GameObject销毁之后就不希望再有操作了
            gameObject
                // 7. if (gameObject) Destroy(gameObject)
                .DestroySelfGracefully();


            GameObject instantiatedObj = null;

            gameObject
                // 8. Destroy(gameObject,1.5f)
                .DestroySelfAfterDelay(1.5f)
                // 9. if (gameObject) Destroy(gameObject,1.5f)
                .DestroySelfAfterDelayGracefully(1.5f)
                // 10. instantiatedObj = Instantiate(gameObject)
                .ApplySelfTo(selfObj => instantiatedObj = selfObj.Instantiate());

            Debug.Log(instantiatedObj);

            #region 通过MonoBehaviour去调用GameObject相关的API

            this
                // 1. this.gameObject.Show()
                .Show()
                // 2. this.gameObject.Hide()
                .Hide()
                // 3. this.gameObject.Name("Yeah")
                .Name("Yeah")
                // 4. gameObject.layer = 10
                .Layer(0)
                // 5. gameObject.layer = LayerMask.NameToLayer("Default);
                .Layer("Default")
                // 6. Destroy(this.gameObject)
                .DestroyGameObj();

            this
                // 7. if(this != null && this.gameObject) Destroy(this.gameObject)
                .DestroyGameObjGracefully();

            this
                // 8. this.gameObject.DestroySelfAfterDelay(1.5f)
                .DestroyGameObjAfterDelay(1.5f)
                // 9. if (this && this.gameObject0 this.gameObject.DestroySelfAfterDelay(1.5f)
                .DestroyGameObjAfterDelayGracefully(1.5f)
                // 10. instantiatedObj = Instantiate(this.gameObject)
                .ApplySelfTo(selfScript => instantiatedObj = selfScript.gameObject.Instantiate());

            #endregion


            #region 也可以使用Transform,因为Transform继承了Component,而Core里的所有的链式扩展都默认支持了Component

            transform
                // 1. transform.gameObject.SetActive(true)
                .Show()
                // 2. transform.gameObject.SetActive(false)
                .Hide()
                // 3. transform.name = "Yeah" (这是UnityEngine.Object的API)
                .Name("Yeah")
                // 4. transform.gameObject.layer = 10
                .Layer(0)
                // 5. transform.gameObject.layer = LayerMask.NameToLayer("Default);
                .Layer("Default")
                // 6. Destroy(transform.gameObject) (这是UnityEngine.Object的API)
                .DestroyGameObj();

            // 这里到会断掉，因为GameObject销毁之后就不希望再有操作了
            transform
                // 7. if (transform && gameObject) Destroy(transform.gameObject)
                .DestroyGameObjGracefully();

            transform
                // 8. Destroy(transform.gameObject,1.5f)
                .DestroyGameObjAfterDelay(1.5f)
                // 9. if (transform.gameObject) Destroy(gameObject,1.5f)
                .DestroyGameObjAfterDelayGracefully(1.5f)
                // 10. instantiatedTrans = Instantiate(transform.gameObject)
                .ApplySelfTo(selfTrans => instantiatedObj = selfTrans.gameObject.Instantiate());

            #endregion
        }
    }
    	/// <summary>
	/// CETR EXAMPLE:Transform 链式调用支持 
	/// </summary>
	public class TransformExample : MonoBehaviour
	{
		private void Start()
		{
			transform
				// 1. transform.SetParent(null)
				.Parent(null)
				// 2. transform.localPosition = Vector3.zero;
				//    transform.localRotation = Quaternion.identity;
				//    transform.localScale = Vector3.one;
				.LocalIdentity()
				// 3. transform.localPosition = Vector3.zero;
				.LocalPositionIdentity()
				.LocalPosition(Vector3.zero)
				.LocalPosition(0, 0, 0)
				.LocalPositionX(0)
				.LocalPositionY(0)
				.LocalPositionZ(0)
				// 4. transform.localRotation = Quaternion.identity;
				.LocalRotationIdentity()
				.LocalRotation(Quaternion.identity)
				// 5. transform.localScale = Vector3.one;
				.LocalScaleIdentity()
				.LocalScale(Vector3.one)
				.LocalScaleX(1)
				.LocalScaleY(1)
				.LocalScale(1, 1)
				.LocalScale(1, 1, 1)
				// 6. transform.position = Vector3.zero;
				//    transform.rotation = Quaternion.identity;
				//    transform.localScale = Vector3.one;
				.Identity()
				// 7. transform.position = Vector3.zero
				.PositionIdentity()
				.Position(Vector3.zero)
				.Position(0, 0, 0)
				.PositionX(0)
				.PositionY(0)
				.PositionZ(0)
				// 8. transform.rotation = Quaternion.identity;
				.RotationIdentity()
				.Rotation(Quaternion.identity)
				// 9. 删除所有子节点
				.DestroyAllChild()
				// 10. transform.SetAsFirstSibling();
				//     transform.SetAsLastSibling();
				//     transform.SetSiblingIndex(1);
				.AsFirstSibling()
				.AsLastSibling()
				.SiblingIndex(1);

			var textTrans = transform.FindByPath("BtnOK.Text");
			textTrans = transform.SeekTrans("Text");
			Debug.Log(textTrans);

		}
	}
}