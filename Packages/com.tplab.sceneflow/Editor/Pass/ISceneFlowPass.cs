using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// SceneFlow Pass 基底クラス
    /// ビルド時に実行される処理の最小単位
    /// 
    /// ■ 実行順序の制御
    /// - ConfigureDependencies で依存関係を宣言
    /// - PassSorter が自動的にトポロジカルソート
    /// 
    /// ■ 依存関係の宣言方法
    /// <code>
    /// protected override void ConfigureDependencies(DependencyBuilder builder)
    /// {
    ///     builder
    ///         .After&lt;SomePass&gt;()  // Type参照（同一アセンブリ推奨）
    ///         .After("Other.Pass, OtherAssembly")  // 文字列参照（他アセンブリ）
    ///         .Before&lt;AnotherPass&gt;();  // この Pass は AnotherPass の前に実行
    /// }
    /// </code>
    /// 
    /// ■ 用途例
    /// - ビルド環境の検証
    /// - アセットの生成・更新
    /// - シーン内オブジェクトの処理
    /// - 参照の自動設定
    /// </summary>
    public abstract class IPass
    {
        IEnumerable<PassDependency> _dependencies;

        /// <summary>
        /// Pass の依存関係を取得
        /// </summary>
        internal IEnumerable<PassDependency> Dependencies
        {
            get
            {
                if (_dependencies == null)
                {
                    var builder = DependencyBuilder.Create();
                    ConfigureDependencies(builder);
                    _dependencies = builder.Build();
                }
                return _dependencies;
            }
        }

        /// <summary>
        /// Pass の依存関係を設定
        /// 
        /// ■ 使用方法
        /// <code>
        /// protected override void ConfigureDependencies(DependencyBuilder builder)
        /// {
        ///     builder
        ///         .After&lt;FirstPass&gt;()
        ///         .After&lt;SecondPass&gt;()
        ///         .Before&lt;FinalPass&gt;()
        ///         .After("OtherPackage.Pass, OtherAssembly");  // 他アセンブリ参照
        /// }
        /// </code>
        /// </summary>
        protected virtual void ConfigureDependencies(DependencyBuilder builder)
        {
            // デフォルトでは依存関係なし
        }

        /// <summary>
        /// Pass 処理を実行する
        /// </summary>
        /// <param name="context">SceneFlow 実行コンテキスト</param>
        public abstract void Execute(SceneFlowContext context);
    }
}

