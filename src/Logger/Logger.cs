using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    internal static class Converters
    {
        public static ILogger.LogLevel ToLogLevel(this string logLevel)
        {
            ILogger.LogLevel retLogLevel = ILogger.LogLevel.INFO;

            string tmp = logLevel.ToLower();

            switch (tmp)
            {
                case "trace":
                    retLogLevel = ILogger.LogLevel.TRACE;
                    break;
                case "debug":
                    retLogLevel = ILogger.LogLevel.DEBUG;
                    break;
                case "info":
                    retLogLevel = ILogger.LogLevel.INFO;
                    break;
                case "warn":
                    retLogLevel = ILogger.LogLevel.WARN;
                    break;
                case "error":
                    retLogLevel = ILogger.LogLevel.ERROR;
                    break;
                default:
                    throw new ArgumentException("logLevel is not expected string.");
            }

            return retLogLevel;
        }
    }

    internal class Logger : ILogger
    {
        public Logger() { }

        private static ILogger.LogLevel outputLogLevel = ILogger.LogLevel.INFO;

        public ILogger.LogLevel OutputLogLevel
        {
            get
            {
                if (isMandatoryLoglevelPermanent == false && mandatoryLoglevelTime < DateTime.Now)
                {
                    return outputLogLevel;
                }
                else
                {
                    return mandatoryLoglevel;
                }
            }
            set { outputLogLevel = value; }
        }

        private static string outputName = "log";

        public string OutputName
        {
            get { return outputName; }
            set { outputName = value; }
        }

        private static IModuleClient myClient = null;

        public IModuleClient MyClient
        {
            get { return myClient; }
            set { myClient = value; }
        }

        private static TextWriter outTextWriter = TextWriter.Synchronized(new StreamWriter(Console.OpenStandardOutput()));
        public TextWriter OutTextWriter
        {
            get { return outTextWriter; }
            set { outTextWriter = value; }
        }

        private static SemaphoreSlim MySemaphore { get; } = new SemaphoreSlim(1, 1);

        private static Queue<string> MessageQueue { get; } = new Queue<string>();

        private string LoggingClassName { get; }

        private static ILogger.LogLevel mandatoryLoglevel = ILogger.LogLevel.INFO;

        private static bool isMandatoryLoglevelPermanent = false;

        private static DateTime mandatoryLoglevelTime = DateTime.Now;





        /// <summary>
        /// 指定した型名で Logger インスタンスを取得する。
        /// </summary>
        /// <param name="type">ロガー名に使用する型情報</param>
        /// <returns>Logger インスタンス</returns>
        public static ILogger GetLogger(Type type)
        {
            return new Logger(type.Name);
        }

        /// <summary>
        /// 出力ログレベルを文字列から設定する。
        /// 指定した出力ログレベルが不正な場合、ArgumentException をスローする。
        /// </summary>
        /// <param name="logLevel">出力ログレベル</param>
        public void SetOutputLogLevel(string logLevel)
        {
            OutputLogLevel = logLevel.ToLogLevel();
        }

        /// <summary>
        /// Loggerで使用するモジュールクライアントをセットする。
        /// </summary>
        /// <param name="moduleClient">モジュールクライアント</param>
        public void SetModuleClient(IModuleClient moduleClient)
        {
            MyClient = moduleClient;
        }

        /// <summary>
        /// クラス名を指定して Logger インスタンスを初期化する。
        /// </summary>
        /// <param name="className">ログ出力時に使用するクラス名</param>
        public Logger(string className)
        {
            LoggingClassName = className;
        }

        /// <summary>
        /// 出力ストリームを設定する。
        /// 指定値が null の場合は設定を変更しない。
        /// </summary>
        /// <param name="textwriter">出力ストリーム</param>
        public void SetOutputTextWriter(TextWriter textwriter)
        {
            // ストリームの変更
            if (textwriter != null)
            {
                // 新しいストリームを設定
                OutTextWriter = TextWriter.Synchronized(textwriter);
            }
        }

        /// <summary>
        /// 指定した出力ログレベルに応じて、ログメッセージを出力する。
        /// メッセージ送信有無が true の場合、ログメッセージをアップロードする。
        /// 処理に失敗した場合、警告ログを出力する。
        /// </summary>
        /// <param name="level">出力ログレベル</param>
        /// <param name="message">ログメッセージ</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void WriteLog(ILogger.LogLevel level, string message, bool isUpload = false)
        {
            if ((int)OutputLogLevel > (int)level)
            {
                return;
            }

            string tag = string.Empty;
            switch (level)
            {
                case ILogger.LogLevel.TRACE:
                    tag = "7";
                    break;
                case ILogger.LogLevel.DEBUG:
                    tag = "6";
                    break;
                case ILogger.LogLevel.INFO:
                    tag = "5";
                    break;
                case ILogger.LogLevel.WARN:
                    tag = "4";
                    break;
                case ILogger.LogLevel.ERROR:
                    tag = "3";
                    break;
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
            string prefix = $"<{tag}> {time} [{LoggingClassName}]";
            string msg = prefix + (message == null ? "" : message).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", $"{Environment.NewLine}{prefix}");

            OutTextWriter.WriteLine(msg);
            OutTextWriter.Flush();

            if (isUpload)
            {
                if (MyClient == null || IotConnectionStatus.Connected != MyClient.ConnectionStatus)
                {
                    OutTextWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [WRN][{LoggingClassName}]" +
                        " - Can't Update Reported Properties because Logger dosen't have module client.");
                    OutTextWriter.Flush();
                    return;
                }

                MessageQueue.Enqueue(msg);
                Task.Run(async () =>
                {
                    await MySemaphore.WaitAsync();
                    try
                    {
                        string deqMsg;
                        if (MessageQueue.TryDequeue(out deqMsg))
                        {
                            var info = new JsonMessage.RecordInfo()
                            {
                                RecordHeader = new List<string>()
                                {
                                    time
                                },
                                RecordData = new List<string>()
                                {
                                    deqMsg
                                }
                            };
                            var sendMsg = new IotMessage(JsonMessage.SerializeRecordInfoByte(info));
                            sendMsg.SetProperty("type", "log");
                            sendMsg.SetProperty("level", tag);
                            await MyClient.SendEventAsync(OutputName, sendMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        OutTextWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [WRN][{LoggingClassName}]" +
                            $" - UpdateReportedProperties failed:{ex.Message} {ex.StackTrace}");
                        OutTextWriter.Flush();
                    }
                    finally
                    {
                        MySemaphore.Release();
                    }
                });
            }
        }

        /// <summary>
        /// TRACEログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void Trace(string message, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithMemberName(ILogger.LogLevel.TRACE, message, memberName, isUpload);

        /// <summary>
        /// DEBUGログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void Debug(string message, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithMemberName(ILogger.LogLevel.DEBUG, message, memberName, isUpload);

        /// <summary>
        /// INFOログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void Info(string message, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithMemberName(ILogger.LogLevel.INFO, message, memberName, isUpload);

        /// <summary>
        /// WARNログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void Warn(string message, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithMemberName(ILogger.LogLevel.WARN, message, memberName, isUpload);

        /// <summary>
        /// ERRORログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void Error(string message, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithMemberName(ILogger.LogLevel.ERROR, message, memberName, isUpload);

        /// <summary>
        /// 引数情報を含めてTRACEログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void TraceWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithArgs(ILogger.LogLevel.TRACE, message, args, memberName, isUpload);

        /// <summary>
        /// 引数情報を含めてDEBUGログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void DebugWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithArgs(ILogger.LogLevel.DEBUG, message, args, memberName, isUpload);

        /// <summary>
        /// 引数情報を含めてINFOログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void InfoWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithArgs(ILogger.LogLevel.INFO, message, args, memberName, isUpload);

        /// <summary>
        /// 引数情報を含めてWARNログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void WarnWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithArgs(ILogger.LogLevel.WARN, message, args, memberName, isUpload);

        /// <summary>
        /// 引数情報を含めてERRORログを出力する。
        /// </summary>
        /// <param name="message">ログメッセージ</param>
        /// <param name="args">出力する引数情報</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        /// <param name="isUpload">メッセージ送信有無（デフォルト：false）</param>
        public void ErrorWithArgs(string message, object args, [CallerMemberName] string memberName = "", bool isUpload = false)
            => WriteLogWithArgs(ILogger.LogLevel.ERROR, message, args, memberName, isUpload);

        /// <summary>
        /// メソッド開始時のTRACEログを出力する。
        /// </summary>
        /// <param name="args">出力する引数情報（デフォルト：null）</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        public void TraceMethodEntry(object args = null, [CallerMemberName] string memberName = "")
        {
            if (!IsLogLevelToOutput(ILogger.LogLevel.TRACE))
            {
                return;
            }

            string argsInfo = args != null ? $" args: {FormatArgs(args)}" : string.Empty;
            WriteLogWithMemberName(ILogger.LogLevel.TRACE, $"Enter{argsInfo}", memberName, false);
        }

        /// <summary>
        /// メソッド終了時のTRACEログを出力する。
        /// </summary>
        /// <param name="result">出力する戻り値情報（デフォルト：null）</param>
        /// <param name="memberName">呼び出し元メンバー名（デフォルト：呼び出し元メソッド名）</param>
        public void TraceMethodExit(object result = null, [CallerMemberName] string memberName = "")
        {
            if (!IsLogLevelToOutput(ILogger.LogLevel.TRACE))
            {
                return;
            }

            string resultInfo = result != null ? $" result: {FormatArgs(result)}" : string.Empty;
            WriteLogWithMemberName(ILogger.LogLevel.TRACE, $"Exit{resultInfo}", memberName, false);
        }

        private void WriteLogWithMemberName(ILogger.LogLevel level, string message, string memberName, bool isUpload)
        {
            WriteLog(level, FormatMessage(memberName, message), isUpload);
        }

        private void WriteLogWithArgs(ILogger.LogLevel level, string message, object args, string memberName, bool isUpload)
        {
            string formattedMessage = FormatMessage(memberName, $"{message} | args: {FormatArgs(args)}");
            WriteLog(level, formattedMessage, isUpload);
        }

        private static string FormatMessage(string memberName, string message)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                return message ?? string.Empty;
            }

            return $"[{memberName}] {message ?? string.Empty}";
        }

        private static string SanitizeForLogging(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("|", string.Empty);
        }

        private static string FormatArgs(object args)
        {
            if (args == null)
            {
                return "null";
            }

            Type type = args.GetType();
            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
            {
                return SanitizeForLogging(args.ToString());
            }

            try
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (properties.Length == 0)
                {
                    return args.ToString();
                }

                var builder = new StringBuilder("{ ");
                for (int index = 0; index < properties.Length; index++)
                {
                    if (index > 0)
                    {
                        builder.Append(", ");
                    }

                    PropertyInfo property = properties[index];
                    object value;
                    try
                    {
                        value = property.GetValue(args);
                    }
                    catch
                    {
                        value = "<error>";
                    }

                    builder.Append(property.Name);
                    builder.Append(" = ");
                    builder.Append(SanitizeForLogging(value?.ToString() ?? "null"));
                }

                builder.Append(" }");
                return builder.ToString();
            }
            catch
            {
                return args.ToString();
            }
        }

        /// <summary>
        /// 指定した時間定義に応じて、強制ログレベルを変更する。
        /// 時間定義が0の場合：解除
        /// 時間定義が1以上の場合：指定時間（秒）後まで適用
        /// 時間定義が-1の場合：無制限
        /// 時間定義が上記以外：ArgumentException をスローする
        /// </summary>
        /// <param name="level">出力ログレベル</param>
        /// <param name="seconds">強制レベル設定時の時間定義</param>
        public void SetMandatoryLogLevel(string level, int seconds)
        {
            if (seconds == ILogger.SecondsDefinition_CANCEL)        //無期限解除（リセット）
            {
                mandatoryLoglevelTime = DateTime.Now.AddSeconds(seconds);
                isMandatoryLoglevelPermanent = false;
            }
            else if (ILogger.SecondsDefinition_MINIMUM <= seconds)   //期限付き
            {
                mandatoryLoglevel = level.ToLogLevel();
                mandatoryLoglevelTime = DateTime.Now.AddSeconds(seconds);
                isMandatoryLoglevelPermanent = false;
            }
            else if (seconds == ILogger.SecondsDefinition_UNLIMITED) //無期限
            {
                mandatoryLoglevel = level.ToLogLevel();
                isMandatoryLoglevelPermanent = true;
            }
            else
            {
                throw new ArgumentException("seconds is not expected value.");
            }
        }

        /// <summary>
        /// 指定した出力ログレベルが有効かチェックする。
        /// </summary>
        /// <param name="level">出力ログレベル</param>
        /// <returns>有効(true)／無効(false)</returns>
        public bool IsLogLevelToOutput(ILogger.LogLevel level)
        {
            bool result = false;
            if (OutputLogLevel <= level)
            {
                result = true;
            }
            return result;
        }
    }
}
