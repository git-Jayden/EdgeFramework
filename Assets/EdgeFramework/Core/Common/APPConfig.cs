﻿/**
 * 配置定义
 */
namespace EdgeFramework
{
    public static class AppConfig
    {
        /// <summary>
        /// 编辑环境下才可以选择是否用AssetBundle进行资源控制
        /// 正式环境一律用AssetBundle
        /// </summary>
        public static bool UseAssetBundle = false;
        /// <summary>
        /// 是否检测版本更新
        /// </summary>
        public static bool CheckVersionUpdate = false;
        /// <summary>
        /// 服务器资源下载地址
        /// </summary>
        public static readonly string ServerResourceURL = "http://127.0.0.1:8000/";
        /// <summary>
        /// Http服务器IP地址
        /// </summary>
        public static readonly string HTTPServerIP = "http://127.0.0.1";
        /// <summary>
        /// 服务器host
        /// </summary>
        public static readonly string ServerHost = "127.0.0.1";
        /// <summary>
        /// 服务器port
        /// </summary>
        public static readonly int ServerPort = 3000;
    }
}