using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(StubLogger_WriteLog))]
    [CollectionDefinition(nameof(StubLogger_WriteLog), DisableParallelization = true)]
    public class StubLogger_WriteLog
    {
        const string ConnectionString = "TestConnectionString";

        private readonly ITestOutputHelper _output;
        ILogger log = StubLoggerFactory.GetLogger(typeof(StubLogger_WriteLog));


        public StubLogger_WriteLog(ITestOutputHelper output)
        {
            _output = output;
            log.SetOutputLogLevel("INFO");
            log.SetMandatoryLogLevel("INFO", 0);
        }

        /// ログの出力テスト
        /// ログが出力されない　その１
        /// ログレベルが最低
        ///     isUpload：false
        [Fact(DisplayName = "ログ不出：ログレベル最低")]
        public void WriteLogTest_001()
        {
            string inputMessage;

            //No1
            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "出力されない";

            //When
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage);

            //Then
            IStubLoggerResults loggerResults = log as IStubLoggerResults;
            var logs = loggerResults.GetLogs();
            Assert.Empty(logs);
        }

        /// ログの出力テスト
        /// ログが出力されない　その２
        /// ログレベルが最高
        ///     isUpload：true
        [Fact(DisplayName = "ログ不出：ログレベル最高")]
        public void WriteLogTest_002()
        {
            string inputMessage;

            //No2
            //Given
            log.SetOutputLogLevel("ERROR");
            inputMessage = "";

            //When
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage);

            //Then
            IStubLoggerResults loggerResults = log as IStubLoggerResults;
            var logs = loggerResults.GetLogs();
            Assert.Empty(logs);
        }

        /// ログの出力テスト
        /// ログが出力される　その１
        /// ログレベルがTRACE(0)
        ///     isUpload：true
        ///     ModuleClient：テスト用ModuleClient
        [Fact(DisplayName = "ログ出力：ログレベルTrace(0)")]
        public void WriteLogTest_003()
        {
            string inputMessage;

            //No3
            //Given
            log.SetOutputLogLevel("TRACE");
            inputMessage = null;

            //When
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage, true);

            //Then
            IStubLoggerResults loggerResults = log as IStubLoggerResults;
            var logs = loggerResults.GetLogs();
            Assert.Single(logs);
            // Assert.True(_consoleOutput.Contains("[TRC][Commons.Test.StubLogger_WriteLog] -"));
            Assert.StartsWith("<7>", logs[0]);
            Assert.Contains("[StubLogger_WriteLog]", logs[0]);

        }

        /// ログの出力テスト
        /// ログが出力される　その２
        /// ログレベルがDEBUG(1)
        ///     isUpload：false
        ///     ModuleClient：テスト用ModuleClient
        [Fact(DisplayName = "ログ出力：ログレベルDEBUG(1)")]
        public void WriteLogTest_004()
        {
            string inputMessage;

            //No4
            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "TestNo.4";

            //When
            log.WriteLog(ILogger.LogLevel.INFO, inputMessage, false);

            //Then
            IStubLoggerResults loggerResults = log as IStubLoggerResults;
            var logs = loggerResults.GetLogs();
            Assert.Single(logs);
            // Assert.True(_consoleOutput.Contains("[INF][Commons.Test.StubLogger_WriteLog] - TestNo.4"));
            Assert.StartsWith("<5>", logs[0]);
            Assert.Contains("[StubLogger_WriteLog]TestNo.4", logs[0]);

        }

        /// ログの出力テスト
        /// ログが出力される　その３
        ///     ログレベルがINFO(2)
        /// プロパティ更新不可の警告を出す
        ///     isUpload：true
        ///     ModuleClient：null
        // [Fact(Skip = "ログ出力：ログレベルINFO(2)")]
        // public void WriteLogTest_005()
        // {
        //     string inputMessage;

        //     //No5
        //     //Given
        //     log.SetOutputLogLevel("INFO");
        //     inputMessage = "";
        //     // ここでNotSupportedExcepionとなる為、テスト対象外
        //     log.SetModuleClient(null);

        //     //When
        //     log.WriteLog(ILogger.LogLevel.INFO, inputMessage, true);

        //     //Then
        //     IStubLoggerResults loggerResults = log as IStubLoggerResults;
        //     var logs = loggerResults.GetLogs();
        //     Assert.Single(logs);

        //     // Assert.True(_consoleOutput.Contains("[INF][Commons.Test.StubLogger_WriteLog] - "));
        //     Assert.StartsWith("<5>", logs[0]);
        //     Assert.Contains("[StubLogger_WriteLog] - ", logs[0]);
        //     Assert.Contains("[WRN][StubLogger_WriteLog] - Can't Update Reported Properties because Logger dosen't have module client.", logs[0]);

        // }

        /// ログの出力テスト
        /// ログが出力される　その４
        ///     ログレベルがWARN(3)
        /// プロパティ更新不可の警告を出さない
        ///     isUpload：false
        ///     ModuleClient：null
        // [Fact(Skip = "ログ出力：ログレベルWARN(3)")]
        // public void WriteLogTest_006()
        // {
        //     string inputMessage;

        //     //No6
        //     //Given
        //     log.SetOutputLogLevel("WARN");
        //     inputMessage = null;
        //     // ここでNotSupportedExcepionとなる為、テスト対象外
        //     log.SetModuleClient(null);

        //     //When
        //     log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, false);

        //     //Then
        //     System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
        //     IStubLoggerResults loggerResults = log as IStubLoggerResults;
        //     var logs = loggerResults.GetLogs();
        //     Assert.Single(logs);

        //     // Assert.True(_consoleOutput.Contains("[ERR][Commons.Test.StubLogger_WriteLog]"));
        //     Assert.StartsWith("<3>", logs[0]);
        //     Assert.Contains("[StubLogger_WriteLog]", logs[0]);
        // }

        /// ログの出力テスト
        /// ログが出力される　その４
        ///     ログレベルがERROR(4)
        /// 非同期処理コール確認
        ///     ※例外を発生させることで確認する
        ///     isUpload：true
        ///     ModuleClient：テスト用ModuleClient
        [Fact(DisplayName = "ログ出力：ログレベルERROR(4)")]
        public void WriteLogTest_007()
        {
            string inputMessage;

            // モジュールクライアント初期化
            //No7
            //Given
            log.SetOutputLogLevel("ERROR");
            inputMessage = "TestNo.7";

            //When
            log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true);

            //Then
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            IStubLoggerResults loggerResults = log as IStubLoggerResults;
            var logs = loggerResults.GetLogs();
            Assert.Single(logs);

            // Assert.True(_consoleOutput.Contains("[ERR][Commons.Test.StubLogger_WriteLog] - TestNo.7"));
            Assert.StartsWith("<3>", logs[0]);
            Assert.Contains("[StubLogger_WriteLog]TestNo.7", logs[0]);

            // 再現が出来ない為、一旦削除
            // Assert.True(_consoleOutput.Contains("[WRN][Commons.Test.StubLogger_WriteLog] - UpdateReportedProperties failed:"));

            //Assert.Throws<Exception>(() => log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true));

        }

        /// ログの出力テスト
        /// ログが出力される　その5
        ///     ログレベルがINFO(2)
        ///     isUpload：false
        //StreamWriterの引数としてStream型のMemoeryStreamについて確認
        // [Fact(Skip = "ログ出力：MemoryStream使用")]
        // public void SetOutputStreamTest001()
        // {
        //     // Stub未対応メソッドの為、対象外
        //     var memoryStream = new MemoryStream();
        //     var msw = new StreamWriter(memoryStream);
        //     log.SetOutputTextWriter(msw);
        //     log.WriteLog(ILogger.LogLevel.INFO, "Test", false);
        //     msw.Flush();

        //     memoryStream.Position = 0; // ストリームの位置を初めに戻す
        //     using (var reader = new StreamReader(memoryStream))
        //     {
        //         var _consoleOutput = reader.ReadToEnd(); // ストリームの内容を文字列として読み取る
        //         Assert.StartsWith("<5>", _consoleOutput);
        //     }
        // }

        /// ログの出力テスト
        /// ログが出力される　その6
        ///     ログレベルがINFO(2)
        ///     isUpload：false
        //StreamWriterの引数としてStream型のFileStreamについて確認
        // [Fact(Skip = "ログ出力：FileStream使用")]
        // public void SetOutputStreamTest002()
        // {
        //     // Stub未対応メソッドの為、対象外
        //     FileStream fs = new FileStream("Test.txt", FileMode.Create);
        //     var msw = new StreamWriter(fs);
        //     log.SetOutputTextWriter(msw);
        //     log.WriteLog(ILogger.LogLevel.INFO, "Test", false);
        //     msw.Flush();
        //     fs.Close();

        //     FileStream fsr = new FileStream("Test.txt", FileMode.Open);
        //     using (var reader = new StreamReader(fsr))
        //     {
        //         var _consoleOutput = reader.ReadToEnd(); // ストリームの内容を文字列として読み取る
        //         Assert.StartsWith("<5>", _consoleOutput);
        //     }
        //     fsr.Close();
        // }

        /// ログの出力テスト
        /// ログが出力される　その７
        ///     StubModuleClientで送信を確認
        /// 非同期処理コール確認
        ///     isUpload：true
        ///     ModuleClient：StubModuleClient
        // [Fact(Skip = "ログ出力：ModuleClientによる送信")]
        // public async void MessageSendTest001()
        // {
        //     // Stub未対応メソッドの為、対象外
        //     string inputMessage;

        //     var w = new System.IO.StringWriter();
        //     log.SetOutputTextWriter(w);

        //     // モジュールクライアント初期化
        //     IModuleClient client;
        //     //No7
        //     //Given
        //     log.SetOutputLogLevel("ERROR");
        //     inputMessage = "MessageSendTest001";

        //     client = new StubModuleClient();
        //     log.SetModuleClient(client);

        //     IotMessageHandler handler = async (IotMessage message, Object context) =>
        //     {
        //         var body = message.GetBodyString();

        //         // Logger内でtry～catchしているので、ここでのテスト結果は、下部のDoesNotContainの部分で判定。
        //         Assert.Contains("\"RecordData\":[\"<3>", body);
        //         Assert.Contains("[StubLogger_WriteLog]MessageSendTest001", body);

        //         await Task.CompletedTask;
        //         return MessageResponse.Completed;
        //     };
        //     await client.SetInputMessageHandlerAsync("log", handler, null);

        //     //When
        //     log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true);

        //     //Then
        //     System.Threading.Thread.Sleep(500);//非同期処理の例外が完了するのを待つため0.５秒間停止する。
        //     var _consoleOutput = w.GetStringBuilder().ToString().Trim();

        //     Assert.StartsWith("<3>", _consoleOutput);
        //     Assert.Contains("[StubLogger_WriteLog]MessageSendTest001", _consoleOutput);

        //     // 送信メッセージの内容チェックにエラーが無いか判定
        //     Assert.DoesNotContain("[WRN][StubLogger_WriteLog] - UpdateReportedProperties failed:", _consoleOutput);

        //     //Assert.Throws<Exception>(() => log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true));

        // }

    }
}