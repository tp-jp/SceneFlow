using System;
using TpLab.SceneFlow.Editor.Cores;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.EntryPoints
{
    public class SceneFlowEntryPoint : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            try
            {
                SceneFlowPipeline.Run(scene);
            }
            catch (Exception e)
            {
                if (BuildPipeline.isBuildingPlayer) throw new BuildFailedException(e);

                // 再生時は例外だけでは止まらないため、明示的に停止する
                Debug.LogException(e);
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += () => EditorApplication.isPlaying = false;
            }
        }
    }
}
