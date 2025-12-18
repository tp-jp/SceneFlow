using System;
using System.Collections.Generic;
using System.Reflection;
using TpLab.SceneFlow.Editor.Internal;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// アセンブリから Pass を検出する
    /// </summary>
    public static class PassDiscovery
    {
        /// <summary>
        /// IBuildPass を実装した Pass をすべて検出する
        /// </summary>
        public static IEnumerable<IBuildPass> DiscoverBuildPasses()
        {
            return DiscoverPasses<IBuildPass>();
        }

        /// <summary>
        /// IProjectPass を実装した Pass をすべて検出する
        /// </summary>
        public static IEnumerable<IProjectPass> DiscoverProjectPasses()
        {
            return DiscoverPasses<IProjectPass>();
        }

        /// <summary>
        /// IScenePass を実装した Pass をすべて検出する
        /// </summary>
        public static IEnumerable<IScenePass> DiscoverScenePasses()
        {
            return DiscoverPasses<IScenePass>();
        }

        /// <summary>
        /// IPass を実装した Pass をすべて検出する（汎用メソッド）
        /// </summary>
        public static IEnumerable<T> DiscoverPasses<T>() where T : IPass
        {
            var interfaceType = typeof(T);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    // アセンブリ読み込みエラーは無視
                    continue;
                }

                foreach (var type in types)
                {
                    // インターフェースを実装していて、具象クラスであることを確認
                    if (interfaceType.IsAssignableFrom(type) && 
                        !type.IsInterface && 
                        !type.IsAbstract)
                    {
                        T instance;
                        try
                        {
                            // デフォルトコンストラクタでインスタンス化
                            instance = (T)Activator.CreateInstance(type);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Failed to instantiate {type.Name}: {ex.Message}");
                            continue;
                        }

                        if (instance != null)
                        {
                            yield return instance;
                        }
                    }
                }
            }
        }
    }
}

