
/****************************************************
	文件：AssetBundleConfig.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:59   	
	Features：
*****************************************************/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EdgeFramework.Res
{
    [System.Serializable]
    public class AssetBundleConfig
    {
        [XmlElement("ABList")]
        public List<ABBase> abList { get; set; }
    }

    [System.Serializable]
    public class ABBase
    {
        [XmlAttribute("Path")]
        public string path { get; set; }
        [XmlAttribute("Crc")]
        public uint crc { get; set; }
        [XmlAttribute("ABName")]
        public string abName { get; set; }
        [XmlAttribute("AssetName")]
        public string assetName { get; set; }
        [XmlElement("ABDependce")]
        public List<string> abDependce { get; set; }
    }
}