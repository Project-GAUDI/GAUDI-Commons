using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace TICO.GAUDI.Commons
{
    public class Logger
    {
        /// <summary>
        /// ログレベル
        /// </summary>
        public enum LogLevel
        {
            TRACE = 0,
            DEBUG = 1,
            INFO = 2,
            WARN = 3,
            ERROR = 4
        }

        public static LogLevel OutputLogLevel {get; private set;} = LogLevel.INFO;

        public static string OutputName { private get; set; } = "log";

        private static IModuleClient MyClient {get; set;} = null;

        private static SemaphoreSlim MySemaphore {get;} = new SemaphoreSlim(1,1);

        private static Queue<string> MessageQueue {get;} = new Queue<string>();

        private string LoggingClassName {get;}

        /// <summary>
        /// Loggerインスタンスの取得
        /// </summary>
        public static Logger GetLogger(Type type)
        {
            return new Logger(type.FullName);
        }

        /// <summary>
        /// 出力ログレベルを文字列から設定
        /// </summary>
        public static void SetOutputLogLevel(string logLevel)
        {
            string tmp = logLevel.ToLower();

            switch(tmp)
            {
                case "trace":
                    OutputLogLevel = LogLevel.TRACE;
                    break;
                case "debug":
                    OutputLogLevel = LogLevel.DEBUG;
                    break;
                case "info":
                    OutputLogLevel = LogLevel.INFO;
                    break;
                case "warn":
                    OutputLogLevel = LogLevel.WARN;
                    break;
                case "error":
                    OutputLogLevel = LogLevel.ERROR;
                    break;
                default:
                    throw new ArgumentException("logLevel is not expected string.");
            }
        }

        /// <summary>
        /// Loggerで使用するモジュールクライアントをセット
        /// </summary>
        public static void SetModuleClient(IModuleClient moduleClient)
        {
            MyClient = moduleClient;
        }

        private Logger(string className)
        {
            LoggingClassName = className;
        }

        /// <summary>
        /// ログの出力
        /// </summary>
        public void WriteLog(LogLevel level, string message, bool isUpload = false)
        {
            if((int)OutputLogLevel > (int)level)
            {
                return;
            }

            string tag = string.Empty;
            switch(level)
            {
                case LogLevel.TRACE:
                    tag = "7";
                    break;
                case LogLevel.DEBUG:
                    tag = "6";
                    break;
                case LogLevel.INFO:
                    tag = "5";
                    break;
                case LogLevel.WARN:
                    tag = "4";
                    break;
                case LogLevel.ERROR:
                    tag = "3";
                    break;
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
            string prefix = $"<{tag}> {time} [{LoggingClassName}]";
            string msg = prefix + (message == null ? "" : message).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", $"{Environment.NewLine}{prefix}");

            Console.WriteLine(msg);

            if(isUpload)
            {
                if(MyClient == null)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [WRN][{LoggingClassName}]" +
                        " - Can't Update Reported Properties because Logger dosen't have module client.");
                    return;
                }

                MessageQueue.Enqueue(msg);
                Task.Run(async () => {
                    await MySemaphore.WaitAsync();
                    try
                    {
                        string deqMsg;
                        if(MessageQueue.TryDequeue(out deqMsg))
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
                            await MyClient.SendEventAsync(OutputName, sendMsg, TransportTopic.Iothub);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [WRN][{LoggingClassName}]" +
                            $" - UpdateReportedProperties failed:{e.Message} {e.StackTrace}");
                    }
                    finally
                    {
                        MySemaphore.Release();
                    }
                });
            }
        } 
    }
}