using System;
using System.Linq;
using TpLab.SceneFlow.Editor.Internal;
using TpLab.SceneFlow.Editor.Pass;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// SceneFlow パイプライン
    /// Build-time orchestration フレームワーク
    /// 実行順序: IBuildPass → IProjectPass → IScenePass
    /// </summary>
    public static class SceneFlowPipeline
    {
        /// <summary>
        /// Pass ベースのパイプラインを実行する
        /// </summary>
        /// <param name="scene">シーン</param>
        public static void Run(Scene scene)
        {
            Logger.Log("SceneFlow Pipeline started");

            try
            {
                var context = new SceneFlowContext(scene);

                // 1. IBuildPass: ビルド全体で一度だけ実行
                ExecuteBuildPasses(context);

                // 2. IProjectPass: プロジェクト全体に対する処理
                ExecuteProjectPasses(context);

                // 3. IScenePass: シーン単位の処理
                ExecuteScenePasses(context);

                Logger.Log("SceneFlow Pipeline completed");
            }
            catch (Exception ex)
            {
                Logger.LogError($"SceneFlow Pipeline failed: {ex}");
                throw;
            }
        }

        /// <summary>
        /// IBuildPass を実行
        /// </summary>
        static void ExecuteBuildPasses(SceneFlowContext context)
        {
            var passes = PassDiscovery.DiscoverBuildPasses().ToList();
            if (passes.Count == 0) return;

            Logger.Log($"Executing {passes.Count} BuildPass(es)");

            var sorted = PassSorter.Sort(
                passes,
                p => p.RunAfter,
                p => p.RunBefore);

            foreach (var pass in sorted)
            {
                try
                {
                    Logger.Log($"  → {pass.GetType().Name}");
                    pass.Execute(context);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in BuildPass {pass.GetType().Name}: {ex}");
                    throw;
                }
            }
        }

        /// <summary>
        /// IProjectPass を実行
        /// </summary>
        static void ExecuteProjectPasses(SceneFlowContext context)
        {
            var passes = PassDiscovery.DiscoverProjectPasses().ToList();
            if (passes.Count == 0) return;

            Logger.Log($"Executing {passes.Count} ProjectPass(es)");

            var sorted = PassSorter.Sort(
                passes,
                p => p.RunAfter,
                p => p.RunBefore);

            foreach (var pass in sorted)
            {
                try
                {
                    Logger.Log($"  → {pass.GetType().Name}");
                    pass.Execute(context);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in ProjectPass {pass.GetType().Name}: {ex}");
                    throw;
                }
            }
        }

        /// <summary>
        /// IScenePass を実行
        /// </summary>
        static void ExecuteScenePasses(SceneFlowContext context)
        {
            var passes = PassDiscovery.DiscoverScenePasses().ToList();
            if (passes.Count == 0) return;

            Logger.Log($"Executing {passes.Count} ScenePass(es) for scene: {context.Scene.name}");

            var sorted = PassSorter.Sort(
                passes,
                p => p.RunAfter,
                p => p.RunBefore);

            foreach (var pass in sorted)
            {
                try
                {
                    Logger.Log($"  → {pass.GetType().Name}");
                    pass.Execute(context);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error in ScenePass {pass.GetType().Name}: {ex}");
                    throw;
                }
            }
        }
    }
}