using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{

    public class SysRuntimeJsonSerializer_DeserializeBytes
    {
        private readonly ITestOutputHelper _output;

        public SysRuntimeJsonSerializer_DeserializeBytes(ITestOutputHelper output)
        {
            _output = output;
        }

        
        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容：デシリアライズ(Bytes[])のテスト
        [Fact]
        public void DeserializeBytes_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.SysRuntimeSerialization);
            var testData = new TestData_SRJ();
            var serialized = serializer.SerializeBytes<TestData_SRJ>(testData);
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
