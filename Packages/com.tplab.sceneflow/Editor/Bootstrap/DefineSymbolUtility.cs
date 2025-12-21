using System;
using System.Linq;
using UnityEditor;

namespace TpLab.SceneFlow.Editor.Bootstrap
{
    /// <summary>
    /// 定義シンボル操作ユーティリティ。
    /// </summary>
    internal static class DefineSymbolUtility
    {
        /// <summary>
        /// 指定したシンボルが定義されているかどうかを取得する。
        /// </summary>
        /// <param name="symbol">シンボル</param>
        /// <returns>定義されている場合はtrue、それ以外はfalse</returns>
        public static bool IsSymbolDefined(string symbol)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Contains(symbol);
        }

        /// <summary>
        /// 指定したシンボルを全てのプラットフォームで有効化または無効化する。
        /// </summary>
        /// <param name="symbol">シンボル</param>
        /// <param name="enabled">有効状態</param>
        public static void SetSymbolForAllPlatforms(string symbol, bool enabled)
        {
            foreach (BuildTargetGroup targetGroup in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (targetGroup == BuildTargetGroup.Unknown || IsObsolete(targetGroup)) continue;

                try
                {
                    var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                    var symbolList = defines.Split(';').ToList();

                    if (enabled)
                    {
                        if (!symbolList.Contains(symbol))
                        {
                            symbolList.Add(symbol);
                        }
                    }
                    else
                    {
                        symbolList.Remove(symbol);
                    }

                    var newDefines = string.Join(";", symbolList.Where(s => !string.IsNullOrWhiteSpace(s)));
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
                }
                catch (ArgumentException)
                {
                    // 対応していないビルドターゲットグループの場合はスキップ
                }
            }
        }

        /// <summary>
        /// 指定したビルドターゲットグループが廃止されているかどうかを取得する。
        /// </summary>
        /// <param name="group">ビルドターゲットグループ</param>
        /// <returns>廃止されている場合はtrue、それ以外はfalse</returns>
        static bool IsObsolete(BuildTargetGroup group)
        {
            var fieldInfo = typeof(BuildTargetGroup).GetField(group.ToString());
            return fieldInfo?.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length > 0;
        }
    }
}