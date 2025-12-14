using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.Sample.Editor
{
    public sealed class ModifyScenePass : ISceneFlowPass
    {
        public string Id => "ModifyScene";

        public SceneFlowPhase Phase => SceneFlowPhase.Transform;

        public IEnumerable<string> RunAfter => new[]
        {
            "ValidateScene"
        };

        public IEnumerable<string> RunBefore => null;

        public void Execute(Scene scene, BuildReport report)
        {
            Debug.Log("[SceneFlow] ModifyScenePass");
        }
    }
}