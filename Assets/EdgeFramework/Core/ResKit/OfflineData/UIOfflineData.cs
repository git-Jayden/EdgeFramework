using EdgeFramework.Res;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************
	文件：UIOfflineData.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:58   	
	Features：
*****************************************************/
namespace EdgeFramework
{
    public class UIOfflineData:OfflineData
    {
        public Vector2[] anchorMax;
        public Vector2[] anchorMin;
        public Vector2[] pivot;
        public Vector2[] sizeDelta;
        public Vector3[] anchorePos;
        public ParticleSystem[] particle;

        public override void ResetProp()
        {
            int allPointCount = allPoint.Length;
            for (int i = 0; i < allPointCount; i++)
            {
                RectTransform tempTrs = allPoint[i] as RectTransform;
                if (tempTrs != null)
                {
                    tempTrs.localPosition = pos[i];
                    tempTrs.localRotation = rot[i];
                    tempTrs.localScale = scale[i];
                    tempTrs.anchorMax = anchorMax[i];
                    tempTrs.anchorMin = anchorMin[i];
                    tempTrs.pivot = pivot[i];
                    tempTrs.sizeDelta = sizeDelta[i];
                    tempTrs.anchoredPosition3D = anchorePos[i];
                }

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

            int particleCount = particle.Length;
            for (int i = 0; i < particleCount; i++)
            {
                particle[i].Clear(true);
                particle[i].Play();
            }
        }

        public override void BindData()
        {
            base.BindData();
            Transform[] allTrs = gameObject.GetComponentsInChildren<Transform>(true);
            int allTrsCount = allTrs.Length;
            for (int i = 0; i < allTrsCount; i++)
            {
                if (!(allTrs[i] is RectTransform))
                {
                    allTrs[i].gameObject.AddComponent<RectTransform>();
                }
            }
            allPoint = gameObject.GetComponentsInChildren<RectTransform>(true);
            particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            int allPointCount = allPoint.Length;
            allPointChildCount = new int[allPointCount];
            allPointActive = new bool[allPointCount];
            pos = new Vector3[allPointCount];
            rot = new Quaternion[allPointCount];
            scale = new Vector3[allPointCount];
            pivot = new Vector2[allPointCount];
            anchorMax = new Vector2[allPointCount];
            anchorMin = new Vector2[allPointCount];
            sizeDelta = new Vector2[allPointCount];
            anchorePos = new Vector3[allPointCount];

            for (int i = 0; i < allPointCount; i++)
            {
                RectTransform temp = allPoint[i] as RectTransform;
                allPointChildCount[i] = temp.childCount;
                allPointActive[i] = temp.gameObject.activeSelf;
                pos[i] = temp.localPosition;
                rot[i] = temp.localRotation;
                scale[i] = temp.localScale;
                pivot[i] = temp.pivot;
                anchorMax[i] = temp.anchorMax;
                anchorMin[i] = temp.anchorMin;
                sizeDelta[i] = temp.sizeDelta;
                anchorePos[i] = temp.anchoredPosition3D;
            }
        }

    }
}
