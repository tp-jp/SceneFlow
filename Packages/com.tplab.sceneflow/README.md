# SceneFlow

UnityのIProcessSceneWithReportはorderを指定できますが、同じorder内で順序を保証できません。SceneFlowは、SpringBatchのようなパイプライン設計により、シーン処理の順序を柔軟に制御するフレームワークです。

## ✨ 特徴

- **プラグインベース**: 機能を独立したプラグインとして実装
- **ステップ制御**: プラグイン内で複数のステップを定義し、ステップ単位で依存関係を指定
- **フェーズ管理**: Resolve, Generate, Transform, Optimizeの4つのフェーズで処理を整理
- **メソッドチェーン**: SpringBatch風の流暢なAPIでステップを構築
- **トポロジカルソート**: プラグイン間・ステップ間の依存関係を自動解決

## 🚀 使い方

### プラグインの作成

```csharp
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Plugin;
using UnityEngine;

public sealed class MyPlugin : ISceneFlowPlugin
{
    public string PluginId => "MyCompany.MyPlugin";
    public IEnumerable<string> RunAfterPlugins => null;
    public IEnumerable<string> RunBeforePlugins => null;

    private List<ValidationResult> _validationResults;

    public void ConfigureJob(ISceneFlowJobBuilder builder)
    {
        // Resolveフェーズで検証
        builder.AddStep(SceneFlowPhase.Resolve, "MyCompany.MyPlugin.Validate")
            .Execute((scene, report) =>
            {
                _validationResults = ValidateScene(scene);
                Debug.Log($"Validated: {_validationResults.Count} results");
            });

        // Transformフェーズでシーン変更
        builder.AddStep(SceneFlowPhase.Transform, "MyCompany.MyPlugin.Transform")
            .RunAfter("MyCompany.MyPlugin.Validate")
            .Execute((scene, report) =>
            {
                if (_validationResults?.All(r => r.IsValid) == true)
                {
                    ModifyScene(scene);
                    Debug.Log("Scene modified successfully");
                }
            });

        // Optimizeフェーズでクリーンアップ
        builder.AddStep(SceneFlowPhase.Optimize, "MyCompany.MyPlugin.Cleanup")
            .Execute((scene, report) =>
            {
                _validationResults = null;
                Debug.Log("Cleanup completed");
            });
    }

    private List<ValidationResult> ValidateScene(Scene scene) { /* ... */ }
    private void ModifyScene(Scene scene) { /* ... */ }
}
```

### プラグイン間の依存関係

```csharp
public sealed class MyAdvancedPlugin : ISceneFlowPlugin
{
    public string PluginId => "MyCompany.MyAdvancedPlugin";
    
    // このプラグインのステップは MyPlugin の後に実行される
    public IEnumerable<string> RunAfterPlugins => new[] { "MyCompany.MyPlugin" };
    
    public IEnumerable<string> RunBeforePlugins => null;

    public void ConfigureJob(ISceneFlowJobBuilder builder)
    {
        builder.AddStep(SceneFlowPhase.Transform, "MyCompany.MyAdvancedPlugin.Process")
            .Execute((scene, report) =>
            {
                // MyPluginの処理が完了した後に実行される
                Debug.Log("Advanced processing");
            });
    }
}
```

### ステップ間の依存関係

```csharp
public void ConfigureJob(ISceneFlowJobBuilder builder)
{
    // ステップA
    builder.AddStep(SceneFlowPhase.Generate, "MyPlugin.StepA")
        .Execute((scene, report) => { /* ... */ });

    // ステップB (StepAの後に実行)
    builder.AddStep(SceneFlowPhase.Generate, "MyPlugin.StepB")
        .RunAfter("MyPlugin.StepA")
        .Execute((scene, report) => { /* ... */ });

    // ステップC (StepAとStepBの後に実行)
    builder.AddStep(SceneFlowPhase.Generate, "MyPlugin.StepC")
        .RunAfter("MyPlugin.StepA", "MyPlugin.StepB")
        .Execute((scene, report) => { /* ... */ });

    // ステップD (StepCより前に実行)
    builder.AddStep(SceneFlowPhase.Generate, "MyPlugin.StepD")
        .RunBefore("MyPlugin.StepC")
        .Execute((scene, report) => { /* ... */ });
}
```

## 📋 フェーズ

SceneFlowは9つのフェーズで構成され、ビルド時の処理を体系的に整理できます。

| フェーズ | 値 | 説明 | ユースケース例 |
|---------|-----|------|-------------|
| Validate | 100 | シーンの整合性チェック、ビルド前の検証 | 必須コンポーネントの確認、参照の整合性チェック |
| Initialize | 200 | 後続処理に必要な環境やリソースの準備 | 設定の読み込み、サービスの初期化、コンテナのセットアップ |
| Resolve | 300 | 処理計画の立案、必要な要素の決定 | 生成すべきオブジェクトの決定、変換対象の選定、依存関係の解決 |
| Generate | 400 | 新しいオブジェクトやアセットの実際の生成 | GameObjectの生成、ScriptableObjectの作成、動的リソースの生成 |
| **Collect** | **450** | **処理対象となる全オブジェクトを収集・分析** | **既存+生成されたオブジェクトの収集、依存関係の分析、処理対象の特定** |
| Inject | 500 | オブジェクト間の関連付け、データの設定 | 依存関係の注入、参照の設定、プロパティの割り当て |
| Transform | 600 | シーンやオブジェクトの変換・加工 | プラットフォーム固有の変換、シェーダー差し替え、データフォーマット変換 |
| Optimize | 700 | パフォーマンス最適化、不要な要素の削除 | 静的バッチング、開発用コンポーネントの削除 |
| FinalValidate | 800 | 全処理完了後の最終チェック | ビルド結果の検証、必須要素の存在確認、整合性の最終確認 |

フェーズは数値順に実行され、各フェーズ内でステップがトポロジカルソートされます。

### フェーズ設計の理念

1. **Validate → Initialize → Resolve**: 準備段階
   - シーンの検証 → 環境のセットアップ → 処理計画の立案

2. **Generate → Collect → Inject → Transform**: 構築段階
   - オブジェクト生成 → **全オブジェクトの収集** → 関連付け → 変換処理

3. **Optimize → FinalValidate**: 仕上げ段階
   - 最適化 → 最終検証

### 🔑 Collectフェーズの重要性

**問題**: Generateフェーズで動的に生成されたオブジェクトは、Resolveフェーズでは存在しないため、後続処理の対象として認識されません。

**解決**: Collectフェーズ（Generate後に実行）で、既存オブジェクト + 生成されたオブジェクトを含む**全オブジェクト**を収集・解析します。

```
Initialize (環境準備)
  ↓
Resolve (処理計画)
  ↓
Generate (オブジェクト生成) ← ここで新しいオブジェクトが生まれる
  ↓
Collect (全オブジェクトの収集・分析) ← ★生成されたオブジェクトも含めて収集
  ↓
Inject (関連付け実行) ← すべてのオブジェクトが対象
```

## 🔧 アーキテクチャ

### 実行フロー

1. **プラグイン発見**: すべての`ISceneFlowPlugin`実装を自動検出
2. **プラグインソート**: プラグイン間の依存関係をトポロジカルソート
3. **ステップ収集**: 各プラグインから`ConfigureJob`でステップを収集
4. **依存関係適用**: プラグイン間の依存関係をステップに伝播
5. **フェーズ実行**: フェーズごとにステップをソートして実行

### 主要コンポーネント

- **ISceneFlowPlugin**: プラグインのインターフェース
- **ISceneFlowJobBuilder**: ジョブ（ステップの集合）を構築
- **ISceneFlowStepBuilder**: 個別のステップを構築（メソッドチェーン）
- **SceneFlowPipeline**: パイプライン全体の実行を管理
- **SceneFlowGraph**: トポロジカルソートによる依存関係解決

## 🎯 メリット

### 従来の問題点

```csharp
// 問題: フェーズごとにメソッドが分かれており、状態を保持しにくい
public interface ISceneFlowPlugin
{
    void OnValidate(Scene scene, BuildReport report);
    void OnPreProcess(Scene scene, BuildReport report);
    void OnProcess(Scene scene, BuildReport report);
    void OnPostProcess(Scene scene, BuildReport report);
}
```

### SceneFlowの解決策

```csharp
// 解決: プラグイン内で状態を保持しながら、複数フェーズにまたがる処理が可能
public void ConfigureJob(ISceneFlowJobBuilder builder)
{
    builder.AddStep(SceneFlowPhase.Validate, "Validate")
        .Execute((scene, report) => _data = Validate(scene));

    builder.AddStep(SceneFlowPhase.Transform, "Process")
        .RunAfter("Validate")
        .Execute((scene, report) => Process(scene, _data));

    builder.AddStep(SceneFlowPhase.FinalValidate, "Cleanup")
        .Execute((scene, report) => _data = null);
}
```

## 💼 実践的なユースケース

### ユースケース1: 動的オブジェクト生成

ビルド時に必要なGameObjectを動的に生成する例：

```csharp
public class DynamicObjectPlugin : ISceneFlowPlugin
{
    private List<string> _objectsToGenerate;
    private List<GameObject> _generatedObjects;

    public void ConfigureJob(ISceneFlowJobBuilder builder)
    {
        // 1. どのオブジェクトを生成するか決定
        builder.AddStep(SceneFlowPhase.Resolve, "PlanGeneration")
            .Execute((scene, _) =>
            {
                _objectsToGenerate = DetermineDynamicObjects(scene);
            });

        // 2. 実際にオブジェクトを生成
        builder.AddStep(SceneFlowPhase.Generate, "GenerateObjects")
            .RunAfter("PlanGeneration")
            .Execute((scene, _) =>
            {
                _generatedObjects = CreateObjects(_objectsToGenerate, scene);
            });

        // 3. 生成したオブジェクトを設定
        builder.AddStep(SceneFlowPhase.Transform, "ConfigureObjects")
            .RunAfter("GenerateObjects")
            .Execute((_, __) =>
            {
                ConfigureGeneratedObjects(_generatedObjects);
            });

        // 4. 生成結果を検証
        builder.AddStep(SceneFlowPhase.FinalValidate, "ValidateGeneration")
            .Execute((scene, _) =>
            {
                ValidateAllObjectsGenerated(scene, _objectsToGenerate);
            });
    }
}
```

### ユースケース2: DIコンテナによるインジェクション

VContainerやZenjectなどのDIコンテナを使用する例：

```csharp
public class DIContainerPlugin : ISceneFlowPlugin
{
    private IContainer _container;
    private List<MonoBehaviour> _injectableComponents;

    public void ConfigureJob(ISceneFlowJobBuilder builder)
    {
        // 1. DIコンテナを構築
        builder.AddStep(SceneFlowPhase.Initialize, "SetupContainer")
            .Execute((_, __) =>
            {
                _container = new Container();
                RegisterServices(_container);
            });

        // 2. インジェクション対象を収集（Generate後に実行）
        builder.AddStep(SceneFlowPhase.Collect, "AnalyzeDependencies")
            .RunAfter("SetupContainer")
            .Execute((scene, _) =>
            {
                // 既存 + 動的生成されたオブジェクトを含む全オブジェクトから収集
                _injectableComponents = FindInjectableComponents(scene);
            });

        // 3. 依存関係を注入
        builder.AddStep(SceneFlowPhase.Inject, "InjectDependencies")
            .RunAfter("AnalyzeDependencies")
            .Execute((_, __) =>
            {
                _container.Inject(_injectableComponents);
            });

        // 4. インジェクション結果を検証
        builder.AddStep(SceneFlowPhase.FinalValidate, "ValidateInjection")
            .Execute((_, __) =>
            {
                ValidateAllDependenciesInjected(_injectableComponents);
            });
    }
}
```

### ユースケース3: 動的生成 + DIインジェクション（統合）

Generateで生成したオブジェクトにDIで依存関係を注入する例：

```csharp
public class IntegratedPlugin : ISceneFlowPlugin
{
    private IContainer _container;
    private List<GameObject> _generatedObjects;

    public void ConfigureJob(ISceneFlowJobBuilder builder)
    {
        // 1. DIコンテナをセットアップ
        builder.AddStep(SceneFlowPhase.Initialize, "SetupDI")
            .Execute((_, __) =>
            {
                _container = new Container();
                _container.Register<IService, ServiceImpl>();
            });

        // 2. 動的オブジェクトを生成
        builder.AddStep(SceneFlowPhase.Generate, "GenerateObjects")
            .Execute((scene, _) =>
            {
                var obj = new GameObject("DynamicManager");
                obj.AddComponent<InjectableComponent>(); // DIが必要なコンポーネント
                _generatedObjects.Add(obj);
            });

        // 3. 生成されたオブジェクトを含めて収集
        // ★ Collectフェーズを使用することで、Generateで生成されたオブジェクトも対象になる
        builder.AddStep(SceneFlowPhase.Collect, "CollectAll")
            .RunAfter("GenerateObjects")
            .Execute((scene, _) =>
            {
                // 既存 + 生成されたオブジェクトの両方から収集
                var allInjectables = FindAllInjectableComponents(scene);
            });

        // 4. すべてのオブジェクトに注入
        builder.AddStep(SceneFlowPhase.Inject, "InjectAll")
            .RunAfter("CollectAll")
            .Execute((_, __) =>
            {
                // 生成されたオブジェクトにも注入される！
                _container.InjectAll();
            });
    }
}
```

### ユースケース4: 複数プラグインの連携

DIコンテナプラグインの後に、生成プラグインを実行する例：

```csharp
public class AdvancedGenerationPlugin : ISceneFlowPlugin
{
    public string PluginId => "AdvancedGeneration";
    
    // DIコンテナプラグインの後に実行
    public IEnumerable<string> RunAfterPlugins => new[] { "DIContainer" };

    public void ConfigureJob(ISceneFlowJobBuilder builder)
    {
        // DIコンテナが準備された後に、動的オブジェクトを生成
        builder.AddStep(SceneFlowPhase.Generate, "GenerateWithDI")
            .Execute((scene, _) =>
            {
                // DIコンテナから取得したサービスを使用して生成
                var factory = Container.Resolve<IObjectFactory>();
                factory.CreateObjects(scene);
            });
    }
}
```

## 📝 ライセンス

MIT License

## 🤝 コントリビューション

Issue、Pull Requestを歓迎します。

