using System.Linq;
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
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (defines.Contains(SceneFlowSymbols.SceneFlow))
            {
                var newDefines = defines
                    .Split(';')
                    .Where(d => d != SceneFlowSymbols.SceneFlow)
                    .ToArray();

                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    targetGroup,
                    string.Join(";", newDefines)
                );

                Logger.Log("SceneFlow scripting define symbol has been removed.");
            }
        }
    }
}