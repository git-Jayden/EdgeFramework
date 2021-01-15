using EdgeFramework;
using EdgeFramework.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 程序启动
/// </summary>
public class ProcedureLaunch : ProcedureBase
{
    public ProcedureLaunch(ProcedureFSM _fsm)
      : base(_fsm, StateDefine.PROCEDURE_LAUNCH)
    {

    }
    public override void OnEnter(object[] param)
    {
        // 语言配置：设置当前使用的语言 初始化语言
        InitLanguageSettings();
        // 根据使用的语言，通知底层加载对应的资源
        InitCurrentVariant();
        // 声音配置：根据用户配置数据，设置即将使用的声音选项
        InitSoundSettings();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate(float step)
    {
        // TODO: 这里可以播放一个 Splash 动画

        //运行一帧即切换到 check update 流程
        GetFSM().ChangeState(StateDefine.PROCEDURE_CHECK_UPDATE);


        
    }
    private void InitLanguageSettings()
    {
        //TODO后面在考虑
        LanguageHelper.Instance.CurrentLanguage = Language.ChineseSimplified;

    }
    private void InitCurrentVariant()
    {
        //TODO后面在考虑
   
        string currentVariant = null;
        switch (LanguageHelper.Instance.CurrentLanguage)
        {
            case Language.English:
                currentVariant = "en-us";
                break;

            case Language.ChineseSimplified:
                currentVariant = "zh-cn";
                break;

            case Language.ChineseTraditional:
                currentVariant = "zh-tw";
                break;

            case Language.Korean:
                currentVariant = "ko-kr";
                break;

            default:
                currentVariant = "zh-cn";
                break;
        }
        LanguageHelper.Instance.SetCurrentLanguage(currentVariant);
        Debug.Log("Init current Language complete.");
    }
    private void InitSoundSettings()
    {
        AudioManager.Instance.IsMusicOn = true;
        AudioManager.Instance.MusicVolume = 0.3f;
        AudioManager.Instance.IsSoundOn = true;
        AudioManager.Instance.SoundVolume = 1f;
        AudioManager.Instance.SaveAllPreferences();
        Debug.Log("Init sound settings complete.");
    }
}
