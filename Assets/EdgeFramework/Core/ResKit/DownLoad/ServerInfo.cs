/****************************************************
	文件：ServerInfo.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:57   	
	Features：
*****************************************************/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EdgeFramework.Res
{
    [System.Serializable]
    public class ServerInfo 
    {
        [XmlElement("GameVersion")]
        public VersionInfo[] GameVersion;
    }
    //当前游戏版本对应的所有补丁
    [System.Serializable]
    public class VersionInfo
    {
        [XmlAttribute]
        public string Version;
        [XmlElement]
        public Pathces[] Pathces;
    }
    //一个总补丁包
    [System.Serializable]
    public class Pathces
    {
        [XmlAttribute]
        public int Version;

        [XmlAttribute]
        public string Des;//描述

        [XmlElement]
        public List<Patch> Files;
    }
    /// <summary>
    /// 单个补丁包
    /// </summary>
    [System.Serializable]
    public class Patch
    {
        [XmlAttribute]
        public string Name;//ab包名

        [XmlAttribute]
        public string Url;

        [XmlAttribute]
        public string Platform;

        [XmlAttribute]
        public string Md5;

        [XmlAttribute]
        public float Size;
    }
}