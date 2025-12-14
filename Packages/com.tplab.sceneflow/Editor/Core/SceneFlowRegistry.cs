using System.Collections.Generic;
using TpLab.SceneFlow.Editor.Pass;

namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// シーンフローパスの登録を管理するクラス
    /// </summary>
    public static class SceneFlowRegistry
    {
        static readonly List<ISceneFlowPass> _passes = new();

        /// <summary>
        /// シーン処理パスを登録する。
        /// </summary>
        /// <param name="pass">シーン処理パス</param>
        public static void Register(ISceneFlowPass pass)
        {
            _passes.Add(pass);
        }

        /// <summary>
        /// 登録されたシーン処理パスの一覧を取得する。
        /// </summary>
        public static IReadOnlyList<ISceneFlowPass> Passes => _passes;
    }
}
