using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    /// <summary>
    /// シリアライズ／デシリアライズ用テストデータ　クラス
    /// </summary>

    public class NewtonsoftJsonSerializer_Serialize
    {
        private readonly ITestOutputHelper _output;

        public NewtonsoftJsonSerializer_Serialize(ITestOutputHelper output)
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
