using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// ビルドパス基底インターフェース
    /// ビルド全体で一度だけ実行される処理
    /// 例: 環境検証、設定ファイル生成、事前準備、後処理
    /// </summary>
    public interface IBuildPass
    {
        /// <summary>
        /// この Pass より「後」に実行されるべき Pass 型
        /// </summary>
        IEnumerable<Type> RunAfter => Array.Empty<Type>();

        /// <summary>
        /// この Pass より「前」に実行されるべき Pass 型
        /// </summary>
        IEnumerable<Type> RunBefore => Array.Empty<Type>();

        /// <summary>
        /// ビルド処理を実行する
        /// </summary>
        /// <param name="context">SceneFlow 実行コンテキスト</param>
        void Execute(SceneFlowContext context);
    }

    /// <summary>
    /// プロジェクトパス基底インターフェース
    /// プロジェクト全体に対する処理
    /// 例: ScriptableObject 生成、共通アセットの更新、キャッシュ構築
    /// </summary>
    public interface IProjectPass
    {
        /// <summary>
        /// この Pass より「後」に実行されるべき Pass 型
        /// </summary>
        IEnumerable<Type> RunAfter => Array.Empty<Type>();

        /// <summary>
        /// この Pass より「前」に実行されるべき Pass 型
        /// </summary>
        IEnumerable<Type> RunBefore => Array.Empty<Type>();

        /// <summary>
        /// プロジェクト処理を実行する
        /// </summary>
        /// <param name="context">SceneFlow 実行コンテキスト</param>
        void Execute(SceneFlowContext context);
    }

    /// <summary>
    /// シーンパス基底インターフェース
    /// シーン単位の処理
    /// VRChat では実質「ワールド = 1 シーン」
    /// </summary>
    public interface IScenePass
    {
        /// <summary>
        /// この Pass より「後」に実行されるべき Pass 型
        /// </summary>
        IEnumerable<Type> RunAfter => Array.Empty<Type>();

        /// <summary>
        /// この Pass より「前」に実行されるべき Pass 型
        /// </summary>
        IEnumerable<Type> RunBefore => Array.Empty<Type>();

        /// <summary>
        /// シーン処理を実行する
        /// </summary>
        /// <param name="context">SceneFlow 実行コンテキスト</param>
        void Execute(SceneFlowContext context);
    }
}
