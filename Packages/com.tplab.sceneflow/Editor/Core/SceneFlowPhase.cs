namespace TpLab.SceneFlow.Editor.Core
{
    /// <summary>
    /// シーンフローのフェーズ
    /// </summary>
    public enum SceneFlowPhase
    {
        /// <summary>
        /// 解決フェーズ
        /// </summary>
        Resolve = 0,
        
        /// <summary>
        /// 生成フェーズ
        /// </summary>
        Generate = 100,
        
        /// <summary>
        /// 変換フェーズ
        /// </summary>
        Transform = 200,
        
        /// <summary>
        /// 最適化フェーズ
        /// </summary>
        Optimize = 300,
    }
}
