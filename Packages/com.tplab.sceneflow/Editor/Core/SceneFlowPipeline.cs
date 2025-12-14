using System;
using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Discovery;
using TpLab.SceneFlow.Editor.Pass;
using TpLab.SceneFlow.Editor.Plugin;
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
            var plugins = SceneFlowPluginDiscovery
                .Discover()
                .Select(p => new SceneFlowPluginNode(p))
                .ToList();

            var nodes = BuildPassNodes(plugins);

            foreach (SceneFlowPhase phase in Enum.GetValues(typeof(SceneFlowPhase)))
            {
                RunPhase(
                    nodes.Where(n => n.Phase == phase).ToList(),
                    scene,
                    report);
            }
        }

        static List<SceneFlowPassNode> BuildPassNodes(IReadOnlyList<SceneFlowPluginNode> plugins)
        {
            var nodes = plugins
                .SelectMany(p => p.Passes.Select(pass => new SceneFlowPassNode(pass)))
                .ToList();

            var nodeByPlugin = plugins
                .SelectMany(p => p.Passes.Select(pass => (p.Plugin.PluginId, pass.Id)))
                .ToLookup(x => x.PluginId, x => x.Id);

            var nodeById = nodes.ToDictionary(n => n.Id);

            foreach (var plugin in plugins)
            {
                foreach (var after in plugin.Plugin.RunAfterPlugins ?? Enumerable.Empty<string>())
                {
                    foreach (var passId in nodeByPlugin[after])
                    foreach (var pass in plugin.Passes)
                        nodeById[pass.Id].RunAfter.Add(passId);
                }

                foreach (var before in plugin.Plugin.RunBeforePlugins ?? Enumerable.Empty<string>())
                {
                    foreach (var passId in nodeByPlugin[before])
                    foreach (var pass in plugin.Passes)
                        nodeById[pass.Id].RunBefore.Add(passId);
                }
            }

            return nodes;
        }
        
        static void RunPhase(
            IReadOnlyList<SceneFlowPassNode> nodes,
            Scene scene,
            BuildReport report)
        {
            if (nodes.Count == 0) return;

            var graph = new SceneFlowGraph(nodes);
            var ordered = graph.Sort();

            foreach (var node in ordered)
                node.Pass.Execute(scene, report);
        }
    }
}