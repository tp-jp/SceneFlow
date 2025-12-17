using TpLab.SceneFlow.Editor.Core;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.EntryPoint
{
    public class SceneFlowEntryPoint : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            SceneFlowPipeline.Run(scene);
        }
    }
}