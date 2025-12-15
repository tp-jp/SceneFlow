namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// シーンフローのフェーズ
    /// ビルドパイプラインの各段階を表す
    /// </summary>
    public enum SceneFlowPhase
    {
        /// <summary>
        /// 事前検証フェーズ
        /// シーンの整合性チェック、ビルド前の検証を行う
        /// 例: 必須コンポーネントの存在確認、参照の整合性チェック、ビルド設定の検証
        /// </summary>
        PreValidate = 100,
        
        /// <summary>
        /// 準備フェーズ
        /// 後続処理に必要な環境、リソース、データの準備を行う
        /// 例: 設定の読み込み、サービスの初期化、コンテナのセットアップ、処理計画の立案
        /// </summary>
        Setup = 200,
        
        /// <summary>
        /// 生成フェーズ
        /// 新しいオブジェクトやアセットの生成を行う
        /// 例: GameObjectの生成、ScriptableObjectの作成、動的リソースの生成
        /// </summary>
        Build = 300,
        
        /// <summary>
        /// 処理フェーズ
        /// オブジェクトの加工、変換、関連付けを行う
        /// 例: 依存関係の注入、参照の設定、プロパティの割り当て、データの変換
        /// </summary>
        Process = 400,
        
        /// <summary>
        /// 最適化フェーズ
        /// パフォーマンス最適化、不要な要素の削除を行う
        /// 例: 静的バッチング、Lightmapの最適化、開発用コンポーネントの削除
        /// </summary>
        Optimize = 500,
        
        /// <summary>
        /// 事後検証フェーズ
        /// 全処理完了後の最終チェックを行う
        /// 例: ビルド結果の検証、必須要素の存在確認、整合性の最終確認
        /// </summary>
        PostValidate = 600,
    }
}
