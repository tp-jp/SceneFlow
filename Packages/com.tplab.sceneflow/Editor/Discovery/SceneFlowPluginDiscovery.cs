using System;
using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Plugin;

namespace TpLab.SceneFlow.Editor.Discovery
{
    /// <summary>
    /// シーンフロープラグインの発見を行うクラス
    /// </summary>
    internal static class SceneFlowPluginDiscovery
    {
        /// <summary>
        /// シーンフロープラグインを発見する
        /// </summary>
        /// <returns>シーンフロープラグイン</returns>
        public static IEnumerable<ISceneFlowPlugin> Discover()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch { continue; }

                foreach (var type in types)
                {
                    if (type.IsAbstract) continue;
                    if (!typeof(ISceneFlowPlugin).IsAssignableFrom(type)) continue;

                    yield return (ISceneFlowPlugin)Activator.CreateInstance(type);
                }
            }
        }
    }
}