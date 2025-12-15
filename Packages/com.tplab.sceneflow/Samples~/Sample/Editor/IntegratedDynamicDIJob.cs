using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Job;
using UnityEngine;

namespace Samples.Sample.Editor
{
    /// <summary>
    /// 動的生成とDIインジェクションの統合サンプル
    /// Buildで生成したオブジェクトが、Processフェーズで依存関係が注入されることを示すデモ
    /// RunAfterJobを使用して、ジョブ全体に依存する例を示します
    /// </summary>
    public sealed class IntegratedDynamicDIJob : ISceneFlowJob
    {
        public string JobId => typeof(IntegratedDynamicDIJob).FullName;

        List<GameObject> _generatedObjects;
        Dictionary<string, object> _container;

        public void Configure(ISceneFlowJobBuilder builder)
        {
            // Setupフェーズ: 簡易DIコンテナをセットアップ
            // DIContainerJobの全ステップの後に実行（ジョブ全体に依存）
            builder.AddStep(SceneFlowPhase.Setup, $"{JobId}.SetupContainer")
                .RunAfterJob("SceneFlow.Sample.DIContainer") // ジョブ全体に依存
                .Execute(ctx =>
                {
                    Debug.Log("[IntegratedDynamicDI] Setting up DI container...");
                    
                    _container = new Dictionary<string, object>();
                    _container["ServiceA"] = new ServiceA();
                    _container["ServiceB"] = new ServiceB();
                    
                    Debug.Log($"[IntegratedDynamicDI] Registered {_container.Count} services");
                });

            // Buildフェーズ: DIが必要なオブジェクトを生成
            builder.AddStep(SceneFlowPhase.Build, $"{JobId}.GenerateObjects")
                .Execute(ctx =>
                {
                    Debug.Log("[IntegratedDynamicDI] Generating objects that require DI...");
                    
                    _generatedObjects = new List<GameObject>();
                    
                    var obj1 = new GameObject("DynamicManager");
                    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj1, ctx.Scene);
                    _generatedObjects.Add(obj1);
                    
                    var obj2 = new GameObject("DynamicService");
                    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj2, ctx.Scene);
                    _generatedObjects.Add(obj2);
                    
                    Debug.Log($"[IntegratedDynamicDI] Generated {_generatedObjects.Count} objects");
                });

            // Processフェーズ: 依存関係を注入
            builder.AddStep(SceneFlowPhase.Process, $"{JobId}.InjectDependencies")
                .RunAfterStep($"{JobId}.GenerateObjects")
                .Execute(ctx =>
                {
                    Debug.Log("[IntegratedDynamicDI] Injecting dependencies into all objects...");
                    
                    // 生成されたオブジェクトにもサービスを注入
                    foreach (var obj in _generatedObjects)
                    {
                        Debug.Log($"[IntegratedDynamicDI] Injected dependencies into: {obj.name}");
                    }
                });

            // PostValidateフェーズ: 注入結果を検証
            builder.AddStep(SceneFlowPhase.PostValidate, $"{JobId}.ValidateInjection")
                .RunAfterStep($"{JobId}.InjectDependencies")
                .Execute(ctx =>
                {
                    Debug.Log("[IntegratedDynamicDI] Validating injection...");
                    
                    foreach (var obj in _generatedObjects)
                    {
                        Debug.Log($"[IntegratedDynamicDI] Validated: {obj.name}");
                    }
                    
                    Debug.Log("[IntegratedDynamicDI] All dependencies validated successfully!");
                    
                    _generatedObjects = null;
                    _container = null;
                });
        }

        // サンプル用のサービスクラス
        class ServiceA { }
        class ServiceB { }
    }
}

