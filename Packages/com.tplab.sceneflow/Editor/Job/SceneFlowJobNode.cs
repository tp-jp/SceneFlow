using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Job
{
    /// <summary>
    /// シーンフロージョブのノード（トポロジカルソート用）
    /// ジョブレベルの依存関係は廃止され、ステップレベルでのみ依存関係を管理します
    /// </summary>
    internal sealed class SceneFlowJobNode : IGraphNode
    {
        public ISceneFlowJob Job { get; }
        public HashSet<string> RunAfter { get; } = new();
        public HashSet<string> RunBefore { get; } = new();
        
        public string Id => Job.JobId;

        public SceneFlowJobNode(ISceneFlowJob job)
        {
            Job = job;
        }
    }
}