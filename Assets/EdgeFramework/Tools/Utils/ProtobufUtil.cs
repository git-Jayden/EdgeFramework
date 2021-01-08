﻿using ProtoBuf;
using System;
using System.IO;
namespace EdgeFramework.Utils
{
    public class ProtobufUtil
    {
        /// <summary>
        /// 序列化pb数据
        /// </summary>
        public static byte[] NSerialize<T>(T t)
        {
            byte[] buffer = null;
            using (MemoryStream m = new MemoryStream())
            {
                Serializer.Serialize<T>(m, t);
                m.Position = 0;
                int length = (int)m.Length;
                buffer = new byte[length];
                m.Read(buffer, 0, length);
            }
            return buffer;
        }

        /// <summary>
        /// 反序列化pb数据
        /// </summary>
        public static T NDeserialize<T>(byte[] buffer)
        {
            T t = default(T);
            using (MemoryStream m = new MemoryStream(buffer))
            {
                t = Serializer.Deserialize<T>(m);
            }
            return t;
        }

        /// <summary>
        /// 反序列化pb数据
        /// </summary>
        public static object NDeserialize(Type type, byte[] buffer, int index)
        {
            using (MemoryStream m = new MemoryStream(buffer, index, buffer.Length - index))
            {
                return Serializer.Deserialize(type, m);
            }
        }
    }
}