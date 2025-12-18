using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// Pass の基本インターフェース
    /// すべての Pass が実装する共通機能を定義
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

    /// <summary>
    /// ビルドパス基底インターフェース
    /// ビルド全体で一度だけ実行される処理
    /// 例: 環境検証、設定ファイル生成、事前準備、後処理
    /// </summary>
    public interface IBuildPass : IPass
    {
    }

    /// <summary>
    /// プロジェクトパス基底インターフェース
    /// プロジェクト全体に対する処理
    /// 例: ScriptableObject 生成、共通アセットの更新、キャッシュ構築
    /// </summary>
    public interface IProjectPass : IPass
    {
    }

    /// <summary>
    /// シーンパス基底インターフェース
    /// シーン単位の処理
    /// </summary>
    public interface IScenePass : IPass
    {
    }
}

