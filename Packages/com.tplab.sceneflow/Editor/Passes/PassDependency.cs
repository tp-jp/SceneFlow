using System;

namespace TpLab.SceneFlow.Editor.Passes
{
    /// <summary>
    /// Pass の依存関係を表すクラス
    /// 
    /// ■ 推奨：DependencyBuilder を使用
    /// <code>
    /// public IEnumerable&lt;PassDependency&gt; Dependencies => DependencyBuilder
    ///     .Create()
    ///     .After&lt;SomePass&gt;()
    ///     .After("OtherNamespace.OtherPass, OtherAssembly")
    ///     .Before&lt;AnotherPass&gt;()
    ///     .Build();
    /// </code>
    /// 
    /// ■ 従来の方法
    /// <code>
    /// public IEnumerable&lt;PassDependency&gt; Dependencies => new[]
    /// {
    ///     PassDependency.After&lt;SomePass&gt;(),
    ///     PassDependency.After("OtherNamespace.OtherPass, OtherAssembly"),
    ///     PassDependency.Before&lt;AnotherPass&gt;()
    /// };
    /// </code>
    /// </summary>
    public sealed class PassDependency
    {
        /// <summary>
        /// 依存関係の方向
        /// </summary>
        public enum Direction
        {
            /// <summary>指定された Pass の後に実行</summary>
            After,
            /// <summary>指定された Pass の前に実行</summary>
            Before
        }

        /// <summary>
        /// 依存する Pass の Type（Type 参照の場合のみ）
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// 依存する Pass の型名（文字列参照の場合のみ）
        /// </summary>
        public string TargetTypeName { get; }

        /// <summary>
        /// 依存関係の方向（After/Before）
        /// </summary>
        public Direction Relation { get; }

        /// <summary>
        /// Type 参照かどうか
        /// </summary>
        public bool IsTypeReference => TargetType != null;

        /// <summary>
        /// 文字列参照かどうか
        /// </summary>
        public bool IsNameReference => !string.IsNullOrEmpty(TargetTypeName);

        PassDependency(Type targetType, string targetTypeName, Direction relation)
        {
            TargetType = targetType;
            TargetTypeName = targetTypeName;
            Relation = relation;
        }

        /// <summary>
        /// 指定された Pass の後に実行（Type 参照）
        /// </summary>
        /// <typeparam name="T">依存する Pass の型</typeparam>
        public static PassDependency After<T>() where T : IPass
        {
            return new PassDependency(typeof(T), null, Direction.After);
        }

        /// <summary>
        /// 指定された Pass の後に実行（Type 参照）
        /// </summary>
        /// <param name="passType">依存する Pass の型</param>
        public static PassDependency After(Type passType)
        {
            if (passType == null) throw new ArgumentNullException(nameof(passType));
            if (!typeof(IPass).IsAssignableFrom(passType))
                throw new ArgumentException($"Type {passType.Name} does not implement IPass", nameof(passType));
            
            return new PassDependency(passType, null, Direction.After);
        }

        /// <summary>
        /// 指定された Pass の後に実行（文字列参照）
        /// </summary>
        /// <param name="passTypeName">依存する Pass の型名（"Namespace.ClassName" または "Namespace.ClassName, AssemblyName"）</param>
        public static PassDependency After(string passTypeName)
        {
            if (string.IsNullOrWhiteSpace(passTypeName))
                throw new ArgumentException("Pass type name cannot be null or empty", nameof(passTypeName));
            
            return new PassDependency(null, passTypeName, Direction.After);
        }

        /// <summary>
        /// 指定された Pass の前に実行（Type 参照）
        /// </summary>
        /// <typeparam name="T">依存される Pass の型</typeparam>
        public static PassDependency Before<T>() where T : IPass
        {
            return new PassDependency(typeof(T), null, Direction.Before);
        }

        /// <summary>
        /// 指定された Pass の前に実行（Type 参照）
        /// </summary>
        /// <param name="passType">依存される Pass の型</param>
        public static PassDependency Before(Type passType)
        {
            if (passType == null) throw new ArgumentNullException(nameof(passType));
            if (!typeof(IPass).IsAssignableFrom(passType))
                throw new ArgumentException($"Type {passType.Name} does not implement IPass", nameof(passType));
            
            return new PassDependency(passType, null, Direction.Before);
        }

        /// <summary>
        /// 指定された Pass の前に実行（文字列参照）
        /// </summary>
        /// <param name="passTypeName">依存される Pass の型名（"Namespace.ClassName" または "Namespace.ClassName, AssemblyName"）</param>
        public static PassDependency Before(string passTypeName)
        {
            if (string.IsNullOrWhiteSpace(passTypeName))
                throw new ArgumentException("Pass type name cannot be null or empty", nameof(passTypeName));
            
            return new PassDependency(null, passTypeName, Direction.Before);
        }

        public override string ToString()
        {
            var target = IsTypeReference ? TargetType.Name : TargetTypeName;
            return $"{Relation} {target}";
        }
    }
}

