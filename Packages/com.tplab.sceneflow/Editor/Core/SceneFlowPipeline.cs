using System;
using System.Linq;
using TpLab.SceneFlow.Editor.Internal;
using TpLab.SceneFlow.Editor.Pass;
using UnityEditor.Build.Reporting;
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
                ExecutePasses<IBuildPass>("BuildPass", context);

                // 2. IProjectPass: プロジェクト全体に対する処理
                ExecutePasses<IProjectPass>("ProjectPass", context);

                // 3. IScenePass: シーン単位の処理
                ExecutePasses<IScenePass>("ScenePass", context, $"for scene: {scene.name}");

                Logger.Log("SceneFlow Pipeline completed");
            }
            catch (Exception ex)
            {
                Logger.LogError($"SceneFlow Pipeline failed: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Pass を実行する
        /// </summary>
        /// <typeparam name="T">Pass の型</typeparam>
        /// <param name="passTypeName">Pass の種類名（ログ用）</param>
        /// <param name="context">実行コンテキスト</param>
        /// <param name="additionalInfo">追加情報（ログ用）</param>
        static void ExecutePasses<T>(string passTypeName, SceneFlowContext context, string additionalInfo = null)
            where T : IPass
        {
            var passList = PassDiscovery.DiscoverPasses<T>().ToList();
            if (passList.Count == 0) return;

            var logMessage = $"Executing {passList.Count} {passTypeName}(es)";
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                logMessage += $" {additionalInfo}";
            }
            Logger.Log(logMessage);

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
                    Logger.LogError($"Error in {passTypeName} {pass.GetType().Name}: {ex}");
                    throw;
                }
            }
        }
    }
}