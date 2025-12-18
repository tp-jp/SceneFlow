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
            var passes = PassDiscovery.DiscoverPasses<IPass>().ToList();
            
            Logger.Log($"Total {passes.Count} pass(es) discovered:");
            
            foreach (var pass in passes)
            {
                Logger.LogDebug($"  - {pass.GetType().Name}");
            }
        }
    }
}

