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
    /// - RunAfter/RunBefore で依存関係を宣言
    /// - PassSorter が自動的にトポロジカルソート
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
        /// この Pass より「後」に実行されるべき Pass 型（同一アセンブリ内推奨）
        /// </summary>
        IEnumerable<Type> RunAfter => Array.Empty<Type>();

        /// <summary>
        /// この Pass より「前」に実行されるべき Pass 型（同一アセンブリ内推奨）
        /// </summary>
        IEnumerable<Type> RunBefore => Array.Empty<Type>();

        /// <summary>
        /// この Pass より「後」に実行されるべき Pass の型名（他アセンブリ参照時に使用）
        /// 形式: "Namespace.ClassName" または "Namespace.ClassName, AssemblyName"
        /// アセンブリ循環参照を避けるため、他アセンブリの Pass を参照する場合はこちらを使用してください
        /// </summary>
        IEnumerable<string> RunAfterNames => Array.Empty<string>();

        /// <summary>
        /// この Pass より「前」に実行されるべき Pass の型名（他アセンブリ参照時に使用）
        /// 形式: "Namespace.ClassName" または "Namespace.ClassName, AssemblyName"
        /// アセンブリ循環参照を避けるため、他アセンブリの Pass を参照する場合はこちらを使用してください
        /// </summary>
        IEnumerable<string> RunBeforeNames => Array.Empty<string>();

        /// <summary>
        /// Pass 処理を実行する
        /// </summary>
        /// <param name="context">SceneFlow 実行コンテキスト</param>
        void Execute(SceneFlowContext context);
    }
}

