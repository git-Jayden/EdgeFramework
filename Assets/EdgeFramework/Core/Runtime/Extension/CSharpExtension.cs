/****************************************************
	�ļ���CSharpExtension.cs
	Author��JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date��2021/01/14 11:47   	
	Features��
*****************************************************/
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnityEngine.Events;

namespace EdgeFramework
{
    /// <summary>
    /// ��ö�ٵļ�����չ��Array��List<T>��Dictionary<K,V>)
    /// </summary>
    public static class IEnumerableExtension
    {
        #region Array Extension

        /// <summary>
        /// ��������
        /// <code>
        /// var testArray = new[] { 1, 2, 3 };
        /// testArray.ForEach(number => number.LogInfo());
        /// </code>
        /// </summary>
        /// <returns>The each.</returns>
        /// <param name="selfArray">Self array.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <returns> �����Լ� </returns>
        public static T[] ForEach<T>(this T[] selfArray, Action<T> action)
        {
            Array.ForEach(selfArray, action);
            return selfArray;
        }

        /// <summary>
        /// ���� IEnumerable
        /// <code>
        /// // IEnumerable<T>
        /// IEnumerable<int> testIenumerable = new List<int> { 1, 2, 3 };
        /// testIenumerable.ForEach(number => number.LogInfo());
        /// // ֧���ֵ�ı���
        /// new Dictionary<string, string>()
        ///         .ForEach(keyValue => Log.I("key:{0},value:{1}", keyValue.Key, keyValue.Value));
        /// </code>
        /// </summary>
        /// <returns>The each.</returns>
        /// <param name="selfArray">Self array.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> selfArray, Action<T> action)
        {
            if (action == null) throw new ArgumentException();
            foreach (var item in selfArray)
            {
                action(item);
            }

            return selfArray;
        }

        #endregion

        #region List Extension

        /// <summary>
        /// �������
        /// <code>
        /// var testList = new List<int> { 1, 2, 3 };
        /// testList.ForEachReverse(number => number.LogInfo()); // 3, 2, 1
        /// </code>
        /// </summary>
        /// <returns>�����Լ�</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
        {
            if (action == null) throw new ArgumentException();

            for (var i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i]);

            return selfList;
        }

        /// <summary>
        /// ����������ɻ������)
        /// <code>
        /// var testList = new List<int> { 1, 2, 3 };
        /// testList.ForEachReverse((number,index)=> number.LogInfo()); // 3, 2, 1
        /// </code>
        /// </summary>
        /// <returns>The each reverse.</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T, int> action)
        {
            if (action == null) throw new ArgumentException();

            for (var i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i], i);

            return selfList;
        }

        /// <summary>
        /// �����б�(�ɻ��������
        /// <code>
        /// var testList = new List<int> {1, 2, 3 };
        /// testList.Foreach((number,index)=>number.LogInfo()); // 1, 2, 3,
        /// </code>
        /// </summary>
        /// <typeparam name="T">�б�����</typeparam>
        /// <param name="list">Ŀ���</param>
        /// <param name="action">��Ϊ</param>
        public static void ForEach<T>(this List<T> list, Action<int, T> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                action(i, list[i]);
            }
        }
        /// <summary>
        /// �������б���Ԫ��
        /// </summary>
        /// <typeparam name="T">Ԫ������</typeparam>
        /// <param name="list">�б�</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        #endregion

        #region Dictionary Extension

        /// <summary>
        /// �ϲ��ֵ�
        /// <code>
        /// // ʾ��
        /// var dictionary1 = new Dictionary<string, string> { { "1", "2" } };
        /// var dictionary2 = new Dictionary<string, string> { { "3", "4" } };
        /// var dictionary3 = dictionary1.Merge(dictionary2);
        /// dictionary3.ForEach(pair => Log.I("{0}:{1}", pair.Key, pair.Value));
        /// </code>
        /// </summary>
        /// <returns>The merge.</returns>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="dictionaries">Dictionaries.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.Aggregate(dictionary,
                (current, dict) => current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>
        /// �����ֵ�
        /// <code>
        /// var dict = new Dictionary<string,string> {{"name","liangxie},{"age","18"}};
        /// dict.ForEach((key,value)=> Log.I("{0}:{1}",key,value);//  name:liangxie    age:18
        /// </code>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="action"></param>
        public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
        {
            var dictE = dict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                action(current.Key, current.Value);
            }

            dictE.Dispose();
        }

        /// <summary>
        /// �ֵ�����µĴʵ�
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="addInDict"></param>
        /// <param name="isOverride"></param>
        public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> addInDict,
            bool isOverride = false)
        {
            var dictE = addInDict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                if (dict.ContainsKey(current.Key))
                {
                    if (isOverride)
                        dict[current.Key] = current.Value;
                    continue;
                }

                dict.Add(current.Key, current.Value);
            }

            dictE.Dispose();
        }

        /// <summary>
        /// ���Ը���key�õ�value���õ��˵Ļ�ֱ�ӷ���value��û�еõ�ֱ�ӷ���null
        /// this Dictionary<Tkey,Tvalue> dict ����ֵ��ʾ����Ҫ��ȡֵ���ֵ�
        /// </summary>
        public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
        {
            Tvalue value;
            dict.TryGetValue(key, out value);
            return value;
        }
        #endregion
    }

    /// <summary>
    /// �� System.IO ��һЩ��չ
    /// </summary>
    public static class IOExtension
    {

        /// <summary>
        /// ����ļ���������Ӧ�ļ���
        /// </summary>
        public static bool CheckFileAndCreateDirWhenNeeded(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            var fileInfo = new FileInfo(filePath);
            var dirInfo = fileInfo.Directory;
            if (!dirInfo.Exists) Directory.CreateDirectory(dirInfo.FullName);
            return true;
        }
        /// <summary>
        /// �����µ��ļ���,��������򲻴���
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.CreateDirIfNotExists();
        /// // ���Ϊ���� Assets Ŀ¼�´��� TestFolder
        /// </code>
        /// </summary>
        public static string CreateDirIfNotExists(this string dirFullPath)
        {
            if (!Directory.Exists(dirFullPath))
            {
                Directory.CreateDirectory(dirFullPath);
            }

            return dirFullPath;
        }


        /// <summary>
        /// ɾ���ļ��У��������
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.DeleteDirIfExists();
        /// // ���Ϊ���� Assets Ŀ¼��ɾ���� TestFolder
        /// </code>
        /// </summary>
        public static void DeleteDirIfExists(this string dirFullPath)
        {
            if (Directory.Exists(dirFullPath))
            {
                Directory.Delete(dirFullPath, true);
            }
        }

        /// <summary>
        /// ��� Dir������Ŀ¼),������ڡ�
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.EmptyDirIfExists();
        /// // ���Ϊ������� TestFolder �������
        /// </code>
        /// </summary>
        public static void EmptyDirIfExists(this string dirFullPath)
        {
            if (Directory.Exists(dirFullPath))
            {
                Directory.Delete(dirFullPath, true);
            }

            Directory.CreateDirectory(dirFullPath);
        }

        /// <summary>
        /// ɾ���ļ� �������
        /// <code>
        /// // ʾ��
        /// var filePath = "Assets/Test.txt";
        /// File.Create("Assets/Test);
        /// filePath.DeleteFileIfExists();
        /// // ���Ϊ��ɾ���� Test.txt
        /// </code>
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns> �Ƿ������ɾ������ </returns>
        public static bool DeleteFileIfExists(this string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
                return true;
            }

            return false;
        }

        /// <summary>
        /// �ϲ�·��
        /// <code>
        /// // ʾ����
        /// Application.dataPath.CombinePath("Resources").LogInfo();  // /projectPath/Assets/Resources
        /// </code>
        /// </summary>
        /// <param name="selfPath"></param>
        /// <param name="toCombinePath"></param>
        /// <returns> �ϲ����·�� </returns>
        public static string CombinePath(this string selfPath, string toCombinePath)
        {
            return Path.Combine(selfPath, toCombinePath);
        }

        #region δ��������

        /// <summary>
        /// ���ļ���
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolder(string path)
        {
#if UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", path);
#endif
        }


        /// <summary>
        /// ��ȡ�ļ�����
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string fileName)
        {
            fileName = MakePathStandard(fileName);
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }

        /// <summary>
        /// ��ȡ�ļ���
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFileName(string path, char separator = '/')
        {
            path = IOExtension.MakePathStandard(path);
            return path.Substring(path.LastIndexOf(separator) + 1);
        }

        /// <summary>
        /// ��ȡ������׺���ļ���
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtention(string fileName, char separator = '/')
        {
            return GetFilePathWithoutExtention(GetFileName(fileName, separator));
        }

        /// <summary>
        /// ��ȡ������׺���ļ�·��
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePathWithoutExtention(string fileName)
        {
            if (fileName.Contains("."))
                return fileName.Substring(0, fileName.LastIndexOf('.'));
            return fileName;
        }

        /// <summary>
        /// ʹĿ¼����,Path������Ŀ¼���������ļ���
        /// </summary>
        /// <param name="path"></param>
        public static void MakeFileDirectoryExist(string path)
        {
            string root = Path.GetDirectoryName(path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }

        /// <summary>
        /// ʹĿ¼����
        /// </summary>
        /// <param name="path"></param>
        public static void MakeDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// ��ȡ���ļ���
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathParentFolder(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }


        /// <summary>
        /// ʹ·����׼����ȥ���ո񲢽�����'\'ת��Ϊ'/'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakePathStandard(string path)
        {
            return path.Trim().Replace("\\", "/");
        }

        public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true,
            string suffix = "")
        {
            var pathList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                var dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        public static List<string> GetDirSubDirNameList(this string dirABSPath)
        {
            var di = new DirectoryInfo(dirABSPath);

            var dirs = di.GetDirectories();

            return dirs.Select(d => d.Name).ToList();
        }

        public static string GetFileName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");

            return lastIndex >= 0 ? name.Substring(lastIndex + 1) : name;
        }

        public static string GetFileNameWithoutExtend(this string absOrAssetsPath)
        {
            var fileName = GetFileName(absOrAssetsPath);
            var lastIndex = fileName.LastIndexOf(".");

            return lastIndex >= 0 ? fileName.Substring(0, lastIndex) : fileName;
        }

        public static string GetFileExtendName(this string absOrAssetsPath)
        {
            var lastIndex = absOrAssetsPath.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return absOrAssetsPath.Substring(lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirPath(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        public static string GetLastDirName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return absOrAssetsPath.EndsWith("/") ? dirs[dirs.Length - 2] : dirs[dirs.Length - 1];
        }

        #endregion
    }

    /// <summary>
    /// ������չ
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// ͨ�����䷽ʽ���ú���
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName">������</param>
        /// <param name="args">����</param>
        /// <returns></returns>
        public static object InvokeByReflect(this object obj, string methodName, params object[] args)
        {
            var methodInfo = obj.GetType().GetMethod(methodName);
            return methodInfo == null ? null : methodInfo.Invoke(obj, args);
        }

        /// <summary>
        /// ͨ�����䷽ʽ��ȡ��ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">����</param>
        /// <returns></returns>
        public static object GetFieldByReflect(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            return fieldInfo == null ? null : fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// ͨ�����䷽ʽ��ȡ����
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">������</param>
        /// <returns></returns>
        public static object GetPropertyByReflect(this object obj, string propertyName, object[] index = null)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, index);
        }

        /// <summary>
        /// ӵ������
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo prop, Type attributeType, bool inherit)
        {
            return prop.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// ӵ������
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this FieldInfo field, Type attributeType, bool inherit)
        {
            return field.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// ӵ������
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// ӵ������
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this MethodInfo method, Type attributeType, bool inherit)
        {
            return method.GetCustomAttributes(attributeType, inherit).Length > 0;
        }


        /// <summary>
        /// ��ȡ��һ������
        /// </summary>
        public static T GetFirstAttribute<T>(this MethodInfo method, bool inherit) where T : Attribute
        {
            var attrs = (T[])method.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// ��ȡ��һ������
        /// </summary>
        public static T GetFirstAttribute<T>(this FieldInfo field, bool inherit) where T : Attribute
        {
            var attrs = (T[])field.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// ��ȡ��һ������
        /// </summary>
        public static T GetFirstAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
        {
            var attrs = (T[])prop.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// ��ȡ��һ������
        /// </summary>
        public static T GetFirstAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var attrs = (T[])type.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }
    }

    /// <summary>
    /// ������չ
    /// </summary>
    public static class TypeEx
    {
        /// <summary>
        /// ��ȡĬ��ֵ
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object DefaultForType(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }

    /// <summary>
    /// �ַ�����չ
    /// </summary>
    public static class StringExtention
    {
        public static void Example()
        {
            var emptyStr = string.Empty;
            emptyStr = emptyStr.Append("appended").Append("1").ToString();
        }

        /// <summary>
        /// ����
        /// </summary>
        private static readonly char[] mCachedSplitCharArray = { '.' };

        /// <summary>
        /// ����ĸ��д
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UppercaseFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// ����ĸСд
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LowercaseFirst(this string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }



        /// <summary>
        /// ���ǰ׺
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toAppend"></param>
        /// <returns></returns>
        public static StringBuilder Append(this string selfStr, string toAppend)
        {
            return new StringBuilder(selfStr).Append(toAppend);
        }

        /// <summary>
        /// ��Ӻ�׺
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toPrefix"></param>
        /// <returns></returns>
        public static string AddPrefix(this string selfStr, string toPrefix)
        {
            return new StringBuilder(toPrefix).Append(selfStr).ToString();
        }



        /// <summary>
        /// ���һ������
        /// </summary>
        /// <param name="selfUrl"></param>
        /// <returns></returns>
        public static string LastWord(this string selfUrl)
        {
            return selfUrl.Split('/').Last();
        }

        /// <summary>
        /// ��������������
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        public static int ToInt(this string selfStr, int defaulValue = 0)
        {
            var retValue = defaulValue;
            return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

        /// <summary>
        /// ������ʱ������2005-11-5 13:21:25
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string selfStr, DateTime defaultValue = default(DateTime))
        {
            var retValue = defaultValue;
            return DateTime.TryParse(selfStr, out retValue) ? retValue : defaultValue;
        }


        /// <summary>
        /// ���� Float ����
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        public static float ToFloat(this string selfStr, float defaulValue = 0)
        {
            var retValue = defaulValue;
            return float.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

        /// <summary>
        /// �Ƿ���������ַ�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasChinese(this string input)
        {
            return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
        }
        /// <summary>
        /// ɾ���ض��ַ�
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string RemoveString(this string str, params string[] targets)
        {
            return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
        }
    }

    public static class CameraExtension
    {
        public static void Example()
        {
            var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
            Debug.Log(screenshotTexture2D.width);
        }

        public static Texture2D CaptureCamera(this Camera camera, Rect rect)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(renderTexture);

            return screenShot;
        }
    }

    public static class ColorExtension
    {
        public static void Example()
        {
            var color = "#C5563CFF".HtmlStringToColor();
            Debug.Log(color);
        }

        /// <summary>
        /// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static Color HtmlStringToColor(this string htmlString)
        {
            Color retColor;
            var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out retColor);
            return parseSucceed ? retColor : Color.black;
        }

    }

    /// <summary>
    /// GameObject's Util/Static This Extension
    /// </summary>
    public static class GameObjectExtension
    {
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
        }

        #region CEGO001 Show

        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }

        public static T Show<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Show();
            return selfComponent;
        }

        #endregion

        #region CEGO002 Hide

        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

        public static T Hide<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Hide();
            return selfComponent;
        }

        #endregion

        #region CEGO003 DestroyGameObj

        public static void DestroyGameObj<T>(this T selfBehaviour) where T : Component
        {
            selfBehaviour.gameObject.DestroySelf();
        }

        #endregion

        #region CEGO004 DestroyGameObjGracefully

        public static void DestroyGameObjGracefully<T>(this T selfBehaviour) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfGracefully();
            }
        }

        #endregion

        #region CEGO005 DestroyGameObjGracefully

        public static T DestroyGameObjAfterDelay<T>(this T selfBehaviour, float delay) where T : Component
        {
            selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            return selfBehaviour;
        }

        public static T DestroyGameObjAfterDelayGracefully<T>(this T selfBehaviour, float delay) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            }

            return selfBehaviour;
        }

        #endregion

        #region CEGO006 Layer

        public static GameObject Layer(this GameObject selfObj, int layer)
        {
            selfObj.layer = layer;
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, int layer) where T : Component
        {
            selfComponent.gameObject.layer = layer;
            return selfComponent;
        }

        public static GameObject Layer(this GameObject selfObj, string layerName)
        {
            selfObj.layer = LayerMask.NameToLayer(layerName);
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, string layerName) where T : Component
        {
            selfComponent.gameObject.layer = LayerMask.NameToLayer(layerName);
            return selfComponent;
        }

        #endregion

        #region CEGO007 Component

        public static T GetOrAddComponent<T>(this GameObject selfComponent) where T : Component
        {
            var comp = selfComponent.gameObject.GetComponent<T>();
            return comp ? comp : selfComponent.gameObject.AddComponent<T>();
        }

        public static Component GetOrAddComponent(this GameObject selfComponent, Type type)
        {
            var comp = selfComponent.gameObject.GetComponent(type);
            return comp ? comp : selfComponent.gameObject.AddComponent(type);
        }

        #endregion
    }

    public static class GraphicExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image = gameObject.AddComponent<Image>();
            var rawImage = gameObject.AddComponent<RawImage>();

            // image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
            image.ColorAlpha(1.0f);
            rawImage.ColorAlpha(1.0f);
        }

        public static T ColorAlpha<T>(this T selfGraphic, float alpha) where T : Graphic
        {
            var color = selfGraphic.color;
            color.a = alpha;
            selfGraphic.color = color;
            return selfGraphic;
        }
    }

    public static class ObjectExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();

            gameObject.Instantiate()
                .Name("ExtensionExample")
                .DestroySelf();

            gameObject.Instantiate()
                .DestroySelfGracefully();

            gameObject.Instantiate()
                .DestroySelfAfterDelay(1.0f);

            gameObject.Instantiate()
                .DestroySelfAfterDelayGracefully(1.0f);

            gameObject
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("TestObj")
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("ExtensionExample")
                .DontDestroyOnLoad();
        }


        #region CEUO001 Instantiate

        public static T Instantiate<T>(this T selfObj) where T : UnityEngine.Object
        {
            return Object.Instantiate(selfObj);
        }

        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation) where T : Object
        {
            return (T)Object.Instantiate((UnityEngine.Object)selfObj, position, rotation);
        }

        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : Object
        {
            return (T)Object.Instantiate((Object)selfObj, position, rotation, parent);
        }


        public static T InstantiateWithParent<T>(this T selfObj, Transform parent, bool worldPositionStays)
            where T : Object
        {
            return (T)Object.Instantiate((Object)selfObj, parent, worldPositionStays);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Transform parent) where T : Object
        {
            return Object.Instantiate(selfObj, parent, false);
        }

        #endregion

        #region CEUO002 Name

        public static T Name<T>(this T selfObj, string name) where T : Object
        {
            selfObj.name = name;
            return selfObj;
        }

        #endregion

        #region CEUO003 Destroy Self

        public static void DestroySelf<T>(this T selfObj) where T : Object
        {
            Object.Destroy(selfObj);
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj);
            }

            return selfObj;
        }

        #endregion

        #region CEUO004 Destroy Self AfterDelay 

        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : Object
        {
            Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }

        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj, delay);
            }

            return selfObj;
        }

        #endregion

        #region CEUO005 Apply Self To 

        public static T ApplySelfTo<T>(this T selfObj, System.Action<T> toFunction) where T : Object
        {
            toFunction?.Invoke(selfObj);
            return selfObj;
        }

        #endregion

        #region CEUO006 DontDestroyOnLoad

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : Object
        {
            Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }

        #endregion

        public static T As<T>(this object selfObj) where T : class
        {
            return selfObj as T;
        }
    }

    public static class RectTransformExtension
    {
        public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
        }

        public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.x = anchorPosX;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }

        public static RectTransform AnchorPosY(this RectTransform selfRectTrans, float anchorPosY)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.y = anchorPosY;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }

        public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.x = sizeWidth;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.y = sizeHeight;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
        }
    }

    /// <summary>
    /// Transform's Extension
    /// </summary>
    public static class TransformExtension
    {
        public static void Example()
        {
            var selfScript = new GameObject().AddComponent<MonoBehaviour>();
            var transform = selfScript.transform;

            transform
                // .Parent(null)
                .LocalIdentity()
                .LocalPositionIdentity()
                .LocalRotationIdentity()
                .LocalScaleIdentity()
                .LocalPosition(Vector3.zero)
                .LocalPosition(0, 0, 0)
                .LocalPosition(0, 0)
                .LocalPositionX(0)
                .LocalPositionY(0)
                .LocalPositionZ(0)
                .LocalRotation(Quaternion.identity)
                .LocalScale(Vector3.one)
                .LocalScaleX(1.0f)
                .LocalScaleY(1.0f)
                .Identity()
                .PositionIdentity()
                .RotationIdentity()
                .Position(Vector3.zero)
                .PositionX(0)
                .PositionY(0)
                .PositionZ(0)
                .Rotation(Quaternion.identity)
                .DestroyChildren()
                .AsLastSibling()
                .AsFirstSibling()
                .SiblingIndex(0);

            selfScript
                // .Parent(null)
                .LocalIdentity()
                .LocalPositionIdentity()
                .LocalRotationIdentity()
                .LocalScaleIdentity()
                .LocalPosition(Vector3.zero)
                .LocalPosition(0, 0, 0)
                .LocalPosition(0, 0)
                .LocalPositionX(0)
                .LocalPositionY(0)
                .LocalPositionZ(0)
                .LocalRotation(Quaternion.identity)
                .LocalScale(Vector3.one)
                .LocalScaleX(1.0f)
                .LocalScaleY(1.0f)
                .Identity()
                .PositionIdentity()
                .RotationIdentity()
                .Position(Vector3.zero)
                .PositionX(0)
                .PositionY(0)
                .PositionZ(0)
                .Rotation(Quaternion.identity)
                .DestroyChildren()
                .AsLastSibling()
                .AsFirstSibling()
                .SiblingIndex(0);
        }


        /// <summary>
        /// �����һЩ����,���ÿ������
        /// </summary>
        private static Vector3 mLocalPos;

        private static Vector3 mScale;
        private static Vector3 mPos;

        #region CETR001 Parent

        public static T Parent<T>(this T selfComponent, Component parentComponent) where T : Component
        {
            selfComponent.transform.SetParent(parentComponent == null ? null : parentComponent.transform);
            return selfComponent;
        }

        /// <summary>
        /// ���ó�Ϊ���� Transform
        /// </summary>
        /// <returns>The root transform.</returns>
        /// <param name="selfComponent">Self component.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T AsRootTransform<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetParent(null);
            return selfComponent;
        }

        #endregion

        #region CETR002 LocalIdentity

        public static T LocalIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            selfComponent.transform.localRotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR003 LocalPosition

        public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
        {
            selfComponent.transform.localPosition = localPos;
            return selfComponent;
        }

        public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localPosition;
        }


        public static T LocalPosition<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.localPosition = new Vector3(x, y, z);
            return selfComponent;
        }

        public static T LocalPosition<T>(this T selfComponent, float x, float y) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.x = x;
            mLocalPos.y = y;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionX<T>(this T selfComponent, float x) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.x = x;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionY<T>(this T selfComponent, float y) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.y = y;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionZ<T>(this T selfComponent, float z) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.z = z;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }


        public static T LocalPositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            return selfComponent;
        }

        #endregion

        #region CETR004 LocalRotation

        public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localRotation;
        }

        public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
        {
            selfComponent.transform.localRotation = localRotation;
            return selfComponent;
        }

        public static T LocalRotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localRotation = Quaternion.identity;
            return selfComponent;
        }

        #endregion

        #region CETR005 LocalScale

        public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
        {
            selfComponent.transform.localScale = scale;
            return selfComponent;
        }

        public static Vector3 GetLocalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localScale;
        }

        public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one * xyz;
            return selfComponent;
        }

        public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            mScale.y = y;
            mScale.z = z;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            mScale.y = y;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.y = y;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.z = z;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR006 Identity

        public static T Identity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            selfComponent.transform.rotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR007 Position

        public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
        {
            selfComponent.transform.position = position;
            return selfComponent;
        }

        public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.position;
        }

        public static T Position<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.position = new Vector3(x, y, z);
            return selfComponent;
        }

        public static T Position<T>(this T selfComponent, float x, float y) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = x;
            mPos.y = y;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            return selfComponent;
        }

        public static T PositionX<T>(this T selfComponent, float x) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = x;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionX<T>(this T selfComponent, Func<float, float> xSetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = xSetter(mPos.x);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionY<T>(this T selfComponent, float y) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.y = y;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionY<T>(this T selfComponent, Func<float, float> ySetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.y = ySetter(mPos.y);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionZ<T>(this T selfComponent, float z) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.z = z;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionZ<T>(this T selfComponent, Func<float, float> zSetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.z = zSetter(mPos.z);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        #endregion

        #region CETR008 Rotation

        public static T RotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.rotation = Quaternion.identity;
            return selfComponent;
        }

        public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
        {
            selfComponent.transform.rotation = rotation;
            return selfComponent;
        }

        public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.rotation;
        }

        #endregion

        #region CETR009 WorldScale/LossyScale/GlobalScale/Scale

        public static Vector3 GetGlobalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetWorldScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetLossyScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        #endregion

        #region CETR010 Destroy All Child

        [Obsolete("������ ��ʹ�� DestroyChildren")]
        public static T DestroyAllChild<T>(this T selfComponent) where T : Component
        {
            return selfComponent.DestroyChildren();
        }

        [Obsolete("������ ��ʹ�� DestroyChildren")]
        public static GameObject DestroyAllChild(this GameObject selfGameObj)
        {
            return selfGameObj.DestroyChildren();
        }

        public static T DestroyChildren<T>(this T selfComponent) where T : Component
        {
            var childCount = selfComponent.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfComponent;
        }

        public static GameObject DestroyChildren(this GameObject selfGameObj)
        {
            var childCount = selfGameObj.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfGameObj.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfGameObj;
        }

        #endregion

        #region CETR011 Sibling Index

        public static T AsLastSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsLastSibling();
            return selfComponent;
        }

        public static T AsFirstSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsFirstSibling();
            return selfComponent;
        }

        public static T SiblingIndex<T>(this T selfComponent, int index) where T : Component
        {
            selfComponent.transform.SetSiblingIndex(index);
            return selfComponent;
        }

        #endregion


        public static Transform FindByPath(this Transform selfTrans, string path)
        {
            return selfTrans.Find(path.Replace(".", "/"));
        }

        public static Transform SeekTrans(this Transform selfTransform, string uniqueName)
        {
            var childTrans = selfTransform.Find(uniqueName);

            if (null != childTrans)
                return childTrans;

            foreach (Transform trans in selfTransform)
            {
                childTrans = trans.SeekTrans(uniqueName);

                if (null != childTrans)
                    return childTrans;
            }

            return null;
        }

        public static T ShowChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).gameObject.Show();
            return selfComponent;
        }

        public static T HideChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).Hide();
            return selfComponent;
        }

        public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
        {
            selfTrans.SetParent(fromTrans.parent);
            selfTrans.localPosition = fromTrans.localPosition;
            selfTrans.localRotation = fromTrans.localRotation;
            selfTrans.localScale = fromTrans.localScale;
        }

        /// <summary>
        /// �ݹ���������壬�����ú���
        /// </summary>
        /// <param name="tfParent"></param>
        /// <param name="action"></param>
        public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
        {
            action(tfParent);
            foreach (Transform tfChild in tfParent)
            {
                tfChild.ActionRecursion(action);
            }
        }

        /// <summary>
        /// �ݹ��������ָ�������ֵ�������
        /// </summary>
        /// <param name="tfParent">��ǰTransform</param>
        /// <param name="name">Ŀ����</param>
        /// <param name="stringComparison">�ַ����ȽϹ���</param>
        /// <returns></returns>
        public static Transform FindChildRecursion(this Transform tfParent, string name,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (tfParent.name.Equals(name, stringComparison))
            {
                //Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(name, stringComparison);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        /// <summary>
        /// �ݹ����������Ӧ������������
        /// </summary>
        /// <param name="tfParent">��ǰTransform</param>
        /// <param name="predicate">����</param>
        /// <returns></returns>
        public static Transform FindChildRecursion(this Transform tfParent, Func<Transform, bool> predicate)
        {
            if (predicate(tfParent))
            {
                Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(predicate);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }
    }

    public static class MaterialExtensions
    {
        /// <summary>
        /// �ο�����: https://blog.csdn.net/qiminixi/article/details/78402505
        /// </summary>
        /// <param name="self"></param>
        public static void SetStandardMaterialToTransparentMode(this Material self)
        {
            self.SetFloat("_Mode", 3);
            self.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            self.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            self.SetInt("_ZWrite", 0);
            self.DisableKeyword("_ALPHATEST_ON");
            self.EnableKeyword("_ALPHABLEND_ON");
            self.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            self.renderQueue = 3000;
        }
    }

}
