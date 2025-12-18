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
        public static List<T> Sort<T>(IEnumerable<T> passes) where T : IPass
        {
            var passList = passes.ToList();
            if (passList.Count == 0) return passList;

            // Type -> インスタンスのマッピング
            var typeToInstance = passList.ToDictionary(p => p.GetType(), p => p);
            
            // 型名 -> Type のマッピング（文字列参照用）
            var nameToType = BuildNameToTypeMapping(passList);

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

                // RunAfter (Type): この Pass は指定された Pass の「後」に実行される
                foreach (var afterType in pass.RunAfter)
                {
                    AddDependency(graph, inDegree, afterType, passType, passType.Name, afterType.Name);
                }

                // RunBefore (Type): この Pass は指定された Pass の「前」に実行される
                foreach (var beforeType in pass.RunBefore)
                {
                    AddDependency(graph, inDegree, passType, beforeType, passType.Name, beforeType.Name);
                }

                // RunAfterNames (string): 文字列ベースの依存関係
                foreach (var afterName in pass.RunAfterNames)
                {
                    if (nameToType.TryGetValue(afterName, out var afterType))
                    {
                        AddDependency(graph, inDegree, afterType, passType, passType.Name, afterName);
                    }
                    else
                    {
                        Logger.LogWarning($"Pass {passType.Name} depends on '{afterName}' which is not found in the pass list");
                    }
                }

                // RunBeforeNames (string): 文字列ベースの依存関係
                foreach (var beforeName in pass.RunBeforeNames)
                {
                    if (nameToType.TryGetValue(beforeName, out var beforeType))
                    {
                        AddDependency(graph, inDegree, passType, beforeType, passType.Name, beforeName);
                    }
                    else
                    {
                        Logger.LogWarning($"Pass {passType.Name} should run before '{beforeName}' which is not found in the pass list");
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

        /// <summary>
        /// 型名から Type へのマッピングを構築
        /// FullName と "FullName, AssemblyName" の両方に対応
        /// </summary>
        static Dictionary<string, Type> BuildNameToTypeMapping<T>(List<T> passes)
        {
            var mapping = new Dictionary<string, Type>(StringComparer.Ordinal);

            foreach (var pass in passes)
            {
                var type = pass.GetType();
                
                // FullName で登録
                if (type.FullName != null)
                {
                    mapping[type.FullName] = type;
                }
                
                // AssemblyQualifiedName で登録
                if (type.AssemblyQualifiedName != null)
                {
                    mapping[type.AssemblyQualifiedName] = type;
                }
                
                // "FullName, AssemblyName" 形式でも登録
                if (type.FullName != null && type.Assembly?.GetName()?.Name != null)
                {
                    var shortAssemblyName = $"{type.FullName}, {type.Assembly.GetName().Name}";
                    mapping[shortAssemblyName] = type;
                }
            }

            return mapping;
        }

        /// <summary>
        /// グラフに依存関係を追加
        /// </summary>
        static void AddDependency(
            Dictionary<Type, HashSet<Type>> graph,
            Dictionary<Type, int> inDegree,
            Type from,
            Type to,
            string fromName,
            string toName)
        {
            if (graph.ContainsKey(from) && graph.ContainsKey(to))
            {
                if (graph[from].Add(to))
                {
                    inDegree[to]++;
                }
            }
            else if (!graph.ContainsKey(from))
            {
                Logger.LogWarning($"Pass {fromName} depends on {toName} which is not found in the pass list");
            }
        }
    }
}

