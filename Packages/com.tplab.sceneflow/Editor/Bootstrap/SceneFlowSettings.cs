using UnityEditor;

namespace TpLab.SceneFlow.Editor.Bootstrap
{
    public static class SceneFlowSettings
    {
        const string MenuPath = "Tools/SceneFlow/Enable Logging";

        [MenuItem(MenuPath, false)]
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
            DefineSymbolUtility.SetSymbolForAllPlatforms(SceneFlowSymbols.EnableLogging, enable);
        }

        static bool IsLoggingEnabled()
        {
            return DefineSymbolUtility.IsSymbolDefined(SceneFlowSymbols.EnableLogging);
        }
    }
}