using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace EdgeFramework

{
    public static class SerializeHelper
    {
        /// <summary>
        /// 类转换成二进制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SerializeBinary(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("SerializeBinary Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Debug.LogWarning("SerializeBinary obj is Null.");
                return false;
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, obj);
                return true;
            }
        }
        /// <summary>
        /// 读取二进制
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object DeserializeBinary(Stream stream)
        {
            if (stream == null)
            {
                Debug.LogWarning("DeserializeBinary Failed!");
                return null;
            }

            using (stream)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var data = bf.Deserialize(stream);

                // TODO:这里没风险嘛?
                return data;
            }
        }

        public static object DeserializeBinary(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("DeserializeBinary Without Valid Path.");
                return null;
            }

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                Debug.LogWarning("DeserializeBinary File Not Exit.");
                return null;
            }

            using (FileStream fs = fileInfo.OpenRead())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                object data = bf.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }

            Debug.LogWarning("DeserializeBinary Failed:" + path);
            return null;
        }
        /// <summary>
        /// 类序列化成xml
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SerializeXML(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("SerializeBinary Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Debug.LogWarning("SerializeBinary obj is Null.");
                return false;
            }

            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                var xmlserializer = new XmlSerializer(obj.GetType());
                xmlserializer.Serialize(fs, obj);
                return true;
            }
        }
        /// <summary>
        /// Xml的反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object DeserializeXML<T>(string path) 
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("DeserializeBinary Without Valid Path.");
                return null;
            }

            FileInfo fileInfo = new FileInfo(path);

            using (FileStream fs = fileInfo.OpenRead())
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                object data = xmlserializer.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }

            Debug.LogWarning("DeserializeBinary Failed:" + path);
            return null;
        }
        /// <summary>
        /// Xml的反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object DeserializeXML(string path, System.Type type)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("DeserializeBinary Without Valid Path.");
                return null;
            }

            FileInfo fileInfo = new FileInfo(path);

            using (FileStream fs = fileInfo.OpenRead())
            {
                XmlSerializer xmlserializer = new XmlSerializer(type);
                object data = xmlserializer.Deserialize(fs);

                if (data != null)
                {
                    return data;
                }
            }

            Debug.LogWarning("DeserializeBinary Failed:" + path);
            return null;
        }
        /// <summary>
        /// 运行时读取xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T XmlDeSerializeRun<T>(string path) where T : class
        {
            T t = default(T);
            //TextAsset textAsset = ResourcesManager.Instance.LoadResouce<TextAsset>(path);
            //if (textAsset == null)
            //{
            //    UnityEngine.Debug.LogError("cant load TextAsset:" + path);
            //    return null;
            //}
            //try
            //{
            //    using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            //    {
            //        XmlSerializer xs = new XmlSerializer(typeof(T));
            //        t = (T)xs.Deserialize(stream);
            //    }
            //    ResouceManager.Instance.ReleaseResouce(path, true);
            //}
            //catch (System.Exception e)
            //{
            //    Debug.LogError("load TextAsset exception:" + path + "," + e);

            //}
            return t;
        }
        public static string ToJson<T>(this T obj) where T : class
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T FromJson<T>(this string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SaveJson<T>(this T obj, string path) where T : class
        {
            var jsonContent = obj.ToJson();
            File.WriteAllText(path, jsonContent);
            return jsonContent;
        }

        public static T LoadJson<T>(string path) where T : class
        {
            return File.ReadAllText(path).FromJson<T>();
        }

        //public static byte[] ToProtoBuff<T>(this T obj) where T : class
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        ProtoBuf.Serializer.Serialize<T>(ms, obj);
        //        return ms.ToArray();
        //    }
        //}

        //public static T FromProtoBuff<T>(this byte[] bytes) where T : class
        //{
        //    if (bytes == null || bytes.Length == 0)
        //    {
        //        throw new System.ArgumentNullException("bytes");
        //    }
        //    T t = ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(bytes));
        //    return t;
        //}

        //public static void SaveProtoBuff<T>(this T obj, string path) where T : class
        //{
        //    File.WriteAllBytes(path, obj.ToProtoBuff<T>());
        //}

        //public static T LoadProtoBuff<T>(string path) where T : class
        //{
        //    return File.ReadAllBytes(path).FromProtoBuff<T>();
        //}
    }
}
