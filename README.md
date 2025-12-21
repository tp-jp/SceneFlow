# SceneFlow

Unity の「ビルド時処理」を構造化・順序保証するための最小パイプラインフレームワーク

## 概要

SceneFlow は Unity のビルド前処理を Pass 単位で整理し、依存関係に基づいて実行順序を自動解決するフレームワークです。

### 解決したい問題

Unityでは以下が頻発します：

- ビルド前に「必ずやりたい処理」が多数ある
- 処理同士に **暗黙の依存順** がある
- `OnProcessSceneWithReport` に処理が散らばる
- 実行順序がコメントと人間の記憶に依存する

SceneFlow はこれを：

- **Pass 単位**に分離
- **依存関係**で順序解決
- **最小限の API**

で整理します。

## 基本コンセプト

### Pass（最小単位）

SceneFlow における唯一の拡張単位。

- 1 Pass = 1 責務
- 状態を持たない（原則）
- 他 Pass との依存は **宣言的** に表現

### 実行順序の制御

**依存関係ベース**で順序を制御：

- `builder.After<T>()`: 指定 Pass の**後**に実行（Type参照）
- `builder.Before<T>()`: 指定 Pass の**前**に実行（Type参照）
- `builder.After(string)`: 文字列で指定（他アセンブリ参照時）
- `builder.Before(string)`: 文字列で指定（他アセンブリ参照時）

`PassSorter` が依存関係に基づいてトポロジカルソートを実行します。

## クイックスタート

### 基本的な Pass の実装

```csharp
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEngine;

public class MyPass : IPass
{
    public override void Execute(SceneFlowContext context)
    {
        // ビルド時処理を実装
        var scene = context.Scene;
        Debug.Log($"Processing scene: {scene.name}");
    }
}
```

### 依存関係を持つ Pass

```csharp
using TpLab.SceneFlow.Editor.Pass;

public class CollectDataPass : IPass
{
    public override void Execute(SceneFlowContext context)
    {
        Debug.Log("データ収集");
    }
}

public class ProcessDataPass : IPass
{
    // ConfigureDependencies で依存関係を設定
    protected override void ConfigureDependencies(DependencyBuilder builder)
    {
        builder.After<CollectDataPass>();  // CollectDataPass の後に実行
    }

    public override void Execute(SceneFlowContext context)
    {
        Debug.Log("データ処理");
    }
}
```

### 複雑な依存関係

メソッドチェーンで複数の依存関係を宣言できます：

```csharp
public class ComplexPass : IPass
{
    protected override void ConfigureDependencies(DependencyBuilder builder)
    {
        builder
            .After<FirstPass>()
            .After<SecondPass>()
            .Before<FinalPass>()
            .After("OtherPackage.ThirdPartyPass, OtherPackage.Editor");  // 他アセンブリ
    }

    public override void Execute(SceneFlowContext context)
    {
        Debug.Log("複雑な依存関係を持つ処理");
    }
}
```

### 他のアセンブリの Pass に依存する場合

**アセンブリ循環参照を避けるため、文字列ベースの参照を使用してください。**

```csharp
public class MyPass : IPass
{
    // ❌ 他のアセンブリの Pass を型参照すると循環参照の危険性
    // protected override void ConfigureDependencies(DependencyBuilder builder)
    // {
    //     builder.After<OtherAssembly.SomePass>();
    // }

    // ✅ 文字列参照を使用（アセンブリ循環参照回避）
    protected override void ConfigureDependencies(DependencyBuilder builder)
    {
        builder
            .After("OtherNamespace.SomePass")  // 完全修飾名で指定
            .After("OtherNamespace.AnotherPass, OtherAssembly.Editor");  // アセンブリ名含む（推奨）
    }

    public override void Execute(SceneFlowContext context)
    {
        Debug.Log("処理");
    }
}
```

**使い分けの指針:**

- **同一アセンブリ内**: `After<T>()` / `Before<T>()` を使用（タイプセーフ）
- **他のアセンブリ**: `After(string)` / `Before(string)` を使用（循環回避）

## 重要な設計思想

### ⚠️ アセンブリ循環参照に注意

**他のアセンブリの Pass に依存する場合は、文字列ベースの参照を使用してください。**

**理由:**

- Assembly A が Assembly B の Pass を `After<T>()` で参照 → A が B を参照
- Assembly B が Assembly A の Pass を `After<T>()` で参照 → B が A を参照
- **循環参照エラー**

文字列参照を使うことで、アセンブリ間の直接的な依存を回避できます。

## デバッグウィンドウ

SceneFlow には Pass の一覧と実行順序を確認するためのデバッグウィンドウが付属しています。

**開き方**: `Tools > SceneFlow > Debug Window`

### 機能

- ✅ Pass の実行順序を一覧表示
- ✅ 依存関係の確認
- ✅ 検索フィルター
- ✅ Expand All / Collapse All

## サンプル

詳細なサンプルは `Packages/com.tplab.sceneflow/Samples~/Sample/Editor/SamplePasses.cs` を参照してください。

## 変更履歴

このプロジェクトの変更履歴については、[CHANGELOG.md](CHANGELOG.md) を参照してください。

## ライセンス

MIT License - 詳細は [LICENSE](LICENSE) を参照してください。

Copyright (c) 2025 tp.jp
