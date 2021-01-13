//定义状态
public class StateDefine
{
    public const string NONE = "none";
    public const string PROCEDURE_LAUNCH = "procedure_lanuch";//程序启动
    public const string PROCEDURE_SPLASH = "procedure_splash";//开机动画
    public const string PROCEDURE_CHECK_UPDATE = "procedure_check_update";//检查更新
    public const string PROCEDURE_LOADING_SCENE = "procedure_loading_scene";//加载场景
    public const string PROCEDURE_MENU = "state_choose_actor";//主界面 菜单界面

    public const string STATE_CHOOSE_ACTOR = "state_choose_actor";//选择角色状态
    public const string STATE_CHOOSE_CLASS = "state_choose_class";//选择课程状态
    public const string STATE_CLASS_CONTENT = "state_class_content";//班级内容状态
    public const string STATE_GAME_PLAY = "state_test_game";//战斗场景状态
    public const string STATE_END_GAME = "state_end_game";//结束游戏状态
}
