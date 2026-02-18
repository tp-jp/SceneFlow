using System;
using System.Linq;
using TpLab.SceneFlow.Editor.Internals;
using TpLab.SceneFlow.Editor.Passes;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.Cores
{
    /// <summary>
    /// SceneFlow パイプライン
    /// Build-time orchestration フレームワーク
    /// 
    /// ■ 実行順序
    /// - すべての IPass を検出
    /// - Dependencies の依存関係に基づいてソート
    /// - ソート済みの順序で実行
    /// </summary>
    public static class SceneFlowPipeline
    {
        /// <summary>
        /// Pass ベースのパイプラインを実行する
        /// </summary>
        /// <param name="scene">シーン</param>
        public static void Run(Scene scene)
        {
            Logger.Log($"SceneFlow Pipeline started for scene: {scene.name}");

            try
            {
                var context = new SceneFlowContext(scene);

                // すべての IPass を検出してソート実行
                var passList = PassDiscovery.DiscoverPasses<PassBase>().ToList();
                
                if (passList.Count == 0)
                {
                    Logger.Log("No passes found");
                    return;
                }

                Logger.Log($"Executing {passList.Count} pass(es)");

                var sorted = PassSorter.Sort(passList);

                foreach (var pass in sorted)
                {
                    try
                    {
                        Logger.Log($"  → {pass.GetType().Name}");
                        pass.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error in pass {pass.GetType().Name}: {ex}");
                        throw;
                    }
                }

                Logger.Log("SceneFlow Pipeline completed");
            }
            catch (Exception ex)
            {
                Logger.LogError($"SceneFlow Pipeline failed: {ex}");
                throw;
            }
        }
    }
}

