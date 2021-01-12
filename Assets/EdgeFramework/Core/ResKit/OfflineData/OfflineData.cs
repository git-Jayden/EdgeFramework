/****************************************************
	文件：OfflineData.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:58   	
	Features：
*****************************************************/

using EdgeFramework.Res;
using UnityEngine;
namespace EdgeFramework
{
    public class OfflineData : MonoBehaviour
    {
        public Rigidbody Rig;
        public  Collider Collid;
        public Transform[] AllPoint;
        public int[] AllPointChildCount;
        public bool[] AllPointActive;
        public Vector3[] Pos;
        public Vector3[] Scale;
        public Quaternion[] Rot;
        /// <summary>
        /// 还原属性
        /// </summary>
        public virtual void ResetProp()
        {
            int allPointCount = AllPoint.Length;
            for (int i = 0; i < allPointCount; i++)
            {
                Transform tempTrs = AllPoint[i];
                if (tempTrs != null)
                {
                    tempTrs.localPosition = Pos[i];
                    tempTrs.localScale = Scale[i];
                    tempTrs.localRotation = Rot[i];

                    if (AllPointActive[i])
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
                    if (tempTrs.childCount > AllPointChildCount[i])
                    {
                        int childCount = tempTrs.childCount;
                        for (int j = AllPointChildCount[i]; j < childCount; j++)
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
            Rig = GetComponentInChildren<Rigidbody>(true);
            Collid = GetComponentInChildren<Collider>(true);
            AllPoint = GetComponentsInChildren<Transform>(true);
            int allPointCount = AllPoint.Length;
            AllPointChildCount = new int[allPointCount];
            AllPointActive = new bool[allPointCount];
            Pos = new Vector3[allPointCount];
            Scale = new Vector3[allPointCount];
            Rot = new Quaternion[allPointCount];
            for (int i = 0; i < allPointCount; i++)
            {
                Transform temp = AllPoint[i] as Transform;
                AllPointChildCount[i] = temp.childCount;
                AllPointActive[i] = temp.gameObject.activeSelf;
                Pos[i] = temp.localPosition;
                Scale[i] = temp.localScale;
                Rot[i] = temp.localRotation;
            }


        }
    }
}