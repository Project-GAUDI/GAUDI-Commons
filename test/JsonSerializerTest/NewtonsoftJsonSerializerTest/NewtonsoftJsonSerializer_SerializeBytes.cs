using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    /// <summary>
    /// シリアライズ／デシリアライズ用テストデータ　クラス
    /// </summary>

    public class NewtonsoftJsonSerializer_SerializeBytes
    {
        private readonly ITestOutputHelper _output;

        public NewtonsoftJsonSerializer_SerializeBytes(ITestOutputHelper output)
        {
            _output = output;
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
    }
}
