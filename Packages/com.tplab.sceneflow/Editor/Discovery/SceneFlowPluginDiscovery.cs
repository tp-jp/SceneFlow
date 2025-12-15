using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Job;

namespace TpLab.SceneFlow.Editor.Discovery
{
    /// <summary>
    /// シーンフロージョブの発見を行うクラス
    /// </summary>
    internal static class SceneFlowJobDiscovery
    {
        /// <summary>
        /// シーンフロージョブを発見する
        /// </summary>
        /// <returns>シーンフロージョブ</returns>
        public static IEnumerable<ISceneFlowJob> Discover()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch { continue; }

                foreach (var type in types)
                {
                    if (type.IsAbstract) continue;
                    if (!typeof(ISceneFlowJob).IsAssignableFrom(type)) continue;

                    yield return (ISceneFlowJob)Activator.CreateInstance(type);
                }
            }
        }
    }
}

