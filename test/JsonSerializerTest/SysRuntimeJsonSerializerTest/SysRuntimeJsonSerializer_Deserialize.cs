using System.Runtime.Serialization;
using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    /// <summary>
    /// シリアライズ／デシリアライズ用テストデータ　クラス
    /// </summary>
    [DataContract]
    class TestData_SRJ
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

    public class SysRuntimeJsonSerializer_Deserialize
    {
        private readonly ITestOutputHelper _output;

        public SysRuntimeJsonSerializer_Deserialize(ITestOutputHelper output)
        {
            _output = output;
        }

        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容：デシリアライズ(string)のテスト
        [Fact]
        public void Deserialize_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.SysRuntimeSerialization);
            var testData = new TestData_SRJ();
            var serialized = serializer.Serialize<TestData_SRJ>(testData);
            #endregion

            // When
            #region
            var deserialized = serializer.Deserialize<TestData_SRJ>(serialized);
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
