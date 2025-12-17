using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEngine;

namespace TpLab.SceneFlow.Samples
{
    /// <summary>
    /// BuildPass のサンプル実装
    /// ビルド全体で一度だけ実行される処理
    /// </summary>
    public class SampleBuildPass : IBuildPass
    {
        // 依存関係がない場合は実装不要（デフォルト実装が使われる）
        // public IEnumerable<Type> RunAfter => Array.Empty<Type>();
        // public IEnumerable<Type> RunBefore => Array.Empty<Type>();

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
        // CollectUdonBehaviourPass の後に実行
        public IEnumerable<Type> RunAfter
        {
            get { yield return typeof(CollectUdonBehaviourPass); }
        }

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
        public IEnumerable<Type> RunAfter
        {
            get { yield return typeof(InjectReferencePass); }
        }

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[ValidateReferencePass] シーン '{context.Scene.name}' の参照を検証");
            // 参照が正しく設定されているか確認
        }
    }

    /// <summary>
    /// 最適化を行う Pass
    /// 他のすべての処理の後に実行される
    /// </summary>
    public class OptimizeScenePass : IScenePass
    {
        // すべての Pass の後に実行
        public IEnumerable<Type> RunAfter
        {
            get
            {
                yield return typeof(CollectUdonBehaviourPass);
                yield return typeof(InjectReferencePass);
                yield return typeof(ValidateReferencePass);
            }
        }

        public void Execute(SceneFlowContext context)
        {
            Debug.Log($"[OptimizeScenePass] シーン '{context.Scene.name}' を最適化");
            // 開発用コンポーネントの削除など
        }
    }
}
}

