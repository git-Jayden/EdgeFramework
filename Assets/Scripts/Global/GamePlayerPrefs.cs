

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
