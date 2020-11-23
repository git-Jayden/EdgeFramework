
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EdgeFramework
{
    public class UnityAPIEx
    {
        //获取指定范围内随机数
        public static int GetRandomInt(int num1, int num2)
        {
            if (num1 < num2)
            {
                return UnityEngine.Random.Range(num1, num2);
            }
            else
            {
                return UnityEngine.Random.Range(num2, num1);
            }
        }
        //清理内存(一般在切换场景的时候调用)
        public static void ClearMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
        #region 封装PlayerPrefs的方法
        public static bool HasKey(string keyName)
        {
            return PlayerPrefs.HasKey(keyName);
        }
        public static int GetInt(string keyName)
        {
            return PlayerPrefs.GetInt(keyName);
        }
        public static void SetInt(string keyName, int valueInt)
        {
            PlayerPrefs.SetInt(keyName, valueInt);
        }
        public static float GetFloat(string keyName)
        {
            return PlayerPrefs.GetFloat(keyName);
        }
        public static void SetFloat(string keyName, float valueFloat)
        {
            PlayerPrefs.SetFloat(keyName, valueFloat);
        }
        public static string GetString(string keyName)
        {
            return PlayerPrefs.GetString(keyName);
        }
        public static void SetString(string keyName, string valueString)
        {
            PlayerPrefs.SetString(keyName, valueString);
        }
        public static void DeleteAllKey()
        {
            PlayerPrefs.DeleteAll();
        }
        public static void DeleteTheKey(string keyName)
        {
            PlayerPrefs.DeleteKey(keyName);
        }
        #endregion
    }


    public class TransformEx
    {
        //查找父物体下所有子物体
        public static Transform FindTheChild(GameObject goParent, string childName)
        {
            Transform searchTrans = goParent.transform.Find(childName);
            if (searchTrans == null)
            {
                foreach (Transform trans in goParent.transform)
                {
                    searchTrans = FindTheChild(trans.gameObject, childName);
                    if (searchTrans != null)
                    {
                        return searchTrans;
                    }
                }
            }
            return searchTrans;
        }

        public static void SetTheChildText(string text, Transform trans)
        {
            trans.GetComponentInChildren<Text>().text = text;
            //trans.GetChild(0).GetComponent<Text>().text = text;
        }
        public static List<Transform> GetChildList(GameObject goParent)
        {
            List<Transform> trans = new List<Transform>();
            foreach (Transform tran in goParent.transform)
            {
                if (tran.gameObject.activeSelf)
                {
                    if (tran.gameObject.name != goParent.name)
                    {
                        trans.Add(tran);
                    }
                }
            }
            if (trans.Count == 0)
                return null;
            return trans;
        }

        

    }
   

}