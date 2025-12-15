using TpLab.SceneFlow.Editor.Internal;
using UnityEditor;

namespace TpLab.SceneFlow.Editor.Bootstrap
{
    /// <summary>
    /// SceneFlowの初期化を行うクラス
    /// </summary>
    public static class SceneFlowBootstrap
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            Logger.Log("SceneFlow has been initialized.");
            
            // ジョブの事前検証などをここで行うことができます
            ValidateJobs();
        }

        static void ValidateJobs()
        {
            var jobs = Discovery.SceneFlowJobDiscovery.Discover();
            var count = 0;
            foreach (var job in jobs)
            {
                count++;
                Logger.LogDebug($"Found job: {job.JobId}");
            }
            Logger.Log($"Total {count} job(s) discovered.");
        }
    }
}

