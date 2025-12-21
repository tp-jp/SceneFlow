using TpLab.SceneFlow.Editor.Internal;
using UnityEditor;

namespace TpLab.SceneFlow.Editor.Bootstrap
{
    public class SceneFlowCleanup : AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            // パッケージのルートディレクトリが削除される場合
            if (assetPath.Contains("com.tplab.sceneflow"))
            {
                RemoveScriptingDefineSymbol();
            }

            return AssetDeleteResult.DidNotDelete;
        }

        static void RemoveScriptingDefineSymbol()
        {
            if (DefineSymbolUtility.IsSymbolDefined(SceneFlowSymbols.SceneFlow))
            {
                DefineSymbolUtility.SetSymbolForAllPlatforms(SceneFlowSymbols.SceneFlow, false);
                DefineSymbolUtility.SetSymbolForAllPlatforms(SceneFlowSymbols.EnableLogging, false);

                EditorPrefs.DeleteKey("SceneFlow_LoggingInitialized");

                Logger.Log("SceneFlow scripting define symbol has been removed.");
            }
        }
    }
}