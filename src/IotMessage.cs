using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

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
        /// <summary>Property設定モード</summary>
        public enum PropertySetMode {
            /// <summary>追加のみ（上書きしない）</summary>
            Add,        
            /// <summary>更新（上書き）のみ（追加はなし）</summary>
            Modify,
            /// <summary>追加および更新</summary>
            AddOrModify
        }

        /// <summary>
        /// azure messageオブジェクト
        /// </summary>
        public Message message { get; protected set; } = null;

        /// <summary>
        /// メッセージBodyデータ
        /// </summary>
        protected Byte[] byteData {get; set;} = null;

        /// <summary>
        /// message streamオブジェクト
        /// </summary>
        protected Stream bodyStream {get; set;} = null;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public IotMessage()
        {
            message = new Message();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <params name="byteArray">メッセージBodyデータ</params>
        public IotMessage(Byte[] byteArray)
        {
            byteData = byteArray;
            message = new Message(byteArray);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <params name="messageString">メッセージBody文字列</params>
        public IotMessage(string messageString)
        {
            byteData = StringToByates(messageString);
            message = new Message(byteData);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <params name="stream">メッセージBody Stream</params>
        public IotMessage(System.IO.Stream stream)
        {
            bodyStream = stream;
            message = new Message(stream);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// Commons内部利用用。
        /// </remarks>
        /// <params name="orgMessage">azure メッセージオブジェクト</params>
        public IotMessage(Message orgMessage)
        {
            message = orgMessage;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <params name="orgMessage">azure メッセージオブジェクト</params>
        /// <remarks>
        /// メッセージプロパティもコピーします
        /// </remarks>
        public IotMessage(IotMessage orgMessage)
        {
            var stream = orgMessage.GetBodyStream();
            message = new Message(stream);
            SetProperties( orgMessage.GetProperties() );
            bodyStream = stream;
        }

        /// <summary>
        /// 廃棄メソッド。（IDisposableの実装メソッド）
        /// </summary>
        public void Dispose()
        {
            message.Dispose();
            message = null;
        }
        
        /// <summary>
        /// メッセージBodyデータを取得
        /// </summary>
        /// <returns>メッセージBodyデータ</returns>
        public Byte[] GetBytes()
        {
            // 未取得の場合取得する
            if ( byteData == null ) {
                if ( message != null ) {
                    byteData = message.GetBytes();
                }
            }
            return byteData;
        }

        /// <summary>
        /// 文字列化したメッセージBodyを取得
        /// </summary>
        /// <returns>メッセージBody文字列</returns>
        public string GetBodyString()
        {
            return BytesToString(GetBytes());
        }

        /// <summary>
        /// メッセージBody Streamを取得
        /// </summary>
        /// <returns>メッセージBody Stream</returns>
        public Stream GetBodyStream()
        {
            // 未取得の場合取得する
            if ( bodyStream == null ) {
                if ( message != null ) {    
                    bodyStream = message.GetBodyStream();
                }
            }

            // ストリームを初期化して返す
            if ( bodyStream != null ) {
                if (bodyStream.CanSeek) {
                    bodyStream.Seek(0, SeekOrigin.Begin);
                }
            }
            return bodyStream;
        }

        /// <summary>
        /// メッセージBodyデータのサイズを取得
        /// </summary>
        /// <returns>メッセージBodyサイズ</returns>
        public long GetBodyLength()
        {
            long retLength = 0;

            var bodyBytes = GetBytes();
            if ( bodyBytes != null ) 
            {
                retLength = bodyBytes.Length;
            }

            return retLength;
        }

        /// <summary>
        /// プロパティ設定
        /// </summary>
        /// <params name="key">プロパティキー</params>
        /// <params name="value">プロパティ値</params>
        /// <params name="setMode">
        ///     設定モード（PropertySetMode参照）
        ///     デフォルト：PropertySetMode.AddOrModify
        ///　</params>
        /// <returns>
        /// true : 設定した
        /// false : 設定しなかった
        /// </returns>
        public bool SetProperty( string key, string value, PropertySetMode setMode = PropertySetMode.AddOrModify )
        {
            bool retResult = false;

            if ( message != null ) {
                var isContains = message.Properties.ContainsKey(key);

                if ( isContains == true  )
                {
                    // 既存の場合に上書き
                    switch ( setMode ) {
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
                    switch ( setMode ) {
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
        /// プロパティ取得
        /// </summary>
        /// <params name="key">プロパティキー</params>
        /// <returns>
        /// プロパティ値。
        /// キーが無い場合は、nullを返す。
        /// </returns>
        public string GetProperty(string key) 
        {
            string retProperty = null;

            if ( message != null )
            {
                if ( message.Properties.ContainsKey(key) == true )
                {
                    retProperty = message.Properties[key];
                }
            }

            return retProperty;
        }

        /// <summary>
        /// プロパティ一括設定
        /// </summary>
        /// <params name="properties">プロパティ辞書</params>
        /// <params name="setMode">
        ///     設定モード（PropertySetMode参照）
        ///     デフォルト：PropertySetMode.AddOrModify
        /// </params>        
        /// <returns>
        /// true : 全て設定した
        /// false : 一部または全て設定しなかった
        /// </returns>
        public bool SetProperties(IDictionary<string, string> properties, PropertySetMode setMode = PropertySetMode.AddOrModify )
        {
            bool retResult = false;
            
            if ( message != null ) {
                retResult = true;

                foreach (var prop in properties)
                {
                    retResult &= SetProperty(prop.Key, prop.Value, setMode);
                }

            }

            return retResult;
        }

        /// <summary>
        /// プロパティ辞書取得
        /// </summary>
        /// <returns>
        /// プロパティ辞書
        /// </returns>
        public IDictionary<string, string> GetProperties()
        {
            IDictionary<string, string> retProperties = null;
            
            if ( message != null ) {
                retProperties = message.Properties;
            }

            return retProperties;
        }

        /// <summary>
        /// バイト列→文字列変換
        /// </summary>
        /// <params name="byteArray">変換バイト列</params>
        /// <returns>
        /// 変換した文字列
        /// </returns>
        public static string BytesToString( Byte[] convBytes )
        {
            string retString = null;

            if ( convBytes != null ) {
                retString = Encoding.UTF8.GetString(convBytes);
            }
            return retString;
        }

        /// <summary>
        /// 文字列→バイト列変換
        /// </summary>
        /// <params name="convString">変換文字列</params>
        /// <returns>
        /// 変換したバイト列
        /// </returns>
        public static Byte[] StringToByates( string convString )
        {
            Byte[] retBytes = null;

            if ( convString != null ) {
                retBytes = Encoding.UTF8.GetBytes(convString);
            }
            return retBytes;
        }
    }
}
