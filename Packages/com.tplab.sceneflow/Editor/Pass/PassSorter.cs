using System;
using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Internal;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// Pass の依存関係に基づいてトポロジカルソートを行う
    /// </summary>
    public static class PassSorter
    {
        /// <summary>
        /// Pass インスタンスのリストを依存関係に基づいてソートする
        /// </summary>
        public static List<T> Sort<T>(IEnumerable<T> passes, Func<T, IEnumerable<Type>> getRunAfter, Func<T, IEnumerable<Type>> getRunBefore)
        {
            var passList = passes.ToList();
            if (passList.Count == 0) return passList;

            // Type -> インスタンスのマッピング
            var typeToInstance = passList.ToDictionary(p => p.GetType(), p => p);

            // 依存関係グラフを構築
            var graph = new Dictionary<Type, HashSet<Type>>();
            var inDegree = new Dictionary<Type, int>();

            foreach (var pass in passList)
            {
                var passType = pass.GetType();
                if (!graph.ContainsKey(passType))
                {
                    graph[passType] = new HashSet<Type>();
                    inDegree[passType] = 0;
                }
            }

            // RunAfter と RunBefore から依存関係を構築
            foreach (var pass in passList)
            {
                var passType = pass.GetType();

                // RunAfter: この Pass は指定された Pass の「後」に実行される
                // つまり、指定された Pass → この Pass という依存
                foreach (var afterType in getRunAfter(pass))
                {
                    if (graph.ContainsKey(afterType))
                    {
                        graph[afterType].Add(passType);
                        inDegree[passType]++;
                    }
                    else
                    {
                        Logger.LogWarning($"Pass {passType.Name} depends on {afterType.Name} which is not found in the pass list");
                    }
                }

                // RunBefore: この Pass は指定された Pass の「前」に実行される
                // つまり、この Pass → 指定された Pass という依存
                foreach (var beforeType in getRunBefore(pass))
                {
                    if (graph.ContainsKey(beforeType))
                    {
                        graph[passType].Add(beforeType);
                        inDegree[beforeType]++;
                    }
                    else
                    {
                        Logger.LogWarning($"Pass {passType.Name} should run before {beforeType.Name} which is not found in the pass list");
                    }
                }
            }

            // トポロジカルソート (Kahn's algorithm)
            var queue = new Queue<Type>();
            foreach (var kvp in inDegree)
            {
                if (kvp.Value == 0)
                {
                    queue.Enqueue(kvp.Key);
                }
            }

            var result = new List<Type>();
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var neighbor in graph[current])
                {
                    inDegree[neighbor]--;
                    if (inDegree[neighbor] == 0)
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // 循環依存チェック
            if (result.Count != passList.Count)
            {
                var remaining = passList.Select(p => p.GetType()).Except(result).ToList();
                throw new InvalidOperationException(
                    $"Circular dependency detected in passes: {string.Join(", ", remaining.Select(t => t.Name))}");
            }

            // Type の順序に従ってインスタンスを並べ替え
            return result.Select(t => typeToInstance[t]).ToList();
        }
    }
}

