using Xunit;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class SysRuntimeJsonSerializer_Serialize
    {
        private readonly ITestOutputHelper _output;

        public SysRuntimeJsonSerializer_Serialize(ITestOutputHelper output)
        {
            _output = output;
        }

        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容：シリアライズ(string)のテスト
        [Fact]
        public void Serialize_Test001(){
            // Given
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.SysRuntimeSerialization);
            var testData = new TestData_SRJ();
            #endregion

            // When
            #region
            var serialized = serializer.Serialize<TestData_SRJ>(testData);
            #endregion

            // Then
            #region
            Assert.Contains(testData._ALPHABETS, serialized);
            Assert.Contains(testData._alphabets, serialized);
            Assert.Contains(testData._Controls.Replace("\t","\\t").Replace("\r\n","\\r\\n"), serialized);
            Assert.Contains(testData._Japanese, serialized);
            Assert.Contains(testData._Kanas, serialized);
            Assert.Contains(testData._Numbers, serialized);
            Assert.Contains(testData._Symbols.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("/", "\\/"), serialized);
            Assert.Contains(testData._Symbols2B, serialized);
            #endregion
        }
    }
}
