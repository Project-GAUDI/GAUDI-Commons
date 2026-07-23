using System.IO;
using System.Runtime.CompilerServices;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// Logger インターフェース
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// ログレベル
        /// </summary>
        enum LogLevel
        {
            /// <summary>
            /// トレース
            /// </summary>
            TRACE = 0,
            /// <summary>
            /// デバッグ
            /// </summary>
            DEBUG = 1,
            /// <summary>
            /// インフォ（デフォルト）
            /// </summary>
            INFO = 2,
            /// <summary>
            /// ワーニング
            /// </summary>
            WARN = 3,
            /// <summary>
            /// エラー
            /// </summary>
            ERROR = 4
        }
        /// <summary>
        /// 出力ログレベル
        /// </summary>
        LogLevel OutputLogLevel { get; set; }

        /// <summary>
        /// ログ出力名
        /// </summary>
        string OutputName { get; set; }

        /// <summary>
        /// モジュールクライアント
        /// </summary>
        IModuleClient MyClient { get; set; }

        /// <summary>
        /// ログ出力先ストリーム
        /// </summary>
        TextWriter OutTextWriter { get; set; }

        /// <summary>
        /// 強制レベル設定時の時間定義-未設定（範囲外）
        /// </summary>
        const int SecondsDefinition_OUTOFRANGE = -2;

        /// <summary>
        /// 強制レベル設定時の時間定義-無制限
        /// </summary>
        const int SecondsDefinition_UNLIMITED = -1;

        /// <summary>
        /// 強制レベル設定時の時間定義-無制限の解除
        /// </summary>
        const int SecondsDefinition_CANCEL = 0;

        /// <summary>
        /// 強制レベル設定時の時間定義-最小値
        /// </summary>
        const int SecondsDefinition_MINIMUM = 1;

        /// <summary>
        /// 出力ログレベルを文字列から設定する。
        /// 指定した出力ログレベルが不正な場合、ArgumentException をスローする。
        /// </summary>
        /// <param name="logLevel">出力ログレベル</param>
        void SetOutputLogLevel(string logLevel);

        /// <summary>
        /// Loggerで使用するモジュールクライアントをセットする。
        /// </summary>
        /// <param name="moduleClient">モジュールクライアント</param>
        void SetModuleClient(IModuleClient moduleClient);

        /// <summary>
        /// 出力ストリームを設定する。
        /// </summary>
        /// <param name="textWriter">出力ストリーム</param>
        void SetOutputTextWriter(TextWriter textWriter);

        /// <summary>
        /// 指定した出力ログレベルに応じて、ログメッセージを出力する。
        /// メッセージ送信有無が true の場合、ログメッセージをアップロードする。
        /// 処理に失敗した場合、警告ログを出力する。
        /// </summary>
        /// <param name="level">出力ログレベル</param>
        /// <param name="message">ログメッセージ</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void WriteLog(LogLevel level, string message, bool isUpload = false);

        /// <summary>
        /// 指定した時間定義に応じて、強制ログレベルを変更する。
        /// 時間定義が0の場合：解除
        /// 時間定義が1以上の場合：指定時間（秒）後まで適用
        /// 時間定義が-1の場合：無制限
        /// 時間定義が上記以外：ArgumentException をスローする
        /// </summary>
        /// <param name="level">出力ログレベル</param>
        /// <param name="seconds">強制レベル設定時の時間定義</param>
        void SetMandatoryLogLevel(string level, int seconds);

        /// <summary>
        /// 指定した出力ログレベルが有効かチェックする。
        /// </summary>
        /// <param name="level">出力ログレベル</param>
        /// <returns>有効(true)／無効(false)</returns>
        bool IsLogLevelToOutput(LogLevel level);

        /// <summary>
        /// TRACEログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void Trace(string message, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// DEBUGログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void Debug(string message, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// INFOログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void Info(string message, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// WARNログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void Warn(string message, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// ERRORログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void Error(string message, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// 引数情報を含めてTRACEログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void TraceWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// 引数情報を含めてDEBUGログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void DebugWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// 引数情報を含めてINFOログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void InfoWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// 引数情報を含めてWARNログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void WarnWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// 引数情報を含めてERRORログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        void ErrorWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false);

        /// <summary>
        /// メソッド開始時のTRACEログを出力する。
        /// </summary>
        /// <param name="args">出力する引数情報（デフォルト：null）</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        void TraceMethodEntry(object args = null, [CallerMemberName] string memberName = "");

        /// <summary>
        /// メソッド終了時のTRACEログを出力する。
        /// </summary>
        /// <param name="result">出力する戻り値情報（デフォルト：null）</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        void TraceMethodExit(object result = null, [CallerMemberName] string memberName = "");
    }
}
