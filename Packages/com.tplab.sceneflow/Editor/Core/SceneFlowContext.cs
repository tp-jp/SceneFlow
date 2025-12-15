using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// シーンフロー実行コンテキスト
    /// ステップ実行時に渡される情報を保持
    /// </summary>
    public sealed class SceneFlowContext
    {
        /// <summary>
        /// 処理対象のシーン
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// ビルドレポート
        /// </summary>
        public BuildReport Report { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="scene">処理対象のシーン</param>
        /// <param name="report">ビルドレポート</param>
        public SceneFlowContext(Scene scene, BuildReport report)
        {
            Scene = scene;
            Report = report;
        }
    }
}

