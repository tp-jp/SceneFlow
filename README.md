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

- `RunAfter(typeof(OtherPass))`: 指定 Pass の**後**に実行
- `RunBefore(typeof(OtherPass))`: 指定 Pass の**前**に実行
- `RunAfterNames`: 文字列で指定（他アセンブリ参照時）
- `RunBeforeNames`: 文字列で指定（他アセンブリ参照時）

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

### 依存関係を持つ Pass（同一アセンブリ内）

```csharp
using System;
using System.Collections.Generic;

public class CollectDataPass : IPass
{
    public void Execute(SceneFlowContext context)
    {
        Debug.Log("データ収集");
    }
}

public class ProcessDataPass : IPass
{
    // 同一アセンブリ内の Pass なら型参照（タイプセーフ）
    public IEnumerable<Type> RunAfter { get; } = new[] { typeof(CollectDataPass) };

    public void Execute(SceneFlowContext context)
    {
        Debug.Log("データ処理");
    }
}
```

### 他のアセンブリの Pass に依存する場合

**アセンブリ循環参照を避けるため、文字列ベースの参照を使用してください。**

```csharp
public class MyPass : IPass
{
    // ❌ 他のアセンブリの Pass を型参照すると循環参照の危険性
    // public IEnumerable<Type> RunAfter => new[] { typeof(OtherAssembly.SomePass) };

    // ✅ 文字列参照を使用（アセンブリ循環参照回避）
    public IEnumerable<string> RunAfterNames { get; } = new[]
    {
        // 完全修飾名で指定
        "OtherNamespace.SomePass",
        // アセンブリ名を含めることも可能
        "OtherNamespace.AnotherPass, OtherAssembly.Editor"
    };

    public void Execute(SceneFlowContext context)
    {
        Debug.Log("処理");
    }
}
```

**使い分けの指針:**

- **同一アセンブリ内**: `RunAfter`/`RunBefore` を使用（タイプセーフ）
- **他のアセンブリ**: `RunAfterNames`/`RunBeforeNames` を使用（循環回避）

---

## 重要な設計思想

### ⚠️ アセンブリ循環参照に注意

**他のアセンブリの Pass に依存する場合は、文字列ベースの参照を使用してください。**

```csharp
// ❌ アセンブリ循環参照の危険性
public IEnumerable<Type> RunAfter => new[] { typeof(OtherAssembly.Pass) };

// ✅ 文字列参照で循環参照を回避
public IEnumerable<string> RunAfterNames => new[] { "OtherNamespace.Pass" };
```

**理由:**

- Assembly A が Assembly B の Pass を `typeof()` で参照 → A が B を参照
- Assembly B が Assembly A の Pass を `typeof()` で参照 → B が A を参照
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
    /// この Pass より「後」に実行されるべき Pass 型（同一アセンブリ内推奨）
    /// </summary>
    IEnumerable<Type> RunAfter => Array.Empty<Type>();

    /// <summary>
    /// この Pass より「前」に実行されるべき Pass 型（同一アセンブリ内推奨）
    /// </summary>
    IEnumerable<Type> RunBefore => Array.Empty<Type>();

    /// <summary>
    /// この Pass より「後」に実行されるべき Pass の型名（他アセンブリ参照時に使用）
    /// </summary>
    IEnumerable<string> RunAfterNames => Array.Empty<string>();

    /// <summary>
    /// この Pass より「前」に実行されるべき Pass の型名（他アセンブリ参照時に使用）
    /// </summary>
    IEnumerable<string> RunBeforeNames => Array.Empty<string>();

    /// <summary>
    /// Pass 処理を実行する
    /// </summary>
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
