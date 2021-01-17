/****************************************************
	�ļ���ActionGroupAttribute.cs
	Author��JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date��2021/01/15 9:12   	
	Features��
*****************************************************/

using System;

namespace EdgeFramework
{
    public class ActionGroupAttribute : Attribute
    {
        public readonly string GroupName;

        public ActionGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}