# SceneFlow
Unity の「ビルド時処理」を構造化・順序保証するための最小パイプラインフレームワーク
**SceneFlow = Build-time orchestration**  
**DI コンテナではない**
---
## 概要
SceneFlow は Unity のビルド前処理を Pass 単位で整理し、依存関係に基づいて実行順序を自動解決するフレームワークです。
### 解決する問題
Unity（特に VRChat + Udon）では以下が頻発します：
- ビルド前に「必ずやりたい処理」が多数ある
- 処理同士に**暗黙の依存順**がある
- `OnPreprocessBuild` に処理が散らばる
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
- 他 Pass との依存は**宣言的**に表現
### Pass の種類（フェーズ）
Pass の違いは**処理対象のスコープ**です。
```
IBuildPass
  └─ IProjectPass
        └─ IScenePass
```
#### IBuildPass
ビルド全体で一度だけ実行
**例:**
- 環境検証
- 設定ファイル生成
- 事前準備 / 後処理
#### IProjectPass
プロジェクト全体に対する処理
**例:**
- ScriptableObject 生成
- 共通アセットの更新
- キャッシュ構築
#### IScenePass
シーン単位の処理
**VRChat では：** 実質「ワールド = 1 シーン」という意味合いが強い
---
## クイックスタート
### 基本的な Pass の実装
```csharp
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEngine;
public class MyScenePass : IScenePass
{
    public void Execute(SceneFlowContext context)
    {
        // シーン処理を実装
        var scene = context.Scene;
        Debug.Log($"Processing scene: {scene.name}");
    }
}
```
### 依存関係を持つ Pass（同じアセンブリ内）
```csharp
using System;
using System.Collections.Generic;
public class CollectDataPass : IScenePass
{
    public void Execute(SceneFlowContext context)
    {
        Debug.Log("データを収集");
    }
}
public class ProcessDataPass : IScenePass
{
    // 同じアセンブリ内の Pass なら型参照（タイプセーフ）
    public IEnumerable<Type> RunAfter
    {
        get { yield return typeof(CollectDataPass); }
    }
    public void Execute(SceneFlowContext context)
    {
        Debug.Log("データを処理");
    }
}
```
### 他のアセンブリの Pass に依存する場合
**アセンブリ循環参照を避けるため、文字列ベースの参照を使用してください。**
```csharp
public class MyPass : IScenePass
{
    // ? 他のアセンブリの Pass を型参照すると循環参照の危険性
    // public IEnumerable<Type> RunAfter => new[] { typeof(OtherAssembly.SomePass) };
    // ? 文字列参照を使用（アセンブリ参照不要）
    public IEnumerable<string> RunAfterNames
    {
        get
        {
            // 完全修飾名で指定
            yield return "OtherNamespace.SomePass";
            // アセンブリ名も含めることも可能
            yield return "OtherNamespace.SomePass, OtherAssembly";
        }
    }
    public void Execute(SceneFlowContext context)
    {
        Debug.Log("処理");
    }
}
```
**使い分けの指針:**
- **同じアセンブリ内**: `RunAfter`/`RunBefore` を使用（タイプセーフ）
- **他のアセンブリ**: `RunAfterNames`/`RunBeforeNames` を使用（循環参照回避）
---
## 実行順序
### 基本順序（固定）
```
1. IBuildPass
2. IProjectPass
3. IScenePass
```
### 同じ種別内の順序
`PassSorter` が依存関係に基づいてトポロジカルソートを実行
- `RunAfter(typeof(OtherPass))`: 指定 Pass の**後**に実行
- `RunBefore(typeof(OtherPass))`: 指定 Pass の**前**に実行
- `RunAfterNames`: 文字列で指定（他アセンブリ参照時）
- `RunBeforeNames`: 文字列で指定（他アセンブリ参照時）
---
## 重要な設計方針
### ? アセンブリ循環参照に注意
**他のアセンブリの Pass に依存する場合は、文字列ベースの参照を使用してください。**
```csharp
// ? アセンブリ循環参照の危険性
public IEnumerable<Type> RunAfter => new[] { typeof(OtherAssembly.Pass) };
// ? 文字列参照で循環参照を回避
public IEnumerable<string> RunAfterNames => new[] { "OtherNamespace.Pass" };
```
**理由:**
- Assembly A が Assembly B の Pass を `typeof()` で参照 → A が B を参照
- Assembly B が Assembly A の Pass を `typeof()` で参照 → B が A を参照
- **循環参照エラー**
文字列参照を使うことで、アセンブリ間の直接的な依存を避けられます。
---
## API リファレンス
### SceneFlowContext
Pass の実行時に渡されるコンテキスト
```csharp
public class SceneFlowContext
{
    /// <summary>
    /// 処理対象のシーン（IScenePass のみ有効）
    /// </summary>
    public Scene Scene { get; }
}
```
### IBuildPass
```csharp
public interface IBuildPass
{
    IEnumerable<Type> RunAfter => Array.Empty<Type>();
    IEnumerable<Type> RunBefore => Array.Empty<Type>();
    IEnumerable<string> RunAfterNames => Array.Empty<string>();
    IEnumerable<string> RunBeforeNames => Array.Empty<string>();
    void Execute(SceneFlowContext context);
}
```
### IProjectPass
```csharp
public interface IProjectPass
{
    IEnumerable<Type> RunAfter => Array.Empty<Type>();
    IEnumerable<Type> RunBefore => Array.Empty<Type>();
    IEnumerable<string> RunAfterNames => Array.Empty<string>();
    IEnumerable<string> RunBeforeNames => Array.Empty<string>();
    void Execute(SceneFlowContext context);
}
```
### IScenePass
```csharp
public interface IScenePass
{
    IEnumerable<Type> RunAfter => Array.Empty<Type>();
    IEnumerable<Type> RunBefore => Array.Empty<Type>();
    IEnumerable<string> RunAfterNames => Array.Empty<string>();
    IEnumerable<string> RunBeforeNames => Array.Empty<string>();
    void Execute(SceneFlowContext context);
}
```
---
## サンプル
詳細なサンプルは `Samples~/Sample/Editor/SamplePasses.cs` を参照してください。
---
## ライセンス
MIT License
