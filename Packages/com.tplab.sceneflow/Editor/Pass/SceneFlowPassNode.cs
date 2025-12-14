using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// シーンフローパスのノード
    /// </summary>
    internal sealed class SceneFlowPassNode
    {
        public ISceneFlowPass Pass { get; }

        public HashSet<string> RunAfter { get; } = new();
        public HashSet<string> RunBefore { get; } = new();

        public SceneFlowPhase Phase => Pass.Phase;
        public string Id => Pass.Id;

        public SceneFlowPassNode(ISceneFlowPass pass)
        {
            Pass = pass;

            if (pass.RunAfter != null)
            {
                RunAfter.UnionWith(pass.RunAfter);
            }

            if (pass.RunBefore != null)
            {
                RunBefore.UnionWith(pass.RunBefore);
            }
        }
    }
}