using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EdgeFramework
{
    public class OfflineData:MonoBehaviour
    {
        public Rigidbody rigibody;
        public  Collider collider;
        public Transform[] allPoint;
        public int[] allPointChildCount;
        public bool[] allPointActive;
        public Vector3[] pos;
        public Vector3[] scale;
        public Quaternion[] rot;
        /// <summary>
        /// 还原属性
        /// </summary>
        public virtual void ResetProp()
        {
            int allPointCount= allPoint.Length;
            for (int i = 0; i < allPointCount; i++)
            {
                Transform tempTrs = allPoint[i];
                if (tempTrs != null)
                {
                    tempTrs.localPosition = pos[i];
                    tempTrs.localScale = scale[i];
                    tempTrs.localRotation = rot[i];

                    if (allPointActive[i])
                    {
                        if (!tempTrs.gameObject.activeSelf)
                        {
                            tempTrs.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (tempTrs.gameObject.activeSelf)
                        {
                            tempTrs.gameObject.SetActive(false);
                        }
                    }
                    if (tempTrs.childCount > allPointChildCount[i])
                    {
                        int childCount = tempTrs.childCount;
                        for (int j = allPointChildCount[i]; j < childCount; j++)
                        {
                            GameObject tempObj = tempTrs.GetChild(j).gameObject;
                            if (!ObjectManager.Instance.IsObjectManagerCreat(tempObj))
                            {
                                GameObject.Destroy(tempObj);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 编辑器下保存初始数据
        /// </summary>
        public virtual void BindData()
        {
            rigibody = GetComponentInChildren<Rigidbody>(true);
            collider = GetComponentInChildren<Collider>(true);
            allPoint = GetComponentsInChildren<Transform>(true);
            int allPointCount = allPoint.Length;
            allPointChildCount = new int[allPointCount];
            allPointActive = new bool[allPointCount];
            pos = new Vector3[allPointCount];
            scale = new Vector3[allPointCount];
            rot = new Quaternion[allPointCount];
            for (int i = 0; i < allPointCount; i++)
            {
                Transform temp = allPoint[i] as Transform;
                allPointChildCount[i] = temp.childCount;
                allPointActive[i] = temp.gameObject.activeSelf;
                pos[i] = temp.localPosition;
                scale[i] = temp.localScale;
                rot[i] = temp.localRotation;
            }


        }
    }
}