using Ls_Mobile;

public class LanguageStr
{
    public const string English = "English";
    public const string Chineses = "Chineses";
    public const string Indonesian = "Indonesian";
}
public class GameOption
{
    private static int bgm = 100;
    private static int sfx = 100;
    private static int loginway = 0;
    private static string logindata = "English";
    private static string lan = "English";
    public static int Bgm
    {
        get
        {
            if (UnityAPIEx.HasKey("Bgm"))
            {
                bgm = UnityAPIEx.GetInt("Bgm");
            }
            else
            {
                bgm = 100;
                UnityAPIEx.SetInt("Bgm", bgm);
            }
            return bgm;
        }
        set
        {
            bgm = value;
            UnityAPIEx.SetInt("Bgm", bgm);
        }
    }
    public static int Sfx
    {
        get
        {
            if (UnityAPIEx.HasKey("Sfx"))
            {
                sfx = UnityAPIEx.GetInt("Sfx");
            }
            else
            {
                sfx = 100;
                UnityAPIEx.SetInt("Sfx", sfx);
            }
            return sfx;
        }
        set
        {
            sfx = value;
            UnityAPIEx.SetInt("Sfx", sfx);
        }
    }
    public static string Lan
    {
        get
        {
            if (UnityAPIEx.HasKey("Lan"))
            {
                lan = UnityAPIEx.GetString("Lan");
            }
            else
            {
                lan = "English";
                UnityAPIEx.SetString("Lan", lan);
            }
            return lan;
        }
        set
        {
            lan = value;
            UnityAPIEx.SetString("Lan", lan);
        }
    }
    public static string LoginData
    {
        get
        {
            if (UnityAPIEx.HasKey("LoginData"))
            {
                logindata = UnityAPIEx.GetString("LoginData");
            }
            else
            {
                logindata = "";
                UnityAPIEx.SetString("LoginData", logindata);
            }
            return logindata;
        }
        set
        {
            logindata = value;
            UnityAPIEx.SetString("LoginData", logindata);
        }
    }
    public static int LoginWay
    {
        get
        {
            if (UnityAPIEx.HasKey("LoginWay"))
            {
                loginway = UnityAPIEx.GetInt("LoginWay");
            }
            else
            {
                loginway = 0;
                UnityAPIEx.SetInt("LoginWay", loginway);
            }
            return loginway;
        }
        set
        {
            loginway = value;
            UnityAPIEx.SetInt("LoginWay", loginway);
        }
    }
}
