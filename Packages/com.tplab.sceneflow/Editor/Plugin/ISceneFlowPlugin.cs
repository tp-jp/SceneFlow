using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Pass;

namespace TpLab.SceneFlow.Editor.Plugin
{
    /// <summary>
    /// シーンフロープラグインのインターフェース
    /// </summary>
    public interface ISceneFlowPlugin
    {
        /// <summary>
        /// プラグインID
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// このプラグインのパスが実行される「後」のプラグインID一覧
        /// </summary>
        IEnumerable<string> RunAfterPlugins { get; }
        
        /// <summary>
        /// このプラグインのパスが実行される「前」のプラグインID一覧
        /// </summary>
        IEnumerable<string> RunBeforePlugins { get; }

        /// <summary>
        /// プラグインが提供するシーンフローパスを生成する。
        /// </summary>
        /// <returns>シーンフローパス</returns>
        IEnumerable<ISceneFlowPass> CreatePasses();
    }
}
