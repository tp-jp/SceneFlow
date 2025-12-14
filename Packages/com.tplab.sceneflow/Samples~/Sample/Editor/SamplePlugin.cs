using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Pass;
using TpLab.SceneFlow.Editor.Plugin;

namespace Samples.Sample.Editor
{
    public sealed class SamplePlugin : ISceneFlowPlugin
    {
        public string PluginId => "SceneFlow.Sample.Basic";

        public IEnumerable<string> RunAfterPlugins => null;
        public IEnumerable<string> RunBeforePlugins => null;

        public IEnumerable<ISceneFlowPass> CreatePasses()
        {
            yield return new ValidateScenePass();
            yield return new ModifyScenePass();
        }
    }
}