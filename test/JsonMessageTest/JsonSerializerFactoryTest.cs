using System;
using Xunit;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Xunit.Abstractions;
using TICO.GAUDI.Commons;
using System.Threading.Tasks;

namespace TICO.GAUDI.Commons
{
    public class JsonSerializerFactoryTest
    {
        private readonly ITestOutputHelper _output;

        public JsonSerializerFactoryTest(ITestOutputHelper output)
        {
            _output = output;
        }


        /// GetJsonSerializer のテスト
        /// テスト内容：シリアライザ未指定
        [Fact]
        public void GetJsonSerializer_Test001()
        {
            // Given
            #region
            #endregion

            // When
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer();
            #endregion

            // Then
            #region
            Assert.IsType<NewtonsoftJsonSerializer>(serializer);
            #endregion
        }

        /// GetJsonSerializer のテスト
        /// テスト内容：シリアライザ指定
        [Theory]
        [InlineData(SerializerType.Default)]
        [InlineData(SerializerType.NewtonsoftJson)]
        [InlineData(SerializerType.SysRuntimeSerialization)]
        public void GetJsonSerializer_Test002(SerializerType serializerType)
        {
            // Given
            #region
            #endregion

            // When
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(serializerType);
            #endregion

            // Then
            #region
            if (serializerType == SerializerType.SysRuntimeSerialization)
            {
                Assert.IsType<SysRuntimeJsonSerializer>(serializer);
            }
            else
            {
                Assert.IsType<NewtonsoftJsonSerializer>(serializer);
            }
            #endregion
        }

        /// GetJsonSerializer のテスト
        /// テスト内容：デフォルトシリアライザ環境変数指定
        [Theory]
        [InlineData("Default")]
        [InlineData("NewtonsoftJson")]
        [InlineData("SysRuntimeSerialization")]
        [InlineData("unknown")]
        public void GetJsonSerializer_Test003(string serializerTypeName)
        {
            // Given
            #region
            System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", serializerTypeName);
            #endregion

            // When
            #region
            var serializer = JsonSerializerFactory.GetJsonSerializer(SerializerType.Default);
            #endregion

            // Then
            #region
            if (serializerTypeName == "SysRuntimeSerialization")
            {
                Assert.IsType<SysRuntimeJsonSerializer>(serializer);
            }
            else
            {
                Assert.IsType<NewtonsoftJsonSerializer>(serializer);
            }

            System.Environment.SetEnvironmentVariable("IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER", null);
            #endregion
        }
    }

}
