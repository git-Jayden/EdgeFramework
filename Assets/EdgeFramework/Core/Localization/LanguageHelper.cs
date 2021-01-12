using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EdgeFramework
{
    public class LanguageHelper : Singleton<LanguageHelper>
    {
        LanguageHelper() { }
        public Language CurrentLanguage { get; set; }
        private string mLanguageKey;
        public void SetCurrentLanguage(string languageKey)
        {
            mLanguageKey = languageKey;
            //TODO根据key获取
        }
        /// <summary>
        /// 解析数据。
        /// </summary>
        /// <returns></returns>
        public bool ParseData()
        {
            return false;
        }
    }
}
