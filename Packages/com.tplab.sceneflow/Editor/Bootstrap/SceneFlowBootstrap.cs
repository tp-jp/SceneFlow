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
            AddLoggingSymbolOnFirstTime();
            Logger.Log("SceneFlow has been initialized.");
            
            // Pass の事前検証
            ValidatePasses();
        }

        static void AddLoggingSymbolOnFirstTime()
        {
            const string firstInitKey = "SceneFlow_LoggingInitialized";
    
            // 初回起動時のみログシンボルを追加
            if (!EditorPrefs.HasKey(firstInitKey))
            {
                if (!DefineSymbolUtility.IsSymbolDefined(SceneFlowSymbols.EnableLogging))
                {
                    DefineSymbolUtility.SetSymbolForAllPlatforms(SceneFlowSymbols.EnableLogging, true);
                }
                EditorPrefs.SetBool(firstInitKey, true);
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

