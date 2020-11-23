using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace EdgeFramework
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