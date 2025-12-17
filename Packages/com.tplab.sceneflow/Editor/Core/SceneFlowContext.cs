using UnityEngine.SceneManagement;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// SceneFlow パイプライン実行時のコンテキスト
    /// すべての Pass に渡される情報を保持
    /// </summary>
    public sealed class SceneFlowContext
    {
        /// <summary>
        /// 処理対象のシーン
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// SceneFlowContext を初期化
        /// </summary>
        /// <param name="scene">処理対象のシーン</param>
        public SceneFlowContext(Scene scene)
        {
            Scene = scene;
        }
    }
}

