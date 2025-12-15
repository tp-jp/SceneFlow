using System;
using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Discovery;
using TpLab.SceneFlow.Editor.Internal;
using TpLab.SceneFlow.Editor.Pass;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// シーンフローパイプライン
    /// </summary>
    public static class SceneFlowPipeline
    {
        /// <summary>
        /// シーン処理パイプラインを実行する。
        /// </summary>
        /// <param name="scene">シーン</param>
        /// <param name="report">ビルドレポート</param>
        public static void Run(Scene scene, BuildReport report)
        {
            var context = new SceneFlowContext(scene, report);
            
            // ジョブを発見
            var jobs = SceneFlowJobDiscovery
                .Discover()
                .ToList();

            // 各ジョブからステップを収集
            var allSteps = new List<SceneFlowStep>();
            foreach (var job in jobs)
            {
                var jobBuilder = new SceneFlowJobBuilder();
                job.Configure(jobBuilder);
                allSteps.AddRange(jobBuilder.Build());
            }

            // ジョブIDをステップIDに展開
            ExpandJobDependenciesToSteps(allSteps);

            // フェーズごとにステップを実行
            ExecuteStepsByPhase(allSteps, context);
        }

        static void ExpandJobDependenciesToSteps(List<SceneFlowStep> steps)
        {
            // ジョブIDからステップIDへのマッピングを作成
            var jobToSteps = new Dictionary<string, List<string>>();
            
            foreach (var step in steps)
            {
                // ステップIDからジョブIDを抽出（例: "MyJob.Step1" -> "MyJob"）
                var lastDotIndex = step.Id.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    var jobId = step.Id.Substring(0, lastDotIndex);
                    if (!jobToSteps.ContainsKey(jobId))
                    {
                        jobToSteps[jobId] = new List<string>();
                    }
                    jobToSteps[jobId].Add(step.Id);
                }
            }

            // 各ステップのジョブID依存をステップID依存に展開
            foreach (var step in steps)
            {
                // RunAfterJobs -> RunAfterSteps
                foreach (var jobId in step.RunAfterJobs)
                {
                    if (jobToSteps.TryGetValue(jobId, out var jobSteps))
                    {
                        step.RunAfterSteps.UnionWith(jobSteps);
                    }
                }

                // RunBeforeJobs -> RunBeforeSteps
                foreach (var jobId in step.RunBeforeJobs)
                {
                    if (jobToSteps.TryGetValue(jobId, out var jobSteps))
                    {
                        step.RunBeforeSteps.UnionWith(jobSteps);
                    }
                }
            }
        }
        static void ExecuteStepsByPhase(
            List<SceneFlowStep> allSteps,
            SceneFlowContext context)
        {
            // フェーズごとにグループ化
            var stepsByPhase = allSteps
                .GroupBy(s => s.Phase)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var phaseGroup in stepsByPhase)
            {
                var phaseSteps = phaseGroup.ToList();
                
                // このフェーズのステップをトポロジカルソート
                var stepNodes = phaseSteps.Select(s => new SceneFlowStepNode(s)).ToList();
                var graph = new SceneFlowGraph<SceneFlowStepNode>(stepNodes);
                var orderedSteps = graph.Sort();

                // ステップを実行
                foreach (var stepNode in orderedSteps)
                {
                    try
                    {
                        stepNode.Step.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error in step {stepNode.Id}: {ex}");
                    }
                }
            }
        }
    }
}