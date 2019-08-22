using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile
{
    public static class Constants
    {
        public const string EmptyScene = "Empty";
        public const string MenuScene = "Menu";

        //  Frame Product name
        public const string ProductName = "Ls_Mobile";
        // Menum
        public const string Settings = "Settings";

        // Frame Folder
        public const string FramePath = "Assets/Ls_Mobile";
        public const string ResourcesFolder = FramePath + "/Resources";

        // Assets and stuff
        public const string AbConfigPath = ResourcesFolder + "/AbConfig.asset";
        public const string RealFramConfigPath = ResourcesFolder + "/RealFramConfig.asset";

        //PanelJson
        public const string UIJsonPath = "Assets/Ls_Mobile/Config/UIPanel.json";
        //Soundconfig
        public const string AudioConfig = "Assets/Ls_Mobile/Config/config_sound.txt";

        //ILRuntime dll Path
        public const string DLLPATH = "Assets/Data/Hotfix/HotFix.dll.txt";
        public const string PDBPATH = "Assets/Data/Hotfix/HotFix.pdb.txt";
    }
}