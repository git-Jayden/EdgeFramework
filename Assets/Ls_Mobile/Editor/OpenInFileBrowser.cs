/*******************************************************************
* Copyright(c) 2018 DefaultCompany
* All rights reserved.
*
* 文件名称: OpenInFileBrowser.cs
* 简要描述: http://wiki.unity3d.com/index.php/OpenInFileBrowser
* 
* 创建日期: 2018/12/25 09:52:41
* 作者:    
* 说明:  
******************************************************************/
namespace Ls_Mobile
{
    public static class OpenInFileBrowser
    {
        public static bool IsInMacOS
        {
            get
            {
                return UnityEngine.SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;
            }
        }

        public static bool IsInWinOS
        {
            get
            {
                return UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows") != -1;
            }
        }

        [UnityEditor.MenuItem("Ls_Mobile/OpenInFileBrowser")]
        public static void Test()
        {
            Open(UnityEngine.Application.dataPath);
        }

        public static void OpenInMac(string path)
        {
            bool openInsidesOfFolder = false;

            // try mac
            string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

            if (System.IO.Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }

            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }

            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open mac finder in windows
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        public static void OpenInWin(string path)
        {
            bool openInsidesOfFolder = false;

            // try windows
            string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

            if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open win explorer in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        public static void Open(string path)
        {
            if (IsInWinOS)
            {
                OpenInWin(path);
            }
            else if (IsInMacOS)
            {
                OpenInMac(path);
            }
            else // couldn't determine OS
            {
                OpenInWin(path);
                OpenInMac(path);
            }
        }
    }
}