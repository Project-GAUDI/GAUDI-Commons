using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.Devices.Client;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// IotEdge用Messageクラス
    /// </summary>
    /// <remarks>
    /// azure messageオブジェクトのGetBytes()/GetBodyStream()等を使用すると、
    /// 内部データにアクセスできなくなる・再度呼び出しできない等の問題が発生する。
    /// そういった問題を回避する為にMessageクラスをラップした機能を提供する。
    /// ByteデータやStreamはメンバで保持して、上位に返す。
    /// メッセージプロパティは、内包するMessageクラスで直接管理する。
    /// </remarks>
    public class IotMessage : IDisposable
    {
        /// <summary>
        /// プロパティ設定モード
        /// </summary>
        public enum PropertySetMode
        {
            /// <summary>
            /// 追加のみ（上書きしない）
            /// </summary>
            Add,
            /// <summary>
            /// 更新のみ（追加しない）
            /// </summary>
            Modify,
            /// <summary>
            /// 追加または更新
            /// </summary>
            AddOrModify
        }

        /// <summary>
        /// azure messageオブジェクト
        /// </summary>
        protected Message message { get; set; } = null;

        /// <summary>
        /// メッセージBodyデータ
        /// </summary>
        protected Byte[] byteData { get; set; } = null;

        /// <summary>
        /// message streamオブジェクト
        /// </summary>
        protected Stream bodyStream { get; set; } = null;

        /// <summary>
        /// 空のメッセージ Body で、IotMessage インスタンスを生成する。
        /// ContentType プロパティが設定されていない場合は "application/json" を設定、ContentEncoding プロパティが設定されていない場合は "utf-8" を設定する。
        /// </summary>
        public IotMessage()
        {
            message = new Message();
            setContentTypeAndEncoding();
        }

        /// <summary>
        /// 指定したメッセージ Body で、IotMessage インスタンスを生成する。
        /// ContentType プロパティが設定されていない場合は "application/json" を設定、ContentEncoding プロパティが設定されていない場合は "utf-8" を設定する。
        /// </summary>
        /// <param name="byteArray">メッセージ Body バイト配列</param>
        public IotMessage(Byte[] byteArray)
        {
            byteData = byteArray;
            message = new Message(byteArray);
            setContentTypeAndEncoding();
        }

        /// <summary>
        /// 指定したメッセージ Body で、IotMessage インスタンスを生成する。
        /// ContentType プロパティが設定されていない場合は "application/json" を設定、ContentEncoding プロパティが設定されていない場合は "utf-8" を設定する。
        /// </summary>
        /// <param name="messageString">メッセージ Body 文字列</param>
        public IotMessage(string messageString)
        {
            byteData = StringToByates(messageString);
            message = new Message(byteData);
            setContentTypeAndEncoding();
        }

        /// <summary>
        /// 指定したメッセージ Body で、IotMessage インスタンスを生成する。
        /// ContentType プロパティが設定されていない場合は "application/json" を設定、ContentEncoding プロパティが設定されていない場合は "utf-8" を設定する。
        /// </summary>
        /// <param name="stream">メッセージ Body ストリーム</param>
        public IotMessage(System.IO.Stream stream)
        {
            bodyStream = stream;
            message = new Message(stream);
            setContentTypeAndEncoding();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// Commons内部利用用
        /// </remarks>
        /// <params name="orgMessage">azure メッセージオブジェクト</params>
        internal IotMessage(Message orgMessage)
        {
            message = orgMessage;
            setContentTypeAndEncoding();
        }
        
        /// <summary>
        /// 既存の IotMessage インスタンスをコピーして生成する（メッセージプロパティもコピーする）。
        /// ContentType プロパティが設定されていない場合は "application/json" を設定、ContentEncoding プロパティが設定されていない場合は "utf-8" を設定する。
        /// </summary>
        /// <param name="orgMessage">IotMessage インスタンス</param>
        public IotMessage(IotMessage orgMessage)
        {
            var stream = orgMessage.GetBodyStream();
            message = new Message(stream);
            SetProperties(orgMessage.GetProperties());
            bodyStream = stream;
            SetMessageId(orgMessage.GetMessageId());
            SetContentType(orgMessage.GetContentType());
            SetContentEncoding(orgMessage.GetContentEncoding());
            setContentTypeAndEncoding();
        }

        /// <summary>
        /// リソースを解放する。
        /// </summary>
        public void Dispose()
        {
            message.Dispose();
            message = null;
        }

        /// <summary>
        /// メッセージ Body をバイト配列で取得して返す。
        /// </summary>
        /// <returns>メッセージBodyデータ</returns>
        public Byte[] GetBytes()
        {
            // 未取得の場合取得する
            if (byteData == null)
            {
                if (message != null)
                {
                    byteData = message.GetBytes();
                }
            }
            return byteData;
        }

        /// <summary>
        /// メッセージ Body を文字列で取得して返す。
        /// </summary>
        /// <returns>メッセージBody文字列</returns>
        public string GetBodyString()
        {
            return BytesToString(GetBytes());
        }

        /// <summary>
        /// メッセージ Body をストリームで取得して返す。
        /// </summary>
        /// <returns>メッセージBody Stream</returns>
        public Stream GetBodyStream()
        {
            // 未取得の場合取得する
            if (bodyStream == null)
            {
                if (message != null)
                {
                    bodyStream = message.GetBodyStream();
                }
            }

            // ストリームを初期化して返す
            if (bodyStream != null)
            {
                if (bodyStream.CanSeek)
                {
                    bodyStream.Seek(0, SeekOrigin.Begin);
                }
            }
            return bodyStream;
        }

        /// <summary>
        /// メッセージ Body のサイズを取得する。
        /// </summary>
        /// <returns>メッセージBodyサイズ</returns>
        public long GetBodyLength()
        {
            long retLength = 0;

            var bodyBytes = GetBytes();
            if (bodyBytes != null)
            {
                retLength = bodyBytes.Length;
            }

            return retLength;
        }

        /// <summary>
        /// 指定したプロパティ設定モードに応じて、プロパティを追加・更新する。
        /// </summary>
        /// <params name="key">プロパティキー</params>
        /// <params name="value">プロパティ値</params>
        /// <params name="setMode">プロパティ設定モード（デフォルト：AddOrModify）</params>
        /// <returns>
        /// 設定した(true)／設定しなかった(false)
        /// </returns>
        public bool SetProperty(string key, string value, PropertySetMode setMode = PropertySetMode.AddOrModify)
        {
            bool retResult = false;

            if (message != null)
            {
                var isContains = message.Properties.ContainsKey(key);

                if (isContains == true)
                {
                    // 既存の場合に上書き
                    switch (setMode)
                    {
                        case PropertySetMode.Modify:
                        case PropertySetMode.AddOrModify:
                            message.Properties[key] = value;
                            retResult = true;
                            break;
                    }
                }
                else
                {
                    // 無い場合に追加
                    switch (setMode)
                    {
                        case PropertySetMode.Add:
                        case PropertySetMode.AddOrModify:
                            message.Properties.Add(key, value);
                            retResult = true;
                            break;
                    }
                }

            }

            return retResult;
        }

        /// <summary>
        /// 指定したプロパティキーのプロパティ値を取得する。
        /// キーが存在しない場合、null を返す。
        /// </summary>
        /// <params name="key">プロパティキー</params>
        /// <returns>プロパティ値</returns>
        public string GetProperty(string key)
        {
            string retProperty = null;

            if (message != null)
            {
                if (message.Properties.ContainsKey(key) == true)
                {
                    retProperty = message.Properties[key];
                }
            }

            return retProperty;
        }

        /// <summary>
        /// 指定したプロパティ設定モードに応じて、複数のプロパティを一括で追加・更新する。
        /// </summary>
        /// <params name="properties">プロパティ辞書</params>
        /// <params name="setMode">プロパティ設定モード（デフォルト：AddOrModify）</params>
        /// <returns>全て設定した(true)／一部または全て設定しなかった(false)</returns>
        public bool SetProperties(IDictionary<string, string> properties, PropertySetMode setMode = PropertySetMode.AddOrModify)
        {
            bool retResult = false;

            if (message != null)
            {
                retResult = true;

                foreach (var prop in properties)
                {
                    retResult &= SetProperty(prop.Key, prop.Value, setMode);
                }

            }

            return retResult;
        }

        /// <summary>
        /// 全プロパティを取得する。
        /// プロパティが存在しない場合、null を返す。
        /// </summary>
        /// <returns>プロパティ辞書</returns>
        public IDictionary<string, string> GetProperties()
        {
            IDictionary<string, string> retProperties = null;

            if (message != null)
            {
                retProperties = message.Properties;
            }

            return retProperties;
        }

        /// <summary>
        /// バイト配列を文字列に変換する。
        /// </summary>
        /// <params name="byteArray">変換するバイト配列</params>
        /// <returns>変換した文字列</returns>
        public static string BytesToString(Byte[] convBytes)
        {
            string retString = null;

            if (convBytes != null)
            {
                retString = Encoding.UTF8.GetString(convBytes);
            }
            return retString;
        }

        /// <summary>
        /// 文字列をバイト配列に変換する。
        /// </summary>
        /// <params name="convString">変換する文字列</params>
        /// <returns>変換したバイト配列</returns>
        public static Byte[] StringToByates(string convString)
        {
            Byte[] retBytes = null;

            if (convString != null)
            {
                retBytes = Encoding.UTF8.GetBytes(convString);
            }
            return retBytes;
        }

        /// <summary>
        /// ContentTypeとContentEncodingを初期化
        /// </summary>
        protected void setContentTypeAndEncoding()
        {
            if (string.IsNullOrEmpty(GetContentType()))
            {
                SetContentType("application/json");
            }

            if (string.IsNullOrEmpty(GetContentEncoding()))
            {
                SetContentEncoding("utf-8");
            }
        }

        /// <summary>
        /// messageの取得
        /// </summary>
        internal Message GetMessage()
        {
            Message mymessage = null;
            if (message != null)
            {
                mymessage = message;
            }
            return mymessage;
        }

        /// <summary>
        /// MessageId プロパティを取得する。
        /// </summary>
        /// <returns>MessageId</returns>
        public string GetMessageId()
        {
            string messageId = null;
            if (message != null)
            {
                messageId = message.MessageId;
            }
            return messageId;
        }

        /// <summary>
        /// MessageId プロパティを設定する。
        /// </summary>
        /// <param name="messageId">MessageId</param>
        public void SetMessageId(string messageId)
        {
            if (message != null)
            {
                message.MessageId = messageId;
            }
        }

        /// <summary>
        /// ContentType プロパティを取得する。
        /// </summary>
        /// <returns>ContentType</returns>
        public string GetContentType()
        {
            string contentType = null;
            if (message != null)
            {
                contentType = message.ContentType;
            }
            return contentType;
        }

        /// <summary>
        /// ContentType プロパティを設定する。
        /// </summary>
        /// <param name="contentType">ContentType</param>
        public void SetContentType(string contentType)
        {
            if (message != null)
            {
                message.ContentType = contentType;
            }
        }

        /// <summary>
        /// ContentEncoding プロパティを取得する。
        /// </summary>
        /// <returns>ContentEncoding</returns>
        public string GetContentEncoding()
        {
            string contentEncoding = null;
            if (message != null)
            {
                contentEncoding = message.ContentEncoding;
            }
            return contentEncoding;
        }

        /// <summary>
        /// ContentEncoding プロパティを設定する。
        /// </summary>
        /// <param name="contentEncoding">ContentEncoding</param>
        public void SetContentEncoding(string contentEncoding)
        {
            if (message != null)
            {
                message.ContentEncoding = contentEncoding;
            }
        }

        /// <summary>
        /// ConnectionDeviceId プロパティを取得する。
        /// </summary>
        /// <returns>ConnectionDeviceId</returns>
        public string GetConnectionDeviceId()
        {
            string connectionDeviceId = null;
            if (message != null)
            {
                connectionDeviceId = message.ConnectionDeviceId;
            }
            return connectionDeviceId;
        }

        /// <summary>
        /// ConnectionModuleId プロパティを取得する。
        /// </summary>
        /// <returns>ConnectionModuleId</returns>
        public string GetConnectionModuleId()
        {
            string connectionModuleId = null;
            if (message != null)
            {
                connectionModuleId = message.ConnectionModuleId;
            }
            return connectionModuleId;
        }
    }
}
