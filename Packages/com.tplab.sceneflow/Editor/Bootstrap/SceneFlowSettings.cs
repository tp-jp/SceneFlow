using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TpLab.SceneFlow.Editor.Bootstrap
{
    public static class SceneFlowSettings
    {
        const string MenuPath = "Tools/SceneFlow/Enable Logging";

        [MenuItem(MenuPath, false, 0)]
        static void EnableLogging()
        {
            SetLoggingSymbol(!IsLoggingEnabled());
        }

        [MenuItem(MenuPath, true)]
        static bool ValidateEnableLogging()
        {
            Menu.SetChecked(MenuPath,IsLoggingEnabled());
            return true;
        }

        static void SetLoggingSymbol(bool enable)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defineList = defines.Split(';').ToList();

            if (enable && !defineList.Contains(SceneFlowSymbols.EnableLogging))
            {
                defineList.Add(SceneFlowSymbols.EnableLogging);
                Debug.Log("SceneFlow logging enabled.");
            }
            else if (!enable && defineList.Contains(SceneFlowSymbols.EnableLogging))
            {
                defineList.Remove(SceneFlowSymbols.EnableLogging);
                Debug.Log("SceneFlow logging disabled.");
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                targetGroup,
                string.Join(";", defineList)
            );
        }

        static bool IsLoggingEnabled()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            return defines.Contains(SceneFlowSymbols.EnableLogging);
        }
    }
}