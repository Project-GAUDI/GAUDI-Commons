using System;
using Xunit;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// シリアライズ／デシリアライズ用テストデータ　クラス
    /// </summary>
    [DataContract]
    class TestData_NJS
    {
        [DataMemberAttribute(Name = "_Numbers")]
        public string _Numbers{get;set;} = "1234567890";

        [DataMemberAttribute(Name = "_alphabets")]
        public string _alphabets{get;set;} = "abcdefghijklmnopqrstuvwxyz";
        [DataMemberAttribute(Name = "_ALPHABETS")]
        public string _ALPHABETS{get;set;} = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [DataMemberAttribute(Name = "_Symbols")]
        public string _Symbols{get;set;} = @"!""#$%&'()=-~^|\@`{[]}*:+;,<>./?_";
        [DataMemberAttribute(Name = "_Controls")]
        public string _Controls{get;set;} = "[　:全角スペース],[ :半角スペース],[\t:タブ],[\r\n:改行コードCRLF],";

        [DataMemberAttribute(Name = "_Kanas")]
        public string _Kanas{get;set;} = "ﾇﾌｱｧｳｩｴｪｵｫﾔｬﾕｭﾖｮﾜｦﾎﾍｰﾀﾃｲｨｽｶﾝﾅﾆﾗｾﾞﾟ｢｣ﾑｹﾚﾘﾉﾏｸｷﾊｼﾄﾁﾂｯｻｿﾋｺﾐﾓﾈ､ﾙ｡ﾒ･ﾛ";
        [DataMemberAttribute(Name = "_Symbols2B")]
        public string _Symbols2B{get;set;} = "！”＃＄％＆’（）＝－～＾｜￥＠‘｛「」｝＊：＋；，＜＞．／？＿￥";
        [DataMemberAttribute(Name = "_Japanese")]
        public string _Japanese{get;set;} = "①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹ㎜㎝㎞㎎㎏㏄㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫№℡№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼㍻≡∑∫纊鍈蓜炻棈兊夋奛奣寬﨑嵂咊咩哿喆坙坥垬埈―ソЫⅨ噂浬欺圭構蚕十申曾箪貼能表暴予禄兔喀媾彌拿杤歃濬畚秉綵臀藹觸軆鐔饅鷭偆砡";
    }

    public class NewtonsoftJsonSerializerTest
    {
        private readonly ITestOutputHelper _output;

        public NewtonsoftJsonSerializerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        /// NewtonsoftJsonSerializer のテスト
        /// テスト内容：シリアライズ(string)のテスト
        [Fact]
        public void Serialize_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);
            var testData = new TestData_NJS();
            #endregion

            // When
            #region
            var serialized = serializer.Serialize<TestData_NJS>(testData);
            #endregion

            // Then
            #region
            Assert.Contains(testData._ALPHABETS, serialized);
            Assert.Contains(testData._alphabets, serialized);
            Assert.Contains(testData._Controls.Replace("\t","\\t").Replace("\r\n","\\r\\n"), serialized);
            Assert.Contains(testData._Japanese, serialized);
            Assert.Contains(testData._Kanas, serialized);
            Assert.Contains(testData._Numbers, serialized);
            Assert.Contains(testData._Symbols.Replace("\\", "\\\\").Replace("\"", "\\\""), serialized);
            Assert.Contains(testData._Symbols2B, serialized);
            #endregion
        }

        /// NewtonsoftJsonSerializer のテスト
        /// テスト内容：シリアライズ(Bytes[])のテスト
        [Fact]
        public void SerializeBytes_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);
            var testData = new TestData_NJS();
            #endregion

            // When
            #region
            byte[] serialized = serializer.SerializeBytes<TestData_NJS>(testData);
            #endregion

            // Then
            #region
            var serializedString = System.Text.Encoding.UTF8.GetString(serialized);
            Assert.Contains(testData._ALPHABETS, serializedString);
            Assert.Contains(testData._alphabets, serializedString);
            Assert.Contains(testData._Controls.Replace("\t","\\t").Replace("\r\n","\\r\\n"), serializedString);
            Assert.Contains(testData._Japanese, serializedString);
            Assert.Contains(testData._Kanas, serializedString);
            Assert.Contains(testData._Numbers, serializedString);
            Assert.Contains(testData._Symbols.Replace("\\", "\\\\").Replace("\"", "\\\""), serializedString);
            Assert.Contains(testData._Symbols2B, serializedString);
            #endregion
        }


        /// NewtonsoftJsonSerializer のテスト
        /// テスト内容：デシリアライズ(string)のテスト
        [Fact]
        public void Deserialize_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);
            var testData = new TestData_NJS();
            var serialized = serializer.Serialize<TestData_NJS>(testData);
            #endregion

            // When
            #region
            var deserialized = serializer.Deserialize<TestData_NJS>(serialized);
            #endregion

            // Then
            #region
            Assert.Equal(testData._ALPHABETS, deserialized._ALPHABETS);
            Assert.Equal(testData._alphabets, deserialized._alphabets);
            Assert.Equal(testData._Controls, deserialized._Controls);
            Assert.Equal(testData._Japanese, deserialized._Japanese);
            Assert.Equal(testData._Kanas, deserialized._Kanas);
            Assert.Equal(testData._Numbers, deserialized._Numbers);
            Assert.Equal(testData._Symbols, deserialized._Symbols);
            Assert.Equal(testData._Symbols2B, deserialized._Symbols2B);
            #endregion
        }

        /// NewtonsoftJsonSerializer のテスト
        /// テスト内容：デシリアライズ(Bytes[])のテスト
        [Fact]
        public void DeserializeBytes_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.NewtonsoftJson);
            var testData = new TestData_NJS();
            var serialized = serializer.SerializeBytes<TestData_NJS>(testData);
            #endregion

            // When
            #region
            var deserialized = serializer.Deserialize<TestData_NJS>(serialized);
            #endregion

            // Then
            #region
            Assert.Equal(testData._ALPHABETS, deserialized._ALPHABETS);
            Assert.Equal(testData._alphabets, deserialized._alphabets);
            Assert.Equal(testData._Controls, deserialized._Controls);
            Assert.Equal(testData._Japanese, deserialized._Japanese);
            Assert.Equal(testData._Kanas, deserialized._Kanas);
            Assert.Equal(testData._Numbers, deserialized._Numbers);
            Assert.Equal(testData._Symbols, deserialized._Symbols);
            Assert.Equal(testData._Symbols2B, deserialized._Symbols2B);
            #endregion
        }
    }

}
