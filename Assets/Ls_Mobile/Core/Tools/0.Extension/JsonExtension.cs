﻿
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EdgeFramework
{
    public static class JsonExtension
    {
        /// <summary>
        /// Check if json data contains key specified
        /// </summary>
        public static bool JsonDataContainsKey(this JToken self, string key)
        {
            if (self == null) return false;
            if (!self.IsObject()) return false;
            return self[key] != null;
        }

        public static bool IsNullOrUndefined(this JToken self)
        {
            return self == null || self.Type == JTokenType.Null || self.Type == JTokenType.Undefined || self.Type == JTokenType.None;
        }

        public static bool IsString(this JToken self)
        {
            return self != null && self.Type == JTokenType.String;
        }

        public static bool IsObject(this JToken self)
        {
            return self != null && self.Type == JTokenType.Object;
        }

        public static bool IsArray(this JToken self)
        {
            return self != null && self.Type == JTokenType.Array;
        }

        /// <summary>
        /// 临时的先放在这里
        /// </summary>
        /// <returns>The json value.</returns>
        /// <param name="vector2">Vector2.</param>
        public static string ToJsonValue(this Vector2 vector2)
        {
            return vector2.x + "," + vector2.y;
        }
    }
}