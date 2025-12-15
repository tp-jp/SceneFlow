using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Job;
using UnityEngine;

namespace Samples.Sample.Editor
{
    /// <summary>
    /// SceneFlowのサンプルジョブ
    /// 複数フェーズにまたがる処理と状態保持のデモンストレーション
    /// </summary>
    public sealed class SampleJob : ISceneFlowJob
    {
        public string JobId => typeof(IntegratedDynamicDIJob).FullName;

        // ジョブ内で状態を保持
        List<GameObject> _validatedObjects;
        int _modificationCount;

        public void Configure(ISceneFlowJobBuilder builder)
        {
            // PreValidateフェーズ: シーンの検証
            builder.AddStep(SceneFlowPhase.PreValidate, "SceneFlow.Sample.Basic.Validate")
                .Execute(ctx =>
                {
                    Debug.Log("[SceneFlow Sample] Validating scene...");

                    // ルートオブジェクトを取得して検証
                    _validatedObjects = new List<GameObject>();
                    foreach (var rootObj in ctx.Scene.GetRootGameObjects())
                    {
                        if (rootObj.activeSelf)
                        {
                            _validatedObjects.Add(rootObj);
                        }
                    }

                    Debug.Log($"[SceneFlow Sample] Found {_validatedObjects.Count} active root objects");
                });

            // Processフェーズ: シーンの変更
            builder.AddStep(SceneFlowPhase.Process, "SceneFlow.Sample.Basic.Modify")
                .RunAfterStep("SceneFlow.Sample.Basic.Validate") // 検証ステップの後に実行
                .Execute(ctx =>
                {
                    Debug.Log("[SceneFlow Sample] Modifying scene...");

                    // 検証結果を使用して処理
                    if (_validatedObjects != null && _validatedObjects.Any())
                    {
                        _modificationCount = _validatedObjects.Count;
                        Debug.Log($"[SceneFlow Sample] Processing {_modificationCount} objects");

                        // ここで実際のシーン変更処理を行う
                        // 例: タグの追加、コンポーネントの追加など
                    }
                    else
                    {
                        Debug.LogWarning("[SceneFlow Sample] No objects to modify");
                    }
                });

            // PostValidateフェーズ: クリーンアップと結果の出力
            builder.AddStep(SceneFlowPhase.PostValidate, "SceneFlow.Sample.Basic.Cleanup")
                .RunAfterStep("SceneFlow.Sample.Basic.Modify") // 変更ステップの後に実行
                .Execute(ctx =>
                {
                    Debug.Log("[SceneFlow Sample] Cleaning up...");
                    Debug.Log($"[SceneFlow Sample] Total modifications: {_modificationCount}");

                    // 状態をクリア
                    _validatedObjects = null;
                    _modificationCount = 0;

                    Debug.Log("[SceneFlow Sample] Sample job completed successfully!");
                });
        }
    }
}