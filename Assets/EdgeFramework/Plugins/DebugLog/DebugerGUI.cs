/****************************************************
	文件：DebugerGUI.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/03/11 11:22   	
	Features：
*****************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace EdgeFramework.Debugger
{
    public class DebugerGUI : MonoBehaviour
    {
        /// <summary>
        /// 显示和隐藏控制台窗口的热键。
        /// </summary>
        [Header("显示和隐藏控制台窗口的热键")]
        public KeyCode toggleKey = KeyCode.F1;

        /// <summary>  
        /// 在打印的时候是否开启堆栈打印
        /// </summary>  
        [Header("是否开启堆栈打印")]
        public bool StackLog = false;

        ///  <summary>  
        /// 是否保留一定数量的日志
        ///  </summary>  
        [Header("是否保留一定数量的日志")]
        public bool restrictLogCount = true;

        /// <summary>  
        /// 是否通过摇动设备(仅移动设备)来打开窗户
        /// </summary>  
        [Header("是否通过摇动设备(仅移动设备)来打开窗户")]
        public bool shakeToOpen = true;

        /// <summary>  
        /// 显示字体大小
        /// </summary>  
        [Header("显示字体大小")]
        public float FontSize = 30f;

        /// <summary>  
        /// 显示拖动条宽度
        /// </summary>  
        [Header("显示拖动条宽度")]
        public float ScrollbarSize = 40f;

        /// <summary>  
        /// (平方)在上面的加速度，窗口应该打开
        /// </summary>  
        [Header("(平方)在上面的加速度，窗口应该打开")]
        public float shakeAcceleration = 60f;

        /// <summary>  
        /// 在删除旧的日志之前保持日志的数量。 
        /// </summary>  
        [Header("在删除旧的日志之前保持日志的数量")]
        public int maxLogs = 1000;

        /// <summary>
        /// 对应横向、纵向滑动条对应的X,Y数值
        /// </summary>
        [Header("对应横向、纵向滑动条对应的X,Y数值")]
        public Vector2 scrollPosition;
        /// <summary>
        /// Debug按钮的Rect
        /// </summary>
        [Header("Debug按钮的Rect")]
        public Rect DebugRect = new Rect(200, 5, 300, 60);

        private readonly List<DebugerGUI.Log> logs = new List<DebugerGUI.Log>();

        /// <summary>
        /// 可见
        /// </summary>
        public static bool visible;

        /// <summary>
        /// 折叠
        /// </summary>
        private bool collapse;

        private readonly static Dictionary<LogType, Color> logTypeColors;

        private readonly string windowTitle = string.Concat("<size=", 25, ">Debug（打印日志</size>");

        private const int margin = 20;

        private readonly static GUIContent clearLabel;

        private readonly static GUIContent stackLabel;

        private readonly static GUIContent colseLabel;

        private readonly static GUIContent collapseLabel;

        private readonly Rect titleBarRect = new Rect(0f, 0f, 10000f, 40f);

        private Rect windowRect = new Rect(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 40));


        static DebugerGUI()
        {
            DebugerGUI.logTypeColors = new Dictionary<LogType, Color>()
            {
                { (LogType)1, Color.white},
                { (LogType)0, Color.red },
                { (LogType)4, Color.magenta },
                { (LogType)3, Color.green },
                { (LogType)2, Color.yellow }
            };
            DebugerGUI.clearLabel = new GUIContent(string.Concat("<size=", 25, ">Clear</size>"), "清空打印信息.");
            DebugerGUI.stackLabel = new GUIContent(string.Concat("<size=", 25, ">Stack开关</size>"), "开启堆栈追踪.");
            DebugerGUI.colseLabel = new GUIContent(string.Concat("<size=", 25, ">Close</size>"), "关闭打印面板.");
            DebugerGUI.collapseLabel = new GUIContent(string.Concat("<size=", 25, ">Collapse</size>"), "隐藏重复信息.");
        }

        public DebugerGUI()
        {
        }

        /// <summary>  
        /// 显示一个列出已记录日志的窗口。
        /// </summary>  
        /// <param name="windowID">Window ID.</param>  
        private void DrawConsoleWindow(int windowID)
        {
            this.DrawLogsList();
            this.DrawToolbar();
        }

        /// <summary>  
        /// 绘制log列表
        /// </summary>  
        private void DrawLogsList()
        {
            GUIStyle gs_vertica = GUI.skin.verticalScrollbar;
            GUIStyle gs1_vertica = GUI.skin.verticalScrollbarThumb;
            gs_vertica.fixedWidth = this.ScrollbarSize;
            gs1_vertica.fixedWidth = this.ScrollbarSize;
            GUIStyle gs_horizontal = GUI.skin.horizontalScrollbar;
            GUIStyle gs1_horizontal = GUI.skin.horizontalScrollbarThumb;
            gs_horizontal.fixedHeight = this.ScrollbarSize;
            gs1_horizontal.fixedHeight = this.ScrollbarSize;
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true, Array.Empty<GUILayoutOption>());
            for (int i = 0; i < this.logs.Count; i++)
            {
                DebugerGUI.Log log = this.logs[i];
                if ((!this.collapse ? false : i > 0))
                {
                    string previousMessage = this.logs[i - 1].message;
                    if (log.message == previousMessage)
                    {
                        goto Label0;
                    }
                }
                GUI.contentColor = DebugerGUI.logTypeColors[log.type];
                GUILayout.Label(log.message, Array.Empty<GUILayoutOption>());
                if (this.StackLog)
                {
                    GUILayout.Label(log.stackTrace, Array.Empty<GUILayoutOption>());
                }
            Label0:;
            }
            GUI.color = Color.magenta;
            GUILayout.EndScrollView();
            gs_vertica.fixedWidth = 0f;
            gs1_vertica.fixedWidth = 0f;
            gs_horizontal.fixedHeight = 0f;
            gs1_horizontal.fixedHeight = 0f;
            GUI.contentColor = Color.white;
        }

        /// <summary>  
        /// Log日志工具栏
        /// </summary>  
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            if (GUILayout.Button(DebugerGUI.clearLabel, new GUILayoutOption[] { GUILayout.Height(50f) }))
            {
                this.logs.Clear();
            }
            if (GUILayout.Button(DebugerGUI.stackLabel, new GUILayoutOption[] { GUILayout.Height(50f) }))
            {
                this.StackLog = !this.StackLog;
            }
            if (GUILayout.Button(DebugerGUI.colseLabel, new GUILayoutOption[] { GUILayout.Height(50f) }))
            {
                DebugerGUI.visible = false;
            }
            this.collapse = GUILayout.Toggle(this.collapse, DebugerGUI.collapseLabel, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(60f) });
            GUILayout.EndHorizontal();
        }

        /// <summary>  
        /// Debug 对应的回调处理
        /// </summary>  
        /// <param name="message">信息.</param>  
        /// <param name="stackTrace">信息的来源</param>  
        /// <param name="type">信息类型 (error, exception, warning, assert).</param>  
        private void HandleLog(string message, string stackTrace, LogType type)
        {
            List<DebugerGUI.Log> logs = this.logs;
            DebugerGUI.Log log = new DebugerGUI.Log()
            {
                message = string.Concat(new object[] { "<size=", this.FontSize, ">", message, "</size>" }),
                stackTrace = string.Concat(new object[] { "<size=", this.FontSize, ">", stackTrace, "</size>" }),
                type = type
            };
            logs.Add(log);
            this.TrimExcessLogs();
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= new Application.LogCallback(HandleLog);
        }

        private void OnEnable()
        {
            Application.logMessageReceived += new Application.LogCallback(HandleLog);
        }

        private void OnGUI()
        {

            if (GUI.Button(DebugRect, new GUIContent(string.Concat("<size=", 30, ">FPS:</size>" + "<size=", 30, ">", Math.Round(m_FPS, 2), "</size>")))/*GUILayout.Button(string.Concat("<size=", 30, ">     Debug     </size>"),new GUIStyle() , new GUILayoutOption[] { GUILayout.Height(60f) })*/)
            {
                DebugerGUI.visible = true;
            }
            if (DebugerGUI.visible)
            {
                this.windowRect = GUILayout.Window(666, this.windowRect, new GUI.WindowFunction(DrawConsoleWindow), this.windowTitle, Array.Empty<GUILayoutOption>());
            }
        }

        [Conditional("EnableLog")]
        public void Running()
        {
            if (Input.GetKeyDown(this.toggleKey))
            {
                DebugerGUI.visible = !DebugerGUI.visible;
            }
            if ((!this.shakeToOpen || Input.acceleration.sqrMagnitude <= this.shakeAcceleration ? Input.touchCount >= 6 : true))
            {
                DebugerGUI.visible = true;
            }
            if (Input.touchCount >= 3)
            {
                DebugerGUI.visible = true;
            }
        }

        /// <summary>  
        /// 删除超过允许的最大数量的旧日志。
        /// </summary>  
        private void TrimExcessLogs()
        {
            if (this.restrictLogCount)
            {
                int amountToRemove = Mathf.Max(this.logs.Count - this.maxLogs, 0);
                if (amountToRemove != 0)
                {
                    this.logs.RemoveRange(0, amountToRemove);
                }
            }
        }
        public float fpsMeasuringDelta = 2.0f;

        private float timePassed;
        private int m_FrameCount = 0;
        private float m_FPS = 0.0f;
        private void Start()
        {
            timePassed = 0.0f;
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            this.Running();
            FPSUpdate();
        }
        private void FPSUpdate()
        {
            m_FrameCount = m_FrameCount + 1;
            timePassed = timePassed + Time.deltaTime;

            if (timePassed > fpsMeasuringDelta)
            {
                m_FPS = m_FrameCount / timePassed;

                timePassed = 0.0f;
                m_FrameCount = 0;
            }
        }
        private struct Log
        {
            public string message;

            public string stackTrace;

            public LogType type;
        }
    }
}