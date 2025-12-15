using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Builder;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// シーンフローステップノード
    /// </summary>
    internal sealed class SceneFlowStepNode : IGraphNode
    {
        public SceneFlowStep Step { get; }
        public HashSet<string> RunAfter { get; } = new();
        public HashSet<string> RunBefore { get; } = new();

        public string Id => Step.Id;

        public SceneFlowStepNode(SceneFlowStep step)
        {
            Step = step;
            // ステップIDの依存関係をコピー（ジョブIDは既に展開済み）
            RunAfter.UnionWith(step.RunAfterSteps);
            RunBefore.UnionWith(step.RunBeforeSteps);
        }
    }
}

