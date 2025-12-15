using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Job;
using UnityEngine;

namespace Samples.Sample.Editor
{
    /// <summary>
    /// DIコンテナインジェクションのサンプルジョブ
    /// ビルド時にDIコンテナを使用して依存関係を注入するケースのデモ
    /// </summary>
    public sealed class DIContainerJob : ISceneFlowJob
    {
        public string JobId => typeof(DIContainerJob).FullName;

        // 簡易的なDIコンテナ（実際はVContainerやZenjectなどを使用）
        Dictionary<string, object> _container;
        
        // インジェクション対象のコンポーネント
        List<MonoBehaviour> _injectableComponents;

        public void Configure(ISceneFlowJobBuilder builder)
        {
            // Setupフェーズ: DIコンテナを構築
            builder.AddStep(SceneFlowPhase.Setup, $"{JobId}.SetupContainer")
                .Execute(ctx =>
                {
                    Debug.Log("[DIContainer] Setting up DI container...");
                    
                    _container = new Dictionary<string, object>();
                    
                    // サービスを登録
                    _container["ILogger"] = new UnityLogger();
                    _container["IConfigService"] = new ConfigService();
                    
                    Debug.Log($"[DIContainer] Registered {_container.Count} services");
                });

            // Processフェーズ: Build後に全オブジェクトの依存関係を解析・注入
            builder.AddStep(SceneFlowPhase.Process, $"{JobId}.AnalyzeAndInject")
                .RunAfterStep($"{JobId}.SetupContainer")
                .Execute(ctx =>
                {
                    Debug.Log("[DIContainer] Analyzing and injecting dependencies...");
                    
                    _injectableComponents = new List<MonoBehaviour>();
                    
                    // シーン内の全MonoBehaviourを走査（既存 + 動的生成されたオブジェクトを含む）
                    foreach (var rootObj in ctx.Scene.GetRootGameObjects())
                    {
                        foreach (var component in rootObj.GetComponentsInChildren<MonoBehaviour>(true))
                        {
                            if (HasInjectableFields(component))
                            {
                                _injectableComponents.Add(component);
                                // 実際のインジェクション処理
                            }
                        }
                    }
                    
                    Debug.Log($"[DIContainer] Injected into {_injectableComponents.Count} components");
                });

            // PostValidateフェーズ: インジェクション結果を検証
            builder.AddStep(SceneFlowPhase.PostValidate, $"{JobId}.ValidateInjection")
                .RunAfterStep($"{JobId}.AnalyzeAndInject")
                .Execute(ctx =>
                {
                    Debug.Log("[DIContainer] Validating injection result...");
                    
                    var validCount = 0;
                    foreach (var component in _injectableComponents)
                    {
                        if (ValidateComponentDependencies(component))
                        {
                            validCount++;
                        }
                    }
                    
                    Debug.Log($"[DIContainer] Validated {validCount}/{_injectableComponents.Count} components");
                    
                    // クリーンアップ
                    _container = null;
                    _injectableComponents = null;
                });
        }

        bool HasInjectableFields(MonoBehaviour component)
        {
            // 簡易実装。実際は[Inject]属性をチェック
            return component != null;
        }

        bool ValidateComponentDependencies(MonoBehaviour component)
        {
            // 簡易実装。実際は全フィールドがnullでないことをチェック
            return component != null;
        }

        // サンプル用の簡易クラス
        class UnityLogger { }
        class ConfigService { }
    }
}

