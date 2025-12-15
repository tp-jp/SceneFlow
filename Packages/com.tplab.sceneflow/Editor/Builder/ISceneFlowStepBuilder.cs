using System;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Builder
{
    /// <summary>
    /// シーンフローステップビルダーのインターフェース
    /// </summary>
    public interface ISceneFlowStepBuilder
    {
        /// <summary>
        /// 指定したジョブの全ステップの後に、このステップを実行する
        /// </summary>
        /// <param name="jobIds">ジョブID（例: "MyJob"）</param>
        ISceneFlowStepBuilder RunAfterJob(params string[] jobIds);

        /// <summary>
        /// 指定したステップの後に、このステップを実行する
        /// </summary>
        /// <param name="stepIds">ステップID（例: "MyJob.Step1"）</param>
        ISceneFlowStepBuilder RunAfterStep(params string[] stepIds);

        /// <summary>
        /// 指定したジョブの全ステップの前に、このステップを実行する
        /// </summary>
        /// <param name="jobIds">ジョブID（例: "MyJob"）</param>
        ISceneFlowStepBuilder RunBeforeJob(params string[] jobIds);

        /// <summary>
        /// 指定したステップの前に、このステップを実行する
        /// </summary>
        /// <param name="stepIds">ステップID（例: "MyJob.Step1"）</param>
        ISceneFlowStepBuilder RunBeforeStep(params string[] stepIds);

        /// <summary>
        /// ステップの実行アクションを指定する
        /// </summary>
        ISceneFlowStepBuilder Execute(Action<SceneFlowContext> action);
    }
}
