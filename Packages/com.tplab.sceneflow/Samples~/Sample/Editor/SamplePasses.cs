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
    /// BuildPass のサンプル実装
    /// ビルド全体で一度だけ実行される処理
    /// </summary>
    public class SampleBuildPass : IBuildPass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log("[SampleBuildPass] ビルド前の環境検証を実行");
            // 例: 
            // - Unity バージョンチェック
            // - 必須パッケージの存在確認
            // - ビルド設定の検証
        }
    }

    /// <summary>
    /// ProjectPass のサンプル実装
    /// プロジェクト全体に対する処理
    /// </summary>
    public class SampleProjectPass : IProjectPass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log("[SampleProjectPass] プロジェクト全体の処理を実行");
            // 例:
            // - ScriptableObject の生成
            // - 共通アセットの更新
            // - キャッシュの構築
        }
    }

    /// <summary>
    /// ScenePass のサンプル実装
    /// シーン単位の処理
    /// </summary>
    public class SampleScenePass : IScenePass
    {
        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[SampleScenePass] シーン '{context.Scene.name}' の処理を実行");
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
    public class CollectUdonBehaviourPass : IScenePass
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
    public class InjectReferencePass : IScenePass
    {
        // CollectUdonBehaviourPass の後に実行（同じアセンブリ内）
        public IEnumerable<Type> RunAfter { get; } = new[] { typeof(CollectUdonBehaviourPass) };

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
    public class ValidateReferencePass : IScenePass
    {
        // InjectReferencePass の後に実行
        public IEnumerable<Type> RunAfter { get; } = new[] { typeof(InjectReferencePass) };

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
    /// 他のアセンブリの Pass に依存する例
    /// アセンブリ循環参照を避けるため、文字列で指定します
    /// </summary>
    public class CrossAssemblyDependentPass : IScenePass
    {
        // 他のアセンブリの Pass を文字列で指定（アセンブリ循環参照を回避）
        public IEnumerable<string> RunAfterNames { get; } = new[]
        {
            "OtherPackage.SomePass",
            // アセンブリ名を含めた完全修飾名も可能
            // "OtherPackage.SomePass, OtherPackage.Editor"
        };

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[CrossAssemblyDependentPass] 他アセンブリの Pass の後に実行");
        }
    }

    // ========================================
    // 依存関係がない Pass の例
    // ========================================

    /// <summary>
    /// 依存関係がない Pass の例
    /// デフォルト実装により、プロパティの宣言は不要
    /// </summary>
    public class DirectInterfaceImplementationPass : IScenePass
    {
        // 依存関係がない場合は RunAfter/RunBefore を宣言する必要なし
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
    public class OptimizeScenePass : IScenePass
    {
        // すべての Pass の後に実行
        public IEnumerable<Type> RunAfter { get; } = new[]
        {
            typeof(CollectUdonBehaviourPass),
            typeof(InjectReferencePass),
            typeof(ValidateReferencePass)
        };

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[OptimizeScenePass] シーン '{context.Scene.name}' を最適化");
            // 開発用コンポーネントの削除など
        }
    }
}


