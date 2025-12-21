using System;
using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Internals;

namespace TpLab.SceneFlow.Editor.Passes
{
    /// <summary>
    /// Pass の依存関係に基づいてトポロジカルソートを行う
    /// </summary>
    public static class PassSorter
    {
        /// <summary>
        /// Pass インスタンスのリストを依存関係に基づいてソートする
        /// </summary>
        /// <param name="passes">ソート対象の Pass リスト</param>
        /// <param name="suppressWarnings">警告を抑制するか（デバッグウィンドウのフィルタリング時など）</param>
        public static List<T> Sort<T>(IEnumerable<T> passes, bool suppressWarnings = false) where T : IPass
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

            // Dependencies から依存関係を構築
            foreach (var pass in passList)
            {
                var passType = pass.GetType();

                foreach (var dependency in pass.Dependencies)
                {
                    if (dependency.IsTypeReference)
                    {
                        var targetType = dependency.TargetType;
                        if (dependency.Relation == PassDependency.Direction.After)
                        {
                            AddDependency(graph, inDegree, targetType, passType, passType.Name, targetType.Name, suppressWarnings);
                        }
                        else // Before
                        {
                            AddDependency(graph, inDegree, passType, targetType, passType.Name, targetType.Name, suppressWarnings);
                        }
                    }
                    else if (dependency.IsNameReference)
                    {
                        var targetName = dependency.TargetTypeName;
                        if (nameToType.TryGetValue(targetName, out var targetType))
                        {
                            if (dependency.Relation == PassDependency.Direction.After)
                            {
                                AddDependency(graph, inDegree, targetType, passType, passType.Name, targetName, suppressWarnings);
                            }
                            else // Before
                            {
                                AddDependency(graph, inDegree, passType, targetType, passType.Name, targetName, suppressWarnings);
                            }
                        }
                        else if (!suppressWarnings)
                        {
                            var relation = dependency.Relation == PassDependency.Direction.After ? "depends on" : "should run before";
                            Logger.LogWarning($"Pass '{passType.Name}' {relation} '{targetName}' which is not found in the current pass list");
                        }
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
                if (type.FullName != null && type.Assembly != null)
                {
                    var assemblyName = type.Assembly.GetName()?.Name;
                    if (assemblyName != null)
                    {
                        var shortAssemblyName = $"{type.FullName}, {assemblyName}";
                        mapping[shortAssemblyName] = type;
                    }
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
            string toName,
            bool suppressWarnings)
        {
            // 両方の型がグラフに存在する場合のみ依存関係を追加
            if (graph.ContainsKey(from) && graph.ContainsKey(to))
            {
                if (graph[from].Add(to))
                {
                    inDegree[to]++;
                }
            }
            // from が存在しない場合は警告（ただし suppressWarnings が false の場合のみ）
            // これは文字列参照で発生する可能性がある
            else if (!graph.ContainsKey(from) && !suppressWarnings)
            {
                Logger.LogWarning($"Pass '{fromName}' depends on '{toName}' which is not found in the current pass list");
            }
            // to が存在しない場合も同様（ただし、これは Type 参照では通常発生しない）
            else if (!graph.ContainsKey(to) && !suppressWarnings)
            {
                Logger.LogWarning($"Pass '{fromName}' has a dependency on '{toName}' which is not found in the current pass list");
            }
        }
    }
}

