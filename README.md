# SceneFlow

Unity の「ビルド時処理」を構造化・順序保証するための最小パイプラインフレームワーク

**SceneFlow = Build-time orchestration**

---

## 概要

SceneFlow は Unity のビルド前処理を Pass 単位で整理し、依存関係に基づいて実行順序を自動解決するフレームワークです。

### 解決したい問題

Unity（特に VRChat + Udon）では以下が頻発します：

- ビルド前に「必ずやりたい処理」が多数ある
- 処理同士に **暗黙の依存順** がある
- `OnProcessSceneWithReport` に処理が散らばる
- 実行順序がコメントと人間の記憶に依存する

SceneFlow はこれを：

- **Pass 単位**に分離
- **依存関係**で順序解決
- **最小限の API**

で整理します。

---

## 基本コンセプト

### Pass（最小単位）

SceneFlow における唯一の拡張単位。

- 1 Pass = 1 責務
- 状態を持たない（原則）
- 他 Pass との依存は **宣言的** に表現

### 実行順序の制御

**依存関係ベース**で順序を制御：

推奨される API:
- `DependencyBuilder.Create().After<T>()`: 指定 Pass の**後**に実行（Type参照）
- `DependencyBuilder.Create().Before<T>()`: 指定 Pass の**前**に実行（Type参照）
- `DependencyBuilder.Create().After(string)`: 文字列で指定（他アセンブリ参照時）
- `DependencyBuilder.Create().Before(string)`: 文字列で指定（他アセンブリ参照時）

従来の方法も使用可能:
- `PassDependency.After<T>()` / `PassDependency.Before<T>()`
- `PassDependency.After(string)` / `PassDependency.Before(string)`

`PassSorter` が依存関係に基づいてトポロジカルソートを実行します。

---

## クイックスタート

### 基本的な Pass の実装

```csharp
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEngine;

public class MyPass : IPass
{
    public void Execute(SceneFlowContext context)
    {
        // ビルド時処理を実装
        var scene = context.Scene;
        Debug.Log($"Processing scene: {scene.name}");
    }
}
```

### 依存関係を持つ Pass

```csharp
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Pass;

public class CollectDataPass : IPass
{
    public void Execute(SceneFlowContext context)
    {
        Debug.Log("データ収集");
    }
}

public class ProcessDataPass : IPass
{
    // DependencyBuilder を使用（推奨）
    public IEnumerable<PassDependency> Dependencies => DependencyBuilder
        .Create()
        .After<CollectDataPass>()  // CollectDataPass の後に実行
        .Build();

    public void Execute(SceneFlowContext context)
    {
        Debug.Log("データ処理");
    }
}
```

### DependencyBuilder（推奨）

流暢なメソッドチェーンで依存関係を宣言できます：

```csharp
public class ComplexPass : IPass
{
    public IEnumerable<PassDependency> Dependencies => DependencyBuilder
        .Create()
        .After<FirstPass>()
        .After<SecondPass>()
        .Before<FinalPass>()
        .After("OtherPackage.ThirdPartyPass, OtherPackage.Editor")  // 他アセンブリ
        .Build();

    public void Execute(SceneFlowContext context)
    {
        Debug.Log("複雑な依存関係を持つ処理");
    }
}
```

**利点:**
- 読みやすく、明確な意図表現
- Type安全性と柔軟性の両立
- メソッドチェーンで流暢に記述

**従来の配列形式も使用可能:**

```csharp
public IEnumerable<PassDependency> Dependencies => new[]
{
    PassDependency.After<FirstPass>(),
    PassDependency.Before<FinalPass>()
};
```

### 他のアセンブリの Pass に依存する場合

**アセンブリ循環参照を避けるため、文字列ベースの参照を使用してください。**

```csharp
public class MyPass : IPass
{
    // ❌ 他のアセンブリの Pass を型参照すると循環参照の危険性
    // public IEnumerable<PassDependency> Dependencies => DependencyBuilder
    //     .Create()
    //     .After<OtherAssembly.SomePass>()
    //     .Build();

    // ✅ 文字列参照を使用（アセンブリ循環参照回避）
    public IEnumerable<PassDependency> Dependencies => DependencyBuilder
        .Create()
        // 完全修飾名で指定
        .After("OtherNamespace.SomePass")
        // アセンブリ名を含めることも可能（推奨）
        .After("OtherNamespace.AnotherPass, OtherAssembly.Editor")
        .Build();

    public void Execute(SceneFlowContext context)
    {
        Debug.Log("処理");
    }
}
```

**使い分けの指針:**

- **同一アセンブリ内**: `After<T>()` / `Before<T>()` を使用（タイプセーフ）
- **他のアセンブリ**: `After(string)` / `Before(string)` を使用（循環回避）

---

## 重要な設計思想

### ⚠️ アセンブリ循環参照に注意

**他のアセンブリの Pass に依存する場合は、文字列ベースの参照を使用してください。**

```csharp
// ❌ アセンブリ循環参照の危険性
public IEnumerable<PassDependency> Dependencies => DependencyBuilder
    .Create()
    .After<OtherAssembly.Pass>()
    .Build();

// ✅ 文字列参照で循環参照を回避
public IEnumerable<PassDependency> Dependencies => DependencyBuilder
    .Create()
    .After("OtherNamespace.Pass, OtherAssembly.Editor")
    .Build();
```

**理由:**

- Assembly A が Assembly B の Pass を `After<T>()` で参照 → A が B を参照
- Assembly B が Assembly A の Pass を `After<T>()` で参照 → B が A を参照
- **循環参照エラー**

文字列参照を使うことで、アセンブリ間の直接的な依存を回避できます。

---

## API リファレンス

### SceneFlowContext

Pass の実行時に渡されるコンテキスト

```csharp
public class SceneFlowContext
{
    /// <summary>
    /// 処理対象のシーン
    /// </summary>
    public Scene Scene { get; }
}
```

### IPass

```csharp
public interface IPass
{
    /// <summary>
    /// Pass の依存関係（推奨）
    /// </summary>
    IEnumerable<PassDependency> Dependencies => Array.Empty<PassDependency>();


    /// <summary>
    /// Pass 処理を実行する
    /// </summary>
    void Execute(SceneFlowContext context);
}
```

### DependencyBuilder

Pass の依存関係を流暢に定義するための Builder

```csharp
public sealed class DependencyBuilder
{
    /// <summary>新しい Builder インスタンスを作成</summary>
    public static DependencyBuilder Create();
    
    /// <summary>指定された Pass の後に実行（Type 参照）</summary>
    public DependencyBuilder After<T>() where T : IPass;
    public DependencyBuilder After(Type passType);
    
    /// <summary>指定された Pass の後に実行（文字列参照）</summary>
    public DependencyBuilder After(string passTypeName);
    
    /// <summary>指定された Pass の前に実行（Type 参照）</summary>
    public DependencyBuilder Before<T>() where T : IPass;
    public DependencyBuilder Before(Type passType);
    
    /// <summary>指定された Pass の前に実行（文字列参照）</summary>
    public DependencyBuilder Before(string passTypeName);
    
    /// <summary>依存関係のコレクションを構築</summary>
    public IEnumerable<PassDependency> Build();
}
```

### PassDependency

Pass の依存関係を表すクラス

```csharp
public sealed class PassDependency
{
    /// <summary>指定された Pass の後に実行（Type 参照）</summary>
    public static PassDependency After<T>() where T : IPass;
    public static PassDependency After(Type passType);
    public static PassDependency After(string passTypeName);
    
    /// <summary>指定された Pass の前に実行（Type 参照）</summary>
    public static PassDependency Before<T>() where T : IPass;
    public static PassDependency Before(Type passType);
    public static PassDependency Before(string passTypeName);
}
```

### PassDependency

```csharp
public sealed class PassDependency
{
    // Type 参照（同一アセンブリ内推奨）
    public static PassDependency After<T>() where T : IPass;
    public static PassDependency Before<T>() where T : IPass;
    
    // 文字列参照（他アセンブリ参照時）
    public static PassDependency After(string passTypeName);
    public static PassDependency Before(string passTypeName);
}
```

**使用例:**

```csharp
public IEnumerable<PassDependency> Dependencies => new[]
{
    PassDependency.After<SomePass>(),  // Type参照
    PassDependency.After("Other.Pass, OtherAssembly"),  // 文字列参照
    PassDependency.Before<AnotherPass>()  // Before指定
};
```
    void Execute(SceneFlowContext context);
}
```

---

## デバッグウィンドウ

SceneFlow には Pass の一覧と実行順序を確認するためのデバッグウィンドウが付属しています。

**開き方**: `Tools > SceneFlow > Debug Window`

### 機能

- ✅ Pass の実行順序を一覧表示
- ✅ 依存関係の確認（クリックで展開）
- ✅ 検索フィルター
- ✅ Expand All / Collapse All

---

## サンプル

詳細なサンプルは `Packages/com.tplab.sceneflow/Samples~/Sample/Editor/SamplePasses.cs` を参照してください。

---

## ライセンス

MIT License
