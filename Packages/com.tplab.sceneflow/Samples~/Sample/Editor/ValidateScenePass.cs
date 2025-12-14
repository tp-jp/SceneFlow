using System.Collections;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor;
using TpLab.SceneFlow.Editor.Core;
using TpLab.SceneFlow.Editor.Pass;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.Sample.Editor
{
    public sealed class ValidateScenePass : ISceneFlowPass
    {
        public string Id => "ValidateScene";

        public SceneFlowPhase Phase => SceneFlowPhase.Transform;

        public IEnumerable<string> RunAfter => null;

        public IEnumerable<string> RunBefore => new[]
        {
            "ModifyScene"
        };

        public void Execute(Scene scene, BuildReport report)
        {
            Debug.Log("[SceneFlow] ValidateScenePass");
        }
    }
}
