
namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// アプリケーション状態
    /// </summary>
    public enum ApplicationState
    {
        /// <summary>
        /// 開始状態
        /// </summary>
        Start,
        /// <summary>
        /// 初期化状態
        /// </summary>
        Initialize,
        /// <summary>
        /// 待機状態
        /// </summary>
        Ready,
        /// <summary>
        /// 実行中状態
        /// </summary>
        Running,
        /// <summary>
        /// 終了処理状態
        /// </summary>
        Terminate,
        /// <summary>
        /// 終了状態
        /// </summary>
        End
    }
}
