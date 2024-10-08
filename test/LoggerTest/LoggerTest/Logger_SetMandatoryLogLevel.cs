using System;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(Logger_SetMandatoryLogLevel))]
    [CollectionDefinition(nameof(Logger_SetMandatoryLogLevel), DisableParallelization = true)]
    public class Logger_SetMandatoryLogLevel
    {
        const string ConnectionString = "TestConnectionString";

        private readonly ITestOutputHelper _output;
        ILogger log = LoggerFactory.GetLogger(typeof(Logger_SetMandatoryLogLevel));


        public Logger_SetMandatoryLogLevel(ITestOutputHelper output)
        {
            _output = output;
            log.SetOutputLogLevel("INFO");
            log.SetMandatoryLogLevel("INFO",0);
        }


        /// 強制ログレベル変更テスト
        /// ログが出力されない　その１
        /// ログレベルが最低
        ///    isUpload：false
        ///強制レベル変更によって1回目のWriteLogが実行され、時間経過でログレベルが元のDEBUGに戻り、2回目のWriteLogは実行されない。
        [Fact(DisplayName = "強制ログレベル変更1：INFO-DEBUG-TRACE")]
        public void SetMandatoryLogLevelWriteLogTest_001()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            //No1
            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "出力されない";


            //When
            log.SetMandatoryLogLevel("TRACE", 5);
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage);
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.StartsWith("<7>", _consoleOutput1);
            System.Threading.Thread.Sleep(6000);
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage);

            //Then
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("", _consoleOutput2);
        }

        /// 強制ログレベル変更テスト
		/// ログが出力されない　その２
        /// ログレベルが最高
        ///   isUpload：false
        ///強制レベル変更によって1回目のWriteLogが実行され、時間経過でログレベルが元のERRORに戻り、2回目のWriteLogは実行されない。
        [Fact(DisplayName = "強制ログレベル変更2：INFO-ERROR-WARN-ERROR")]
        [Trait("Phase", "Unit")]
        public void SetMandatoryLogLevelWriteLogTest_002()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            //No2
            //Given
            log.SetOutputLogLevel("ERROR");
            inputMessage = "";

            //When
            log.SetMandatoryLogLevel("WARN", 3);
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage);
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.StartsWith("<4>", _consoleOutput1);
            System.Threading.Thread.Sleep(5000);
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage);

            //Then
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("", _consoleOutput2);

        }

        /// 強制ログレベル変更テスト
        /// ログが出力される　その１
        /// ログレベルがTRACE(0)
        ///  isUpload：true
        ///  ModuleClient：テスト用ModuleClient
        ///強制レベル変更によって1回目のWriteLogが実行されず、時間経過でログレベルが元のTRACEに戻り、2回目のWriteLogは実行される。
        [Fact(DisplayName = "強制ログレベル変更3：INFO-TRACE-WARN-TRACE")]
        [Trait("Phase", "Unit")]
        public void SetMandatoryLogLevelWriteLogTest_003()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            // モジュールクライアント初期化
            IModuleClient client;
            //No3
            //Given
            log.SetOutputLogLevel("TRACE");
            inputMessage = null;
            client = CreateModuleClient(ConnectionString);
            log.SetModuleClient(client);
            

            //When
            log.SetMandatoryLogLevel("WARN", 3);
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage, true);
            //Then
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.Equal("", _consoleOutput1);
            System.Threading.Thread.Sleep(5000);
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage, true);

            //Then
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            // Assert.True(_consoleOutput.Contains("[TRC][Commons.Test.Logger_SetMandatoryLogLevel] -"));
            Assert.StartsWith("<7>", _consoleOutput2);
            Assert.Contains("[Logger_SetMandatoryLogLevel]", _consoleOutput2);

        }


        /// 強制ログレベル変更テスト
        /// ログが出力される　その２
        /// ログレベルがDEBUG(1)
        ///  isUpload：false
        ///  ModuleClient：テスト用ModuleClient
        ///強制レベル変更によって1回目のWriteLogが実行されず、時間経過でログレベルが元のDEBUGに戻り、2回目のWriteLogは実行される。
        [Fact(DisplayName = "強制ログレベル変更4：INFO-DEBUG-WARN-DEBUG")]
        [Trait("Phase", "Unit")]
        public void SetMandatoryLogLevelWriteLogTest_004()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            // モジュールクライアント初期化
            IModuleClient client;


            //No4
            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "TestNo.4";
            client = CreateModuleClient(ConnectionString);
            log.SetModuleClient(client);

            //When
            log.SetMandatoryLogLevel("WARN", 3);
            log.WriteLog(ILogger.LogLevel.INFO, inputMessage, false);
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("", _consoleOutput1);
            System.Threading.Thread.Sleep(5000);
            log.WriteLog(ILogger.LogLevel.INFO, inputMessage, false);

            //Then
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            // Assert.True(_consoleOutput.Contains("[INF][Commons.Test.Logger_SetMandatoryLogLevel] - TestNo.4"));
            Assert.StartsWith("<5>", _consoleOutput2);
            Assert.Contains("[Logger_SetMandatoryLogLevel]TestNo.4", _consoleOutput2);
        }

        /// 強制ログレベル変更テスト
        /// ログが出力される　その３
        ///     ログレベルがINFO(2)
        /// プロパティ更新不可の警告を出す
        ///     isUpload：true
        ///     ModuleClient：null
        ///強制レベル変更によって1回目のWriteLogが実行されず、時間経過でログレベルが元のINFOに戻り、2回目のWriteLogは実行される。
        [Fact(DisplayName = "強制ログレベル変更5：INFO-ERROR-INFO")]
        [Trait("Phase", "Unit")]
        public void SetMandatoryLogLevelWriteLogTest_005()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            //No5
            //Given
            log.SetOutputLogLevel("INFO");
            inputMessage = "TestNo.5";
            log.SetModuleClient(null);

            //When
            log.SetMandatoryLogLevel("ERROR", 3);
            log.WriteLog(ILogger.LogLevel.INFO, inputMessage, true);
            w.Flush();
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.Equal("", _consoleOutput1);
            System.Threading.Thread.Sleep(5000);
            log.WriteLog(ILogger.LogLevel.INFO, inputMessage, true);

            //Then
            w.Flush();
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            // Assert.True(_consoleOutput.Contains("[INF][Commons.Test.Logger_SetMandatoryLogLevel] - "));
            Assert.StartsWith("<5>", _consoleOutput2);
            Assert.Contains("[Logger_SetMandatoryLogLevel] - ", _consoleOutput2);
            Assert.Contains("[WRN][Logger_SetMandatoryLogLevel] - Can't Update Reported Properties because Logger dosen't have module client.", _consoleOutput2);

        }

        /// 強制ログレベル変更テスト
        /// ログが出力される　その４
        ///     ログレベルがWARN(3)
        /// プロパティ更新不可の警告を出さない
        ///     isUpload：false
        ///     ModuleClient：null
        ///1回目のWriteLogが実行され、強制ログレベル変更でもログレベルERRORのログを出さないようにすることはできないため、2回目のWriteLogも実行される。
        [Fact(DisplayName = "強制ログレベル変更6：INFO-WARN-ERROR")]
        public void SetMandatoryLogLevelWriteLogTest_006()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            //No6
            //Given
            log.SetOutputLogLevel("WARN");
            inputMessage = null;
            log.SetModuleClient(null);

            //When
            log.SetMandatoryLogLevel("ERROR", 3);
            log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, false);
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            // Assert.True(_consoleOutput.Contains("[ERR][Commons.Test.Logger_SetMandatoryLogLevel]"));
            Assert.StartsWith("<3>", _consoleOutput1);
            Assert.Contains("[Logger_SetMandatoryLogLevel]", _consoleOutput1);
            log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, false);

            //Then
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            // Assert.True(_consoleOutput.Contains("[ERR][Commons.Test.Logger_SetMandatoryLogLevel]"));
            Assert.StartsWith("<3>", _consoleOutput2);
            Assert.Contains("[Logger_SetMandatoryLogLevel]", _consoleOutput2);
        }

        /// 強制ログレベル変更テスト
        /// ログが出力される　その４
        ///     ログレベルがERROR(4)
        ///     isUpload：true
        ///     ModuleClient：テスト用ModuleClient
        /// 非同期処理コール確認
        ///     ※例外を発生させることで確認する
        ///ERRORは一番高いレベルなので防ぐことができず、強制ログレベル変更をしてもログは出力される
        [Fact(DisplayName = "強制ログレベル変更7：INFO-ERROR-WARN-ERROR")]
        public void SetMandatoryLogLevelWriteLogTest_007()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            // モジュールクライアント初期化
            IModuleClient client;
            //No7
            //Given
            log.SetOutputLogLevel("ERROR");
            inputMessage = "TestNo.7";
            client = CreateModuleClient(ConnectionString);
            log.SetModuleClient(client);

            //When
            log.SetMandatoryLogLevel("WARN", 3);
            log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true);
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.StartsWith("<3>", _consoleOutput1);
            Assert.Contains("[Logger_SetMandatoryLogLevel]TestNo.7", _consoleOutput1);
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true);

            //Then
            System.Threading.Thread.Sleep(5000);//非同期処理の例外が発生するのを待つため５秒間停止する。
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            Assert.StartsWith("<3>", _consoleOutput2);
            Assert.Contains("[Logger_SetMandatoryLogLevel]TestNo.7", _consoleOutput2);
            // 再現が出来ない為、一旦削除
            // Assert.True(_consoleOutput.Contains("[WRN][Commons.Test.Logger_SetMandatoryLogLevel] - UpdateReportedProperties failed:"));

            //Assert.Throws<Exception>(() => log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, true));

        }

        /// 強制ログレベル変更テスト
        /// 無期限 
        ///     ログレベルがDEBUG(1)
        ///     isUpload：false
        ///     ModuleClient：テスト用ModuleClient
        ///強制レベル変更によって1回目のWriteLogが実行されず、時間経過でログレベルが元のDEBUGに戻り、2回目のWriteLogは実行される。
        ///無期限でERRORに変更中にSetOutputLogLevelでTRACEに変更すると、強制ログレベル解除後にログレベルはTRACEになる。
        [Fact(DisplayName = "強制ログレベル変更8：無期限変更")]
        public void SetMandatoryLogLevelWriteLogTest_008()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            // モジュールクライアント初期化
            IModuleClient client;



            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "TestNo.8";
            client = CreateModuleClient(ConnectionString);
            log.SetModuleClient(client);

            //When
            log.SetMandatoryLogLevel("ERROR", -1);
            log.SetOutputLogLevel("TRACE");
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage, false);

            //Then
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("", _consoleOutput1);
            System.Threading.Thread.Sleep(5000);
            log.SetMandatoryLogLevel("INFO", 0);
            log.SetOutputLogLevel("TRACE");
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage, false);
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            Assert.StartsWith("<4>", _consoleOutput2);
        }

        /// 強制ログレベル変更テスト
        /// 無期限から期限付きに変更
        ///     ログレベルがDEBUG(1)
        ///     isUpload：false
        ///     ModuleClient：テスト用ModuleClient
        ///強制レベル変更（無期限）によって1回目のWriteLogが実行されず、再度強制ログレベル変更（期限付）で2回目のWriteLogは実行され、時間経過でログレベルがTRACEに戻り、3回目のWriteLogは実行される。
        ///無期限でERRORに変更中にSetOutputLogLevelでTRACEに変更するが反映はされず、強制ログレベル変更（期限付き）でログレベルはWARNになり、期限が切れるとログレベルはTRACEになる。
        [Fact(DisplayName = "強制ログレベル変更9：無期限から期限付きに変更")]
        [Trait("Phase", "Unit")]
        public void SetMandatoryLogLevelWriteLogTest_009()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            // モジュールクライアント初期化
            IModuleClient client;



            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "TestNo.9";
            client = CreateModuleClient(ConnectionString);
            log.SetModuleClient(client);

            //When
            log.SetMandatoryLogLevel("ERROR", -1);
            log.SetOutputLogLevel("TRACE");
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage, false);

            //Then
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.Equal("", _consoleOutput1);
            System.Threading.Thread.Sleep(1000);
            log.SetMandatoryLogLevel("WARN", 3);
            log.WriteLog(ILogger.LogLevel.ERROR, inputMessage, false);
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.StartsWith("<3>", _consoleOutput2);


            System.Threading.Thread.Sleep(5000);
            log.WriteLog(ILogger.LogLevel.TRACE, inputMessage, false);
            var _consoleOutput3 = w.GetStringBuilder().ToString().Trim();
            Assert.StartsWith("<7>", _consoleOutput3);
        }

        /// 強制ログレベル変更テスト
        /// 期限付きから期限付きに変更して延長
        ///     ログレベルがDEBUG(1)
        ///     isUpload：false
        ///     ModuleClient：テスト用ModuleClient
        ///強制ログレベル変更で1回目のWriteLogが実行されず、期限の延長で2回目のWriteLogは実行され、時間経過でログレベルがERRORに戻り、3回目のWriteLogは実行されない。
        [Fact(DisplayName = "強制ログレベル変更10：期限の延長")]
        public void SetMandatoryLogLevelWriteLogTest_010()
        {

            string inputMessage;

            var w = new System.IO.StringWriter();
            log.SetOutputTextWriter(w);

            // モジュールクライアント初期化
            IModuleClient client;



            //Given
            log.SetOutputLogLevel("DEBUG");
            inputMessage = "TestNo.10";
            client = CreateModuleClient(ConnectionString);
            log.SetModuleClient(client);

            //When
            log.SetMandatoryLogLevel("ERROR", 10);
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage, false);

            //Then
            var _consoleOutput1 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.Equal("", _consoleOutput1);
            log.SetMandatoryLogLevel("TRACE", 5);
            log.SetOutputLogLevel("ERROR");
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage, false);
            var _consoleOutput2 = w.GetStringBuilder().ToString().Trim();
            w.GetStringBuilder().Clear();
            Assert.StartsWith("<4>", _consoleOutput2);
            System.Threading.Thread.Sleep(6000);
            log.WriteLog(ILogger.LogLevel.WARN, inputMessage, false);
            var _consoleOutput3 = w.GetStringBuilder().ToString().Trim();
            Assert.Equal("", _consoleOutput3);
        }

        /// 強制ログレベル変更テスト
        /// 予期しないsecondsの値を受けた場合例外を投げる
        [Fact(DisplayName = "強制ログレベル変更11：不正なseconds")]
        public void SetMandatoryLogLevelWriteLogTest_011()
        {
            
            Assert.Throws<ArgumentException>(() => log.SetMandatoryLogLevel("ERROR", -2));
        }

        /*----------------------------------------------------------------------------------
            テスト用
         -----------------------------------------------------------------------------------*/
        /// テスト用 ModuleClient
        private IModuleClient CreateModuleClient(String connectionString)
        {

            // Mqtt
            return new MqttModuleClient(connectionString, "127.0.0.0");

            // SDK
            // TransportProtocol tpSetting = TransportProtocol.Amqp;
            // Task<IotHubModuleClient> task = IotHubModuleClient.CreateAsync(tpSetting.GetTransportSettings());

            // return task.Result;
        }
    }
}
