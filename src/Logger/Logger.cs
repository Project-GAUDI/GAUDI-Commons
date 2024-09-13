using System;
using System.Collections.Generic;
using System.IO;
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
        /// Loggerインスタンスの取得
        /// </summary>
        public static ILogger GetLogger(Type type)
        {
            return new Logger(type.Name);
        }

        /// <summary>
        /// 出力ログレベルを文字列から設定
        /// </summary>
        public void SetOutputLogLevel(string logLevel)
        {
            OutputLogLevel = logLevel.ToLogLevel();
        }

        /// <summary>
        /// Loggerで使用するモジュールクライアントをセット
        /// </summary>
        public void SetModuleClient(IModuleClient moduleClient)
        {
            MyClient = moduleClient;
        }

        public Logger(string className)
        {
            LoggingClassName = className;
        }

        /// <summary>
        /// 出力ストリーム設定
        /// </summary>
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
        /// ログの出力
        /// </summary>
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
                if (MyClient == null ||  IotConnectionStatus.Connected != MyClient.ConnectionStatus)
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

        ///<summary>
        ///強制ログレベル変更
        ///</summary>
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
            else if(seconds == ILogger.SecondsDefinition_UNLIMITED) //無期限
            {
                mandatoryLoglevel = level.ToLogLevel();
                isMandatoryLoglevelPermanent = true;
            }
            else
            {
                throw new ArgumentException("seconds is not expected value.");
            }
        }

        ///<summary>
        ///指定したログレベルが有効かチェック
        ///</summary>
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
