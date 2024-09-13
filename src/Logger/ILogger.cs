using System.IO;

namespace TICO.GAUDI.Commons
{
    public interface ILogger
    {
        /// <summary>
        /// ログレベル
        /// </summary>
        enum LogLevel
        {
            TRACE = 0,
            DEBUG = 1,
            INFO = 2,
            WARN = 3,
            ERROR = 4
        }
        LogLevel OutputLogLevel { get; set; }

        string OutputName { get; set; }

        IModuleClient MyClient { get; set; }

        TextWriter OutTextWriter { get; set; }

        //強制レベル設定時の時間定義-未設定（範囲外）
        const int SecondsDefinition_OUTOFRANGE = -2;

        //強制レベル設定時の時間定義-無制限
        const int SecondsDefinition_UNLIMITED = -1;

        //強制レベル設定時の時間定義-無制限の解除
        const int SecondsDefinition_CANCEL = 0;

        //強制レベル設定時の時間定義-最小値
        const int SecondsDefinition_MINIMUM = 1;
        
        /// <summary>
        /// 出力ログレベルを文字列から設定
        /// </summary>
        void SetOutputLogLevel(string logLevel);

        /// <summary>
        /// Loggerで使用するモジュールクライアントをセット
        /// </summary>
        void SetModuleClient(IModuleClient moduleClient);

        /// <summary>
        /// 出力ストリーム設定
        /// </summary>
        void SetOutputTextWriter(TextWriter textWriter);

        /// <summary>
        /// ログの出力
        /// </summary>
        void WriteLog(LogLevel level, string message, bool isUpload = false);

        ///<summary>
        ///強制ログレベル変更
        ///</summary>
        void SetMandatoryLogLevel(string level, int seconds);

        ///<summary>
        ///指定ログレベルが有効かチェック
        ///</summary>
        bool IsLogLevelToOutput(LogLevel level);
    }
}