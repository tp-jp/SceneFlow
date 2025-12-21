using System.Linq;
using TpLab.SceneFlow.Editor.Internal;
using TpLab.SceneFlow.Editor.Pass;
using UnityEditor;

namespace TpLab.SceneFlow.Editor.Bootstrap
{
    /// <summary>
    /// SceneFlowの初期化を行うクラス
    /// </summary>
    public static class SceneFlowBootstrap
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            AddScriptingDefineSymbol();
            Logger.Log("SceneFlow has been initialized.");
            
            // Pass の事前検証
            ValidatePasses();
        }

        static void AddScriptingDefineSymbol()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            
            if (!defines.Contains(SceneFlowSymbols.SceneFlow))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    targetGroup, 
                    defines + ";" + SceneFlowSymbols.SceneFlow
                );
            }
        }

        static void ValidatePasses()
        {
            var passes = PassDiscovery.DiscoverPasses<IPass>().ToList();
            
            Logger.Log($"Total {passes.Count} pass(es) discovered:");
            
            foreach (var pass in passes)
            {
                Logger.Log($"  - {pass.GetType().Name}");
            }
        }
    }
}

