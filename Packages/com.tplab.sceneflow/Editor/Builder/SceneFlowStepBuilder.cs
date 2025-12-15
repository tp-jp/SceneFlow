using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Builder
{
    /// <summary>
    /// シーンフローステップビルダーの実装
    /// </summary>
    public sealed class SceneFlowStepBuilder : ISceneFlowStepBuilder
    {
        readonly SceneFlowStep _step;

        internal SceneFlowStepBuilder(SceneFlowPhase phase, string stepId)
        {
            _step = new SceneFlowStep(phase, stepId);
        }

        public ISceneFlowStepBuilder RunAfterJob(params string[] jobIds)
        {
            if (jobIds != null)
            {
                foreach (var id in jobIds)
                {
                    _step.RunAfterJobs.Add(id);
                }
            }
            return this;
        }

        public ISceneFlowStepBuilder RunAfterStep(params string[] stepIds)
        {
            if (stepIds != null)
            {
                foreach (var id in stepIds)
                {
                    _step.RunAfterSteps.Add(id);
                }
            }
            return this;
        }

        public ISceneFlowStepBuilder RunBeforeJob(params string[] jobIds)
        {
            if (jobIds != null)
            {
                foreach (var id in jobIds)
                {
                    _step.RunBeforeJobs.Add(id);
                }
            }
            return this;
        }

        public ISceneFlowStepBuilder RunBeforeStep(params string[] stepIds)
        {
            if (stepIds != null)
            {
                foreach (var id in stepIds)
                {
                    _step.RunBeforeSteps.Add(id);
                }
            }
            return this;
        }

        [Obsolete("Use RunAfterStep instead for clarity")]
        public ISceneFlowStepBuilder RunAfter(params string[] stepIds)
        {
            return RunAfterStep(stepIds);
        }

        [Obsolete("Use RunBeforeStep instead for clarity")]
        public ISceneFlowStepBuilder RunBefore(params string[] stepIds)
        {
            return RunBeforeStep(stepIds);
        }

        public ISceneFlowStepBuilder Execute(Action<SceneFlowContext> action)
        {
            _step.Action = action;
            return this;
        }

        internal SceneFlowStep Build()
        {
            return _step;
        }
    }

    /// <summary>
    /// シーンフローステップ
    /// </summary>
    public sealed class SceneFlowStep
    {
        public string Id { get; }
        public SceneFlowPhase Phase { get; }
        
        // ジョブID指定用（パイプライン側でステップIDに展開される）
        public HashSet<string> RunAfterJobs { get; } = new();
        public HashSet<string> RunBeforeJobs { get; } = new();
        
        // ステップID指定用
        public HashSet<string> RunAfterSteps { get; } = new();
        public HashSet<string> RunBeforeSteps { get; } = new();
        
        internal Action<SceneFlowContext> Action { get; set; }

        internal SceneFlowStep(SceneFlowPhase phase, string id)
        {
            Phase = phase;
            Id = id;
        }

        internal void Execute(SceneFlowContext context)
        {
            Action?.Invoke(context);
        }
    }
}
