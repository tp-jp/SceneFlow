using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEngine;

namespace TpLab.SceneFlow.Samples
{
    // ========================================
    // 基本的な Pass の実装例
    // ========================================

    /// <summary>
    /// 環境検証を行う Pass
    /// ビルド前に環境をチェックする用途
    /// </summary>
    public class EnvironmentValidationPass : IPass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log("[EnvironmentValidationPass] ビルド環境を検証");
            // 例: 
            // - Unity バージョンチェック
            // - 必須パッケージの存在確認
            // - ビルド設定の検証
        }
    }

    /// <summary>
    /// アセット生成を行う Pass
    /// プロジェクト全体に対する処理
    /// </summary>
    public class AssetGenerationPass : IPass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log("[AssetGenerationPass] アセットを生成");
            // 例:
            // - ScriptableObject の生成
            // - 共通アセットの更新
            // - キャッシュの構築
        }
    }

    /// <summary>
    /// シーン処理を行う Pass
    /// シーン内のオブジェクトを処理
    /// </summary>
    public class SceneObjectProcessPass : IPass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[SceneObjectProcessPass] シーン '{context.Scene.name}' のオブジェクトを処理");
            // 例:
            // - シーン内のオブジェクト検証
            // - コンポーネントの自動設定
            // - 参照の解決
        }
    }

    // ========================================
    // 依存関係を持つ Pass の例
    // ========================================

    /// <summary>
    /// UdonBehaviour を収集する Pass
    /// </summary>
    public class CollectUdonBehaviourPass : IPass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[CollectUdonBehaviourPass] シーン '{context.Scene.name}' の UdonBehaviour を収集");
            // UdonBehaviour を検出してキャッシュに保存
        }
    }

    /// <summary>
    /// 参照を注入する Pass
    /// CollectUdonBehaviourPass の後に実行される必要がある
    /// </summary>
    public class InjectReferencePass : IPass
    {
        // DependencyBuilder を使用した依存関係の宣言（推奨）
        public IEnumerable<PassDependency> Dependencies => DependencyBuilder
            .Create()
            .After<CollectUdonBehaviourPass>()
            .Build();

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[InjectReferencePass] シーン '{context.Scene.name}' に参照を注入");
            // キャッシュから UdonBehaviour を取得して参照を設定
        }
    }

    /// <summary>
    /// 検証を行う Pass
    /// InjectReferencePass の後に実行される必要がある
    /// </summary>
    public class ValidateReferencePass : IPass
    {
        // DependencyBuilder を使用した依存関係の宣言（推奨）
        public IEnumerable<PassDependency> Dependencies => DependencyBuilder
            .Create()
            .After<InjectReferencePass>()
            .Build();

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[ValidateReferencePass] シーン '{context.Scene.name}' の参照を検証");
            // 参照が正しく設定されているか確認
        }
    }

    // ========================================
    // 他のアセンブリの Pass に依存する例
    // ========================================

    /// <summary>
    /// 他のアセンブリの Pass に依存する場合の例
    /// アセンブリ循環参照を避けるため、文字列で完全修飾名を指定します
    /// 
    /// ■ 文字列参照が必要なケース
    /// - Assembly A の Pass が Assembly B の Pass に依存
    /// - Assembly B の Pass が Assembly A の Pass に依存
    /// このような循環参照を避けるため、どちらか一方を文字列で参照します
    /// 
    /// ■ 使用例（DependencyBuilder 推奨）
    /// <code>
    /// public IEnumerable&lt;PassDependency&gt; Dependencies => DependencyBuilder
    ///     .Create()
    ///     .After("OtherNamespace.SomePass, OtherAssembly.Editor")
    ///     .Before("YetAnotherNamespace.AnotherPass, ThirdPartyPackage.Editor")
    ///     .Build();
    /// </code>
    /// </summary>
    public class CrossAssemblyDependentPassExample : IPass
    {
        // DependencyBuilder を使用（文字列参照）
        public IEnumerable<PassDependency> Dependencies => DependencyBuilder
            .Create()
            // このサンプルでは同一アセンブリ内の Pass を文字列で参照（動作確認用）
            .After("TpLab.SceneFlow.Samples.CollectUdonBehaviourPass")
            // 他のアセンブリの Pass を参照する場合の例：
            // .After("OtherNamespace.SomePass, OtherAssembly.Editor")
            // .Before("YetAnotherNamespace.AnotherPass, ThirdPartyPackage.Editor")
            .Build();

        public void Execute(SceneFlowContext context)
        {
            Debug.Log("[CrossAssemblyDependentPassExample] 文字列参照による依存関係の例");
        }
    }

    // ========================================
    // 依存関係がない Pass の例
    // ========================================

    /// <summary>
    /// 依存関係がない Pass の例
    /// Dependencies プロパティを宣言する必要はありません（デフォルト実装が使用される）
    /// </summary>
    public class DirectInterfaceImplementationPass : IPass
    {
        // 依存関係がない場合は Dependencies を宣言する必要なし
        // インターフェースのデフォルト実装（Array.Empty）が使用される

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[DirectInterfaceImplementationPass] 依存関係なしで実行");
        }
    }

    /// <summary>
    /// 最適化を行う Pass
    /// 他のすべての処理の後に実行される
    /// </summary>
    public class OptimizeScenePass : IPass
    {
        // DependencyBuilder を使用（複数の依存関係を宣言）
        public IEnumerable<PassDependency> Dependencies => DependencyBuilder
            .Create()
            .After<CollectUdonBehaviourPass>()
            .After<InjectReferencePass>()
            .After<ValidateReferencePass>()
            .Build();

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[OptimizeScenePass] シーン '{context.Scene.name}' を最適化");
            // 開発用コンポーネントの削除など
        }
    }

    // ========================================
    // Before の使用例
    // ========================================

    /// <summary>
    /// 準備処理を行う Pass
    /// 「Before」を使って他の Pass より前に実行されることを宣言
    /// </summary>
    public class PreparationPass : IPass
    {
        // DependencyBuilder を使用（Before の例）
        // CollectUdonBehaviourPass の「前」に実行されることを宣言
        public IEnumerable<PassDependency> Dependencies => DependencyBuilder
            .Create()
            .Before<CollectUdonBehaviourPass>()
            .Build();

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[PreparationPass] シーン '{context.Scene.name}' の準備処理");
            // UdonBehaviour の収集前に必要な準備を行う
        }
    }
}



