using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// メッセージ受信時コールバック用プロパティクラス
    /// </summary>
    class CallbackProperties {
        /// <summary>
        /// メッセージハンドラ
        /// </summary>
        public MessageHandler messageHandler = null;

        /// <summary>
        /// ユーザコンテキスト
        /// </summary>
        public object messageUserContext;

    }

    public class MqttModuleClient : IModuleClient
    {
        public MqttClient MyMqttClient { get; set; } = null;

        private TransportTopic defaultSendTopic;
        private TransportTopic defaultReceiveTopic;

        private Dictionary<string, CallbackProperties> dictCallbackProps = new Dictionary<string, CallbackProperties>();
        private DesiredPropertyUpdateCallback patchCallback = null;
        private object patchUserContext;
        private List<string> inputNames = new List<string>();

        private string hubHostName;
        private string deviceId;
        private string moduleId;
        private string sasToken;

        // ツインプロパティ取得のためのトピック
        private const string TwinResponseTopicFilter = "$iothub/twin/res/#";
        private const string TwinResponseTopicPrefix = "$iothub/twin/res/";
        private const string TwinGetTopic = "$iothub/twin/GET/?$rid={0}";
        private const string TwinResponseTopicPattern = @"\$iothub/twin/res/(\d+)/(\?.+)";

        // ツインプロパティ更新通知受信のトピック
        private const string TwinPatchTopicFilter = "$iothub/twin/PATCH/properties/desired/#";
        private const string TwinPatchTopicPrefix = "$iothub/twin/PATCH/properties/desired/";

        // テレメトリ受信のトピック
        private const string ReceiveEventMessagePatternFilter = "devices/{0}/modules/{1}/#";
        private const string ReceiveEventMessagePrefixPattern = "devices/{0}/modules/{1}/";

        // テレメトリ送信のトピック
        private const string SendEventMessagePrefix = "devices/{0}/modules/{1}/messages/events";
        private const string SendEventMessagePrefixNoModule = "devices/{0}/messages/events";

        private readonly Regex twinResponseTopicRegex = new Regex(TwinResponseTopicPattern, RegexOptions.Compiled);

        private readonly string receiveEventMessageFilter;
        private readonly string receiveEventMessagePrefix;
        private readonly string sendEventMessagePrefix;

        // ツイン取得イベント
        private Action<string, string> twinResponseEvent;

        // ツイン取得タイムアウト
        private readonly TimeSpan twinTimeout = TimeSpan.FromSeconds(60);

        public MqttModuleClient(string sas, string hostName = null, string device = null, TransportTopic defaultSendTopic = TransportTopic.Iothub, TransportTopic defaultReceiveTopic = TransportTopic.Iothub)
        {
            hubHostName = Environment.GetEnvironmentVariable("IOTEDGE_IOTHUBHOSTNAME");

            deviceId = null;
            moduleId = null;
            sendEventMessagePrefix = null;
            if (null == device)
            {
                deviceId = Environment.GetEnvironmentVariable("IOTEDGE_DEVICEID");
                moduleId = Environment.GetEnvironmentVariable("IOTEDGE_MODULEID");
                sendEventMessagePrefix = string.Format(SendEventMessagePrefix, deviceId, moduleId);
            }
            else {
                deviceId = device;
                moduleId = null;
                sendEventMessagePrefix = string.Format(SendEventMessagePrefixNoModule, deviceId);
            }

            sasToken = sas;
            this.defaultSendTopic = defaultSendTopic;
            this.defaultReceiveTopic = defaultReceiveTopic;

            receiveEventMessageFilter = string.Format(ReceiveEventMessagePatternFilter, deviceId, moduleId);
            receiveEventMessagePrefix = string.Format(ReceiveEventMessagePrefixPattern, deviceId, moduleId);

            Create(hostName);
        }

        private void Create(string hostName)
        {
            if (hostName == null)
            {
                string gatewayHostName = Environment.GetEnvironmentVariable("IOTEDGE_GATEWAYHOSTNAME");
                MyMqttClient = new MqttClient(gatewayHostName);
            }
            else
            {
                MyMqttClient = new MqttClient(hostName);
            }

            MyMqttClient.Subscribe(
                new[] { TwinResponseTopicFilter },
                new[] { (byte)1 });

            MyMqttClient.Subscribe(
                new[] { TwinPatchTopicFilter },
                new[] { (byte)1 });

            MyMqttClient.Subscribe(
                new[] { receiveEventMessageFilter },
                new[] { (byte)1 });

            MyMqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        public async Task CloseAsync()
        {
            await Task.CompletedTask;

            MyMqttClient.Disconnect();
        }

        public void Dispose()
        {
            inputNames.Clear();
        }

        public async Task OpenAsync()
        {
            await Task.CompletedTask;

            string identifier;
            string username;
            string password = sasToken;

            if (null == moduleId)
            {
                identifier = $"{deviceId}";
                username = $"{hubHostName}/{deviceId}/?api-version=2018-06-30";
            }
            else{
                identifier = $"{deviceId}/{moduleId}";
                username = $"{hubHostName}/{deviceId}/{moduleId}/?api-version=2018-06-30";
            }

            var result = MyMqttClient.Connect(identifier, username, password);
            if (result != MqttMsgConnack.CONN_ACCEPTED)
            {
                throw new MqttCommunicationException();
            }
        }

        public async Task<Twin> GetTwinAsync()
        {
            using (var responseReceived = new SemaphoreSlim(0))
            {
                string response = null;
                string rid = Guid.NewGuid().ToString();
                ExceptionDispatchInfo responseException = null;

                // ツイン受信イベントの定義
                Action<string, string> onTwinResponse = (string message, string topic) =>
                {
                    try
                    {
                        if (ParseResponseTopic(topic, out string receivedRid, out int status))
                        {
                            if (rid == receivedRid && status < 300)
                            {
                                response = message;
                                responseReceived.Release();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        responseException = ExceptionDispatchInfo.Capture(e);
                        responseReceived.Release();
                    }
                };

                try
                {
                    twinResponseEvent += onTwinResponse;
                    string _TwinGetTopic = string.Format(TwinGetTopic, rid);
                    MyMqttClient.Publish(_TwinGetTopic, new byte[0]);

                    await responseReceived.WaitAsync(twinTimeout).ConfigureAwait(true);

                    if (responseException != null)
                    {
                        responseException.Throw();
                    }
                    if (response == null)
                    {
                        throw new TimeoutException($"Response for message {rid} not received");
                    }

                    Twin twin = new Twin()
                    {
                        Properties = JsonConvert.DeserializeObject<TwinProperties>(response)
                    };

                    return twin;
                }
                finally
                {
                    twinResponseEvent -= onTwinResponse;
                }
            }
        }

        public async Task SendEventAsync(string outputName, IotMessage message)
        {
            await SendEventAsync(outputName, message, defaultSendTopic).ConfigureAwait(false);
        }

        public async Task SendEventAsync(string outputName, IotMessage message, TransportTopic transportTopic = TransportTopic.Iothub)
        {
            await Task.CompletedTask;

            string prefix;
            string propertyString = "";

            propertyString += string.Join("&", message.GetProperties().Select(prop => $"{Uri.EscapeDataString(prop.Key)}={Uri.EscapeDataString(prop.Value)}"));

            if (transportTopic == TransportTopic.Iothub)
            {
                prefix = sendEventMessagePrefix;
                if (message.GetProperties().Count > 0) propertyString += "&";
                propertyString += $"{Uri.EscapeDataString("$.on")}={Uri.EscapeDataString(outputName)}";
            }
            else
            {
                prefix = outputName;
            }

            prefix += "/" + propertyString;

            MyMqttClient.Publish(prefix, message.GetBytes());
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            await SetDesiredPropertyUpdateCallbackAsync(callback, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        public async Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext, TransportTopic transportTopic)
        {
            await Task.CompletedTask;

            patchCallback = callback;
            patchUserContext = userContext;
        }

        public async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler iotHandler, object userContext)
        {
            await SetInputMessageHandlerAsync(inputName, iotHandler, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        public async Task SetInputMessageHandlerAsync(string inputName, IotMessageHandler iotHandler, object userContext, TransportTopic transportTopic = TransportTopic.Iothub)
        {
            await Task.CompletedTask;

            CallbackProperties callbackProps = new CallbackProperties();
            callbackProps.messageHandler = (msg, obj)=>{return iotHandler(new IotMessage(msg), obj);};
            callbackProps.messageUserContext = userContext;

            if (!inputNames.Contains(inputName))
            {
                // 辞書用トピック名設定
                string dictTopicName = inputName;

                if(transportTopic == TransportTopic.Mqtt)
                {
                    string topicName = inputName;

                    // #が含まれていないとき
                    if (topicName.IndexOf("#") < 0)
                    {
                        topicName = topicName + "/#";
                    }

                    MyMqttClient.Subscribe(
                        new[] { topicName },
                        new[] { (byte)1 });
                }

                inputNames.Add(inputName);
                dictCallbackProps.Add(dictTopicName, callbackProps);
            }
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext)
        {
            await SetMethodHandlerAsync(methodName, methodHandler, userContext, defaultReceiveTopic).ConfigureAwait(false);
        }

        public async Task SetMethodHandlerAsync(string methodName, MethodCallback methodHandler, object userContext, TransportTopic transportTopic)
        {
            await Task.Run(() =>
            {
                throw new NotImplementedException($"This method is not yet implemented in MqttModuleClient");
            });
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
        {
            await UpdateReportedPropertiesAsync(reportedProperties, defaultSendTopic).ConfigureAwait(false);
        }

        public async Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties, TransportTopic transportTopic)
        {
            await Task.Run(() =>
            {
                throw new NotImplementedException($"This method is not yet implemented in MqttModuleClient");
            });
        }

        /// <summary>
        /// メッセージ受信イベント
        /// </summary>
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var topic = e.Topic;
            var messageBytes = e.Message;
            var message = Encoding.UTF8.GetString(messageBytes);

            // ツイン
            if (topic.StartsWith(TwinResponseTopicPrefix))
            {
                twinResponseEvent(message, topic);
            }
            // ツイン更新通知
            else if (topic.StartsWith(TwinPatchTopicPrefix))
            {
                ReceiveTwinPatch(message);
            }
            // テレメトリ
            else if (IsInputTelemetry(topic))
            {
                ReceiveTelemetry(messageBytes, topic);
            }
        }

        /// <summary>
        /// ツインパッチ受信ハンドラ
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        private void ReceiveTwinPatch(string message)
        {
            if (patchCallback == null)
            {
                return;
            }

            var twinPatch = new Twin()
            {
                Properties = JsonConvert.DeserializeObject<TwinProperties>(message)
            };

            patchCallback(twinPatch.Properties.Desired, patchUserContext);
        }

        /// <summary>
        /// テレメトリ受信ハンドラ
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <param name="topic">受信トピック</param>
        private void ReceiveTelemetry(byte[] message, string topic)
        {
            // トピック名に対するコールバックプロパティを取得
            CallbackProperties callbackProps = GetCallbackProperty(topic);
            if (callbackProps == null)
            {
                return;
            }

            Message hubMessage = new Message(message);

            // トピックからプロパティ抽出
            var blocks = topic.Split("/");
            var propStr = blocks[blocks.Length - 1];

            var props = propStr.Split("&");
            foreach (var prop in props)
            {
                // Key/Valueに分解後、URIデコード実施。
                var t = prop.Split("=").Select(v => Uri.UnescapeDataString(v)).ToArray();
                if (t.Length == 2)
                {
                    // システムプロパティは無視
                    if (t[0].StartsWith("$"))
                    {
                        continue;
                    }
                    hubMessage.Properties.Add(t[0], t[1]);
                }
            }

            callbackProps.messageHandler?.Invoke(hubMessage, callbackProps.messageUserContext);
        }

        private bool ParseResponseTopic(string topic, out string rid, out Int32 status)
        {
            Match match = twinResponseTopicRegex.Match(topic);
            if (match.Success)
            {
                status = Convert.ToInt32(match.Groups[1].Value);
                rid = HttpUtility.ParseQueryString(match.Groups[2].Value).Get("$rid");
                return true;
            }

            rid = "";
            status = 500;
            return false;
        }

        private bool IsInputTelemetry(string topic)
        {
            bool result = false;

            if (topic.StartsWith(receiveEventMessagePrefix))
            {
                var blocks = topic.Split("/");

                var inputStr = blocks[blocks.Length - 2];
                result = inputNames.Contains(inputStr);
            }
            else if (topic.StartsWith("$iothub"))
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// コールバックプロパティデータ取得
        /// </summary>
        /// <param name="topic">指定トピック名</param>
        /// <returns>
        /// コールバックプロパティ
        /// 指定トピック名がない場合、nullを返す。
        /// </returns>
        private CallbackProperties GetCallbackProperty(string topic)
        {
            CallbackProperties retCallbakcProps = null;

            if (topic.StartsWith(receiveEventMessagePrefix))
            {
                // IoTHubトピック受信時

                // トピック全文を"/"で分解
                var blocks = topic.Split("/");
                // トピック名部分を抽出
                var inputStr = blocks[blocks.Length - 2];

                // トピック名に該当するコールバックプロパティを取得
                dictCallbackProps.TryGetValue(inputStr, out retCallbakcProps);
            }
            else if (topic.StartsWith("$iothub"))
            {
                retCallbakcProps = null;
            }
            else
            {
                // Mqttトピック受信時

                // Mqttトピック名抽出
                string mqttTopicName = GetMqttTopicName(topic);

                // トピック名に該当するコールバックプロパティを取得
                dictCallbackProps.TryGetValue(mqttTopicName, out retCallbakcProps);

                if (null == retCallbakcProps){
                    dictCallbackProps.TryGetValue("#", out retCallbakcProps);
                }
            }
            return retCallbakcProps;
        }

        /// <summary>
        /// Mqttトピック名の抽出
        /// </summary>
        /// <remarks>
        /// 末尾の"/"以降を切り捨てる。
        /// </remarks>
        /// <param name="fullTopic">メッセージ受信したトピック全文</param>
        /// <returns>
        /// 切り出したトピック名。
        /// "/"が含まれない場合、入力のまま返す。
        /// </returns>
        private string GetMqttTopicName(string fullTopic)
        {
            string retMqttTopicName = fullTopic;
            int sepIndex = fullTopic.LastIndexOf("/");

            if ( 0 <= sepIndex )
            {
                // 最後の"/"前までを切り取り
                retMqttTopicName = fullTopic.Substring(0, sepIndex);
            }

            return retMqttTopicName;
        }
    }
}
