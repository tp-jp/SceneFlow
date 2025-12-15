using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Job;
using UnityEngine;

namespace Samples.Sample.Editor
{
    /// <summary>
    /// 動的オブジェクト生成のサンプルジョブ
    /// ビルド時に必要なGameObjectを動的に生成するケースのデモ
    /// </summary>
    public sealed class DynamicObjectGenerationJob : ISceneFlowJob
    {
        public string JobId => typeof(DynamicObjectGenerationJob).FullName;

        // 生成計画
        List<string> _objectsToGenerate;
        
        // 生成されたオブジェクト
        List<GameObject> _generatedObjects;

        public void Configure(ISceneFlowJobBuilder builder)
        {
            // PreValidateフェーズ: シーンの構造を検証
            builder.AddStep(SceneFlowPhase.PreValidate, $"{JobId}.ValidateSceneStructure")
                .Execute(ctx =>
                {
                    Debug.Log("[DynamicObjectGeneration] Validating scene structure...");
                    
                    // シーンに必要な要素があるか確認
                    var rootObjects = ctx.Scene.GetRootGameObjects();
                    Debug.Log($"[DynamicObjectGeneration] Found {rootObjects.Length} root objects");
                });

            // Setupフェーズ: どのオブジェクトを生成するか決定
            builder.AddStep(SceneFlowPhase.Setup, $"{JobId}.PlanGeneration")
                .Execute(ctx =>
                {
                    Debug.Log("[DynamicObjectGeneration] Planning object generation...");
                    
                    _objectsToGenerate = new List<string>();
                    
                    // 条件に基づいて生成するオブジェクトを決定
                    // 例: ビルドターゲット、プラットフォーム、設定などから判断
                    _objectsToGenerate.Add("DynamicManager");
                    _objectsToGenerate.Add("RuntimeConfiguration");
                    
                    Debug.Log($"[DynamicObjectGeneration] Planned to generate {_objectsToGenerate.Count} objects");
                });

            // Buildフェーズ: 実際にオブジェクトを生成
            builder.AddStep(SceneFlowPhase.Build, $"{JobId}.GenerateObjects")
                .RunAfterStep($"{JobId}.PlanGeneration")
                .Execute(ctx =>
                {
                    Debug.Log("[DynamicObjectGeneration] Generating objects...");
                    
                    _generatedObjects = new List<GameObject>();
                    
                    foreach (var objName in _objectsToGenerate)
                    {
                        var obj = new GameObject(objName);
                        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, ctx.Scene);
                        _generatedObjects.Add(obj);
                        
                        Debug.Log($"[DynamicObjectGeneration] Generated: {objName}");
                    }
                });

            // Processフェーズ: 生成したオブジェクトを設定
            builder.AddStep(SceneFlowPhase.Process, $"{JobId}.ConfigureGeneratedObjects")
                .RunAfterStep($"{JobId}.GenerateObjects")
                .Execute(ctx =>
                {
                    Debug.Log("[DynamicObjectGeneration] Configuring generated objects...");
                    
                    foreach (var obj in _generatedObjects)
                    {
                        // オブジェクトにコンポーネントを追加したり、プロパティを設定
                        obj.tag = "Generated";
                    }
                });

            // PostValidateフェーズ: 生成結果を検証
            builder.AddStep(SceneFlowPhase.PostValidate, $"{JobId}.ValidateGeneration")
                .RunAfterStep($"{JobId}.ConfigureGeneratedObjects")
                .Execute(ctx =>
                {
                    Debug.Log("[DynamicObjectGeneration] Validating generation result...");
                    
                    // 全てのオブジェクトが正しく生成されたか確認
                    var generatedCount = 0;
                    foreach (var obj in ctx.Scene.GetRootGameObjects())
                    {
                        if (obj.tag == "Generated")
                        {
                            generatedCount++;
                        }
                    }
                    
                    Debug.Log($"[DynamicObjectGeneration] Verified {generatedCount} generated objects");
                    
                    // クリーンアップ
                    _objectsToGenerate = null;
                    _generatedObjects = null;
                });
        }
    }
}

