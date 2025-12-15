using TpLab.SceneFlow.Editor.Builder;

namespace TpLab.SceneFlow.Editor.Job
{
    /// <summary>
    /// シーンフロージョブのインターフェース
    /// ジョブは複数のステップで構成される処理の単位
    /// </summary>
    public interface ISceneFlowJob
    {
        /// <summary>
        /// ジョブID
        /// ステップIDのプレフィックスとして使用されます（例: "MyJob.Step1"）
        /// </summary>
        string JobId { get; }

        /// <summary>
        /// ジョブを構成するステップを登録する
        /// ステップレベルで他のジョブのステップIDを指定することで、ジョブ間の依存関係を制御できます
        /// </summary>
        /// <param name="builder">ジョブビルダー</param>
        void Configure(ISceneFlowJobBuilder builder);
    }
}
