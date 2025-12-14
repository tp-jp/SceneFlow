using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Core;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.Pass
{
    /// <summary>
    /// シーンフローパスのインターフェース
    /// </summary>
    public interface ISceneFlowPass
    {
        /// <summary>
        /// パスを一意に識別する ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// パスのフェーズ
        /// </summary>
        SceneFlowPhase Phase { get; }

        /// <summary>
        /// この Pass より「後」に実行されるべき Pass ID
        /// </summary>
        IEnumerable<string> RunAfter { get; }

        /// <summary>
        /// この Pass より「前」に実行されるべき Pass ID
        /// </summary>
        IEnumerable<string> RunBefore { get; }

        /// <summary>
        /// シーン処理を実行する。
        /// </summary>
        /// <param name="scene">シーン</param>
        /// <param name="report">ビルドレポート</param>
        void Execute(Scene scene, BuildReport report);
    }
}
