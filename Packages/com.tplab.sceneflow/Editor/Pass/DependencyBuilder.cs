using System;
using System.Collections.Generic;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// Pass の依存関係を流暢に定義するための Builder
    /// 
    /// ■ 使用例
    /// <code>
    /// public IEnumerable&lt;PassDependency&gt; Dependencies => DependencyBuilder
    ///     .Create()
    ///     .After&lt;FirstPass&gt;()
    ///     .After&lt;SecondPass&gt;()
    ///     .Before&lt;FinalPass&gt;()
    ///     .After("OtherPackage.Pass, OtherAssembly")
    ///     .Build();
    /// </code>
    /// </summary>
    public sealed class DependencyBuilder
    {
        readonly List<PassDependency> _dependencies = new List<PassDependency>();

        /// <summary>
        /// 新しい Builder インスタンスを作成
        /// </summary>
        public static DependencyBuilder Create() => new DependencyBuilder();

        /// <summary>
        /// 指定された Pass の後に実行（Type 参照）
        /// </summary>
        public DependencyBuilder After<T>() where T : IPass
        {
            _dependencies.Add(PassDependency.After<T>());
            return this;
        }

        /// <summary>
        /// 指定された Pass の後に実行（Type 参照）
        /// </summary>
        public DependencyBuilder After(Type passType)
        {
            _dependencies.Add(PassDependency.After(passType));
            return this;
        }

        /// <summary>
        /// 指定された Pass の後に実行（文字列参照）
        /// </summary>
        public DependencyBuilder After(string passTypeName)
        {
            _dependencies.Add(PassDependency.After(passTypeName));
            return this;
        }

        /// <summary>
        /// 指定された Pass の前に実行（Type 参照）
        /// </summary>
        public DependencyBuilder Before<T>() where T : IPass
        {
            _dependencies.Add(PassDependency.Before<T>());
            return this;
        }

        /// <summary>
        /// 指定された Pass の前に実行（Type 参照）
        /// </summary>
        public DependencyBuilder Before(Type passType)
        {
            _dependencies.Add(PassDependency.Before(passType));
            return this;
        }

        /// <summary>
        /// 指定された Pass の前に実行（文字列参照）
        /// </summary>
        public DependencyBuilder Before(string passTypeName)
        {
            _dependencies.Add(PassDependency.Before(passTypeName));
            return this;
        }

        /// <summary>
        /// 依存関係のコレクションを構築
        /// </summary>
        public IEnumerable<PassDependency> Build() => _dependencies;

        /// <summary>
        /// 暗黙的に IEnumerable に変換（Build() の省略可能）
        /// </summary>
        public static implicit operator PassDependency[](DependencyBuilder builder)
        {
            return builder._dependencies.ToArray();
        }
    }
}

