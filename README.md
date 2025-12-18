# SceneFlow

Unity の「ビルド時処理」を構造化・順序保証するための最小パイプラインフレームワーク

**SceneFlow = Build-time orchestration**

---

## 概要

SceneFlow は Unity のビルド前処理を Pass 単位で整理し、依存関係に基づいて実行順序を自動解決するフレームワークです。

- ✅ **Pass ベース**: 1 Pass = 1 責務
- ✅ **型安全な依存関係**: `RunAfter(typeof(OtherPass))`
- ✅ **自動順序解決**: トポロジカルソート
- ✅ **最小 API**: シンプルで学習コストが低い

詳細は [パッケージ内の README](./Packages/com.tplab.sceneflow/README.md) を参照してください。

---

## クイックスタート

### 基本的な Pass の実装

```csharp
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;

public class MyPass : IPass
{
    public void Execute(SceneFlowContext context)
    {
        // ビルド時処理を実装
        // context.Scene でシーンにアクセス
    }
}
```

### 依存関係の宣言

```csharp
public class InjectReferencePass : IPass
{
    // この Pass は CollectDataPass の「後」に実行される
    public IEnumerable<Type> RunAfter { get; } = new[] { typeof(CollectDataPass) };
    
    // 他のアセンブリの Pass には文字列で依存（循環参照回避）
    public IEnumerable<string> RunAfterNames { get; } = new[] { "OtherPackage.SomePass" };
    
    public void Execute(SceneFlowContext context)
    {
        // 参照を注入する処理
    }
}
```

### 複数の依存関係

```csharp
public class ValidatePass : IPass
{
    // 複数の Pass の後に実行
    public IEnumerable<Type> RunAfter { get; } = new[]
    {
        typeof(CollectDataPass),
        typeof(InjectReferencePass)
    };
    
    public void Execute(SceneFlowContext context)
    {
        // 検証処理
    }
}
```

ビルド時に自動実行されます。

詳細は [Pass 実装ガイド](./Packages/com.tplab.sceneflow/Documentation~/PassImplementationGuide.md) を参照してください。

---


