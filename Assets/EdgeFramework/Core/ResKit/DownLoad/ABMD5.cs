/****************************************************
	文件：ABMD5.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:56   	
	Features：
*****************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace EdgeFramework.Res
{
    [System.Serializable]
    public class ABMD5
    {
        [XmlElement("ABMD5List")]
        public List<ABMD5Base> ABMD5List { get; set; }
    }

    [System.Serializable]
    public class ABMD5Base
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Md5")]
        public string Md5 { get; set; }
        [XmlAttribute("Size")]
        public float Size { get; set; }
    }
}