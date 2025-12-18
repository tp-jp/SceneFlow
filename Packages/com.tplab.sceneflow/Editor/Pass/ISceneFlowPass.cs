using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// SceneFlow Pass インターフェース
    /// ビルド時に実行される処理の最小単位
    /// 
    /// ■ 実行順序の制御
    /// - Dependencies で依存関係を宣言
    /// - PassSorter が自動的にトポロジカルソート
    /// 
    /// ■ 依存関係の宣言方法
    /// <code>
    /// public IEnumerable&lt;PassDependency&gt; Dependencies => DependencyBuilder
    ///     .Create()
    ///     .After&lt;SomePass&gt;()  // Type参照（同一アセンブリ推奨）
    ///     .After("Other.Pass, OtherAssembly")  // 文字列参照（他アセンブリ）
    ///     .Before&lt;AnotherPass&gt;()  // この Pass は AnotherPass の前に実行
    ///     .Build();
    /// </code>
    /// 
    /// ■ 用途例
    /// - ビルド環境の検証
    /// - アセットの生成・更新
    /// - シーン内オブジェクトの処理
    /// - 参照の自動設定
    /// </summary>
    public interface IPass
    {
        /// <summary>
        /// Pass の依存関係
        /// 
        /// ■ 使用方法（推奨：DependencyBuilder）
        /// <code>
        /// public IEnumerable&lt;PassDependency&gt; Dependencies => DependencyBuilder
        ///     .Create()
        ///     .After&lt;FirstPass&gt;()
        ///     .After&lt;SecondPass&gt;()
        ///     .Before&lt;FinalPass&gt;()
        ///     .After("OtherPackage.Pass, OtherAssembly")  // 他アセンブリ参照
        ///     .Build();
        /// </code>
        /// </summary>
        IEnumerable<PassDependency> Dependencies => Array.Empty<PassDependency>();

        /// <summary>
        /// Pass 処理を実行する
        /// </summary>
        /// <param name="context">SceneFlow 実行コンテキスト</param>
        void Execute(SceneFlowContext context);
    }
}

