using System;
using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Pass;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// シーンフローパスの依存関係グラフ
    /// </summary>
    internal sealed class SceneFlowGraph
    {
        readonly Dictionary<string, SceneFlowPassNode> _nodes;
        readonly Dictionary<string, HashSet<string>> _edges = new();

        public SceneFlowGraph(IEnumerable<SceneFlowPassNode> nodes)
        {
            _nodes = nodes.ToDictionary(n => n.Id);
            BuildEdges();
        }

        private void BuildEdges()
        {
            foreach (var node in _nodes.Values)
            {
                if (!_edges.ContainsKey(node.Id))
                    _edges[node.Id] = new();

                foreach (var after in node.RunAfter)
                    _edges[after].Add(node.Id);

                foreach (var before in node.RunBefore)
                    _edges[node.Id].Add(before);
            }
        }

        public IReadOnlyList<SceneFlowPassNode> Sort()
        {
            var indegree = _nodes.Keys.ToDictionary(k => k, _ => 0);

            foreach (var from in _edges)
            foreach (var to in from.Value)
                indegree[to]++;

            var queue = new Queue<string>(
                indegree.Where(p => p.Value == 0).Select(p => p.Key));

            var result = new List<SceneFlowPassNode>();

            while (queue.Count > 0)
            {
                var id = queue.Dequeue();
                result.Add(_nodes[id]);

                if (!_edges.TryGetValue(id, out var nexts)) continue;

                foreach (var next in nexts)
                    if (--indegree[next] == 0)
                        queue.Enqueue(next);
            }

            if (result.Count != _nodes.Count)
                throw new Exception("Circular SceneFlow pass dependency");

            return result;
        }
    }
}