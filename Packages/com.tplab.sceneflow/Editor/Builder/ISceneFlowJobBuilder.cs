using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Builder
{
    /// <summary>
    /// シーンフロージョブビルダーのインターフェース
    /// </summary>
    public interface ISceneFlowJobBuilder
    {
        /// <summary>
        /// 指定されたフェーズにステップを追加する
        /// </summary>
        ISceneFlowStepBuilder AddStep(SceneFlowPhase phase, string stepId);
    }
}

