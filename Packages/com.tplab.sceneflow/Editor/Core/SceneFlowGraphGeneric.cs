using System;
using System.Collections.Generic;
using System.Linq;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// ノードインターフェース
    /// </summary>
    internal interface IGraphNode
    {
        string Id { get; }
        HashSet<string> RunAfter { get; }
        HashSet<string> RunBefore { get; }
    }

    /// <summary>
    /// シーンフローの依存関係グラフ（汎用版）
    /// </summary>
    internal sealed class SceneFlowGraph<T> where T : IGraphNode
    {
        readonly Dictionary<string, T> _nodes;
        readonly Dictionary<string, HashSet<string>> _edges = new();

        public SceneFlowGraph(IEnumerable<T> nodes)
        {
            _nodes = nodes.ToDictionary(n => n.Id);
            BuildEdges();
        }

        void BuildEdges()
        {
            foreach (var node in _nodes.Values)
            {
                if (!_edges.ContainsKey(node.Id))
                    _edges[node.Id] = new();

                foreach (var after in node.RunAfter)
                {
                    if (_nodes.ContainsKey(after))
                    {
                        if (!_edges.ContainsKey(after))
                            _edges[after] = new();
                        _edges[after].Add(node.Id);
                    }
                }

                foreach (var before in node.RunBefore)
                {
                    if (_nodes.ContainsKey(before))
                    {
                        _edges[node.Id].Add(before);
                    }
                }
            }
        }

        public IReadOnlyList<T> Sort()
        {
            var indegree = _nodes.Keys.ToDictionary(k => k, _ => 0);

            foreach (var from in _edges)
            foreach (var to in from.Value)
                indegree[to]++;

            var queue = new Queue<string>(
                indegree.Where(p => p.Value == 0).Select(p => p.Key));

            var result = new List<T>();

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
                throw new Exception("Circular dependency detected in SceneFlow graph");

            return result;
        }
    }
}

