
/****************************************************
	文件：GamePlayerPrefs.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:29   	
	Features：
*****************************************************/
using UnityEngine;

public class GamePlayerPrefs 
{
    private static string sLanguage = "English";
    public static string Language
    {
        get
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                sLanguage = PlayerPrefs.GetString("Language");
            }
            else
            {
                sLanguage = "English";
                PlayerPrefs.SetString("Language", sLanguage);
            }
            return sLanguage;
        }
        set
        {
            sLanguage = value;
            PlayerPrefs.SetString("Language", sLanguage);
        }
    }

}
