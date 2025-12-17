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
            Logger.Log("SceneFlow has been initialized.");
            
            // Pass の事前検証
            ValidatePasses();
        }

        static void ValidatePasses()
        {
            var buildPasses = PassDiscovery.DiscoverBuildPasses().ToList();
            var projectPasses = PassDiscovery.DiscoverProjectPasses().ToList();
            var scenePasses = PassDiscovery.DiscoverScenePasses().ToList();
            
            var total = buildPasses.Count + projectPasses.Count + scenePasses.Count;
            
            Logger.Log($"Total {total} pass(es) discovered:");
            Logger.Log($"  - BuildPass: {buildPasses.Count}");
            Logger.Log($"  - ProjectPass: {projectPasses.Count}");
            Logger.Log($"  - ScenePass: {scenePasses.Count}");
            
            foreach (var pass in buildPasses)
            {
                Logger.LogDebug($"  [Build] {pass.GetType().Name}");
            }
            foreach (var pass in projectPasses)
            {
                Logger.LogDebug($"  [Project] {pass.GetType().Name}");
            }
            foreach (var pass in scenePasses)
            {
                Logger.LogDebug($"  [Scene] {pass.GetType().Name}");
            }
        }
    }
}

