using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Pass;

namespace TpLab.SceneFlow.Editor.Plugin
{
    /// <summary>
    /// シーンフロープラグインのノード
    /// </summary>
    internal sealed class SceneFlowPluginNode
    {
        public ISceneFlowPlugin Plugin { get; }
        public IReadOnlyList<ISceneFlowPass> Passes { get; }

        public SceneFlowPluginNode(ISceneFlowPlugin plugin)
        {
            Plugin = plugin;
            Passes = plugin.CreatePasses().ToList();
        }
    }
}