using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Builder
{
    /// <summary>
    /// シーンフロージョブビルダーの実装
    /// </summary>
    internal sealed class SceneFlowJobBuilder : ISceneFlowJobBuilder
    {
        readonly List<SceneFlowStepBuilder> _stepBuilders = new();

        public ISceneFlowStepBuilder AddStep(SceneFlowPhase phase, string stepId)
        {
            var builder = new SceneFlowStepBuilder(phase, stepId);
            _stepBuilders.Add(builder);
            return builder;
        }

        internal List<SceneFlowStep> Build()
        {
            var steps = new List<SceneFlowStep>();
            foreach (var builder in _stepBuilders)
            {
                steps.Add(builder.Build());
            }
            return steps;
        }
    }
}

