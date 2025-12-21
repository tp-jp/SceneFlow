using System.Diagnostics;
using TpLab.SceneFlow.Editor.Bootstraps;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TpLab.SceneFlow.Editor.Internals
{
    /// <summary>
    /// SceneFlow内部用の簡易ロガー
    /// </summary>
    internal static class Logger
    {
        const string Prefix = "[<color=#4EC9B0>SceneFlow</color>]";

        /// <summary>
        /// ログを出力
        /// </summary>
        [Conditional(SceneFlowSymbols.EnableLogging)]
        public static void Log(object message)
        {
            Debug.Log($"{Prefix} {message}");
        }
        
        /// <summary>
        /// ログを出力
        /// </summary>
        [Conditional(SceneFlowSymbols.EnableLogging)]
        public static void Log(object message, Object context)
        {
            Debug.Log($"{Prefix} {message}", context);
        }
        
        /// <summary>
        /// 警告ログを出力
        /// </summary>
        public static void LogWarning(object message)
        {
            Debug.LogWarning($"{Prefix} {message}");
        }
        
        /// <summary>
        /// 警告ログを出力
        /// </summary>
        public static void LogWarning(object message, Object context)
        {
            Debug.LogWarning($"{Prefix} {message}", context);
        }
        
        /// <summary>
        /// エラーログを出力
        /// </summary>
        public static void LogError(object message)
        {
            Debug.LogError($"{Prefix} {message}");
        }
        
        /// <summary>
        /// エラーログを出力
        /// </summary>
        public static void LogError(object message, Object context)
        {
            Debug.LogError($"{Prefix} {message}", context);
        }
    }
}

