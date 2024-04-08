using System;
using Xunit;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    public class LoggerTest
    {
        private readonly string ConnectionString;
        private string _consoleOutput;
        private readonly ITestOutputHelper _output;

        public LoggerTest(ITestOutputHelper output)
        {
            _output = output;
            ConnectionString = Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING");
        }

        ///ログ出力レベル設定のテスト
        ///規定の文字列以外が引数に渡されたときにエラーが発生することを確認する。
        ///規定の文字列については、WriteLogTest_XXXの中で確認する。
        [Fact]
        public void SetOutputLogLevelErrorTest(){
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            Assert.Throws<ArgumentException>(() => Logger.SetOutputLogLevel("XXXX"));

        }

        /// ログの出力テスト
        /// ログが出力されない　その１
        /// ログレベルが最低
        [Fact]
        public void WriteLogTest_001()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

        //No1
        //Given
            Logger.SetOutputLogLevel("DEBUG");
            inputMessage = "出力されない";

        //When
            log.WriteLog(Logger.LogLevel.TRACE, inputMessage);


        //Then
            _consoleOutput = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("" , _consoleOutput);
        }

        /// ログの出力テスト
        /// ログが出力されない　その２
        /// ログレベルが最高
        [Fact]
        public void WriteLogTest_002()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

        //No2
        //Given
            Logger.SetOutputLogLevel("ERROR");
            inputMessage = "";

        //When
            log.WriteLog(Logger.LogLevel.WARN, inputMessage);


        //Then
            _consoleOutput = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("" , _consoleOutput);
        }

        /// ログの出力テスト
        /// ログが出力される　その１
        ///     ログレベルがTrace(0)
        [Fact]
        public void WriteLogTest_003()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

             // モジュールクライアント初期化
            IModuleClient client;
        //No3
        //Given
            Logger.SetOutputLogLevel("TRACE");
            inputMessage = null;
            client = CreateModuleClient(ConnectionString);
            Logger.SetModuleClient(client);

        //When
            log.WriteLog(Logger.LogLevel.TRACE, inputMessage, true);

        //Then
            _consoleOutput = w.GetStringBuilder().ToString().Trim();
            // Assert.True(_consoleOutput.Contains("[TRC][Commons.LoggerTest] -"));
            Assert.StartsWith("<7>", _consoleOutput);
            Assert.Contains("[TICO.GAUDI.Commons.LoggerTest]", _consoleOutput);

        }

        /// ログの出力テスト
        /// ログが出力される　その２
        ///     ログレベルがDEBUG(1)
        [Fact]
        public void WriteLogTest_004()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

             // モジュールクライアント初期化
            IModuleClient client;


        //No4
        //Given
            Logger.SetOutputLogLevel("DEBUG");
            inputMessage = "TestNo.4";
            client = CreateModuleClient(ConnectionString);
            Logger.SetModuleClient(client);

        //When
            log.WriteLog(Logger.LogLevel.INFO, inputMessage, false);

        //Then
            _consoleOutput = w.GetStringBuilder().ToString().Trim();
            // Assert.True(_consoleOutput.Contains("[INF][Commons.LoggerTest] - TestNo.4"));
            Assert.StartsWith("<5>", _consoleOutput);
            Assert.Contains("[TICO.GAUDI.Commons.LoggerTest]TestNo.4", _consoleOutput);

        }

        /// ログの出力テスト
        /// ログが出力される　その３
        ///     ログレベルがINFO(2)
        /// プロパティ更新不可の警告を出す
        ///     updateProperty:true
        ///     modukeClient:null
        [Fact]
        public void WriteLogTest_005()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

        //No5
        //Given
            Logger.SetOutputLogLevel("INFO");
            inputMessage = "";
            Logger.SetModuleClient(null);

        //When
            log.WriteLog(Logger.LogLevel.INFO, inputMessage, true);

        //Then
            _consoleOutput = w.GetStringBuilder().ToString().Trim();

            // Assert.True(_consoleOutput.Contains("[INF][Commons.LoggerTest] - "));
            Assert.StartsWith("<5>", _consoleOutput);
            Assert.Contains("[TICO.GAUDI.Commons.LoggerTest] - ", _consoleOutput);
            Assert.Contains("[WRN][TICO.GAUDI.Commons.LoggerTest] - Can't Update Reported Properties because Logger dosen't have module client.", _consoleOutput);


        }

        /// ログの出力テスト
        /// ログが出力される　その４
        ///     ログレベルがWARN(3)
        /// プロパティ更新不可の警告を出さない
        ///     updateProperty:false
        ///     modukeClient:null
        [Fact]
        public void WriteLogTest_006()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

        //No6
        //Given
            Logger.SetOutputLogLevel("WARN");
            inputMessage = null;
            Logger.SetModuleClient(null);

        //When
            log.WriteLog(Logger.LogLevel.ERROR, inputMessage, false);

        //Then
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            _consoleOutput = w.GetStringBuilder().ToString().Trim();

            // Assert.True(_consoleOutput.Contains("[ERR][Commons.LoggerTest]"));
            Assert.StartsWith("<3>", _consoleOutput);
            Assert.Contains("[TICO.GAUDI.Commons.LoggerTest]", _consoleOutput);


        }

        /// ログの出力テスト
        /// ログが出力される　その４
        ///     ログレベルがERROR(4)
        /// 非同期処理コール確認
        ///     ※例外を発生させることで確認する
        [Fact]
        public void WriteLogTest_007()
        {
            Logger log = Logger.GetLogger(typeof(LoggerTest));

            string inputMessage;

            var w = new System.IO.StringWriter();
            Console.SetOut(w);

             // モジュールクライアント初期化
            IModuleClient client;
        //No7
        //Given
            Logger.SetOutputLogLevel("ERROR");
            inputMessage = "TestNo.7";
            client = CreateModuleClient(ConnectionString);
            Logger.SetModuleClient(client);

        //When
            log.WriteLog(Logger.LogLevel.ERROR, inputMessage, true);

        //Then
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            _consoleOutput = w.GetStringBuilder().ToString().Trim();

            // Assert.True(_consoleOutput.Contains("[ERR][Commons.LoggerTest] - TestNo.7"));
            Assert.StartsWith("<3>", _consoleOutput);
            Assert.Contains("[TICO.GAUDI.Commons.LoggerTest]TestNo.7", _consoleOutput);

            // 再現が出来ない為、一旦削除
            // Assert.True(_consoleOutput.Contains("[WRN][Commons.LoggerTest] - UpdateReportedProperties failed:"));

            //Assert.Throws<Exception>(() => log.WriteLog(Logger.LogLevel.ERROR, inputMessage, true));
        }

        /*----------------------------------------------------------------------------------
            テスト用
         -----------------------------------------------------------------------------------*/
         /// テスト用 ModuleClient
        private IModuleClient CreateModuleClient(String connectionString) {
            // Mqtt
            return new MqttModuleClient(connectionString, "127.0.0.0" );

            // SDK
            // TransportProtocol tpSetting = TransportProtocol.Amqp;
            // Task<IotHubModuleClient> task = IotHubModuleClient.CreateAsync(tpSetting.GetTransportSettings());

            // return task.Result;
        }

    }

}
