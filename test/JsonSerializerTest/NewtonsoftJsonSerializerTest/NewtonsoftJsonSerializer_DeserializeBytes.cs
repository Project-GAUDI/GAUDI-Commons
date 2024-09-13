using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    /// <summary>
    /// シリアライズ／デシリアライズ用テストデータ　クラス
    /// </summary>

    public class NewtonsoftJsonSerializer_DeserializeBytes
    {
        private readonly ITestOutputHelper _output;

        public NewtonsoftJsonSerializer_DeserializeBytes(ITestOutputHelper output)
        {
            _output = output;
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
