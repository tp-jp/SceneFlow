using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TpLab.SceneFlow.Editor.Internal
{
    /// <summary>
    /// SceneFlow内部用の簡易ロガー
    /// </summary>
    internal static class Logger
    {
        const string Prefix = "[<color=#4EC9B0>SceneFlow</color>]";

        /// <summary>
        /// デバッグログを出力
        /// </summary>
        public static void LogDebug(object message)
        {
            Debug.Log($"{Prefix} [DEBUG] {message}");
        }

        /// <summary>
        /// デバッグログを出力
        /// </summary>
        public static void LogDebug(object message, Object context)
        {
            Debug.Log($"{Prefix} [DEBUG] {message}", context);
        }

        /// <summary>
        /// 情報ログを出力
        /// </summary>
        public static void Log(object message)
        {
            Debug.Log($"{Prefix} {message}");
        }
        
        /// <summary>
        /// 情報ログを出力
        /// </summary>
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

