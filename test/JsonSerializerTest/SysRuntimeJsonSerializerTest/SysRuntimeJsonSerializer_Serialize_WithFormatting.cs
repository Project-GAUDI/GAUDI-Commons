using Xunit;
using Xunit.Abstractions;

namespace TICO.GAUDI.Commons.Test
{
    public class SysRuntimeJsonSerializer_Serialize_WithFormatting
    {
        private readonly ITestOutputHelper _output;

        public SysRuntimeJsonSerializer_Serialize_WithFormatting(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容:Formatting=Indented 指定時にインデント付き出力されることを確認
        /// </summary>
        [Fact]
        public void Serialize_WithIndented_ReturnsFormattedJson()
        {
            // Given
            var serializer = new SysRuntimeJsonSerializer();
            var testData = new TestData_SRJ();
            var settings = new JsonSerializerSettings
            {
                Formatting = JsonFormatting.Indented
            };

            // When
            var result = serializer.Serialize(testData, settings);

            // Then
            _output.WriteLine("Serialized JSON (Indented):");
            _output.WriteLine(result);

            // インデント付き出力の検証
            Assert.NotNull(result);
            Assert.Contains("\n", result); // 改行が含まれる
            Assert.Contains("  ", result); // インデントが含まれる

            // データの整合性確認
            Assert.Contains(testData._ALPHABETS, result);
            Assert.Contains(testData._Numbers, result);
        }

        /// <summary>
        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容:Formatting=None 指定時にコンパクト出力されることを確認
        /// </summary>
        [Fact]
        public void Serialize_WithNone_ReturnsCompactJson()
        {
            // Given
            var serializer = new SysRuntimeJsonSerializer();
            var testData = new TestData_SRJ();
            var settings = new JsonSerializerSettings
            {
                Formatting = JsonFormatting.None
            };

            // When
            var result = serializer.Serialize(testData, settings);

            // Then
            _output.WriteLine("Serialized JSON (None):");
            _output.WriteLine(result);

            // コンパクト出力の検証
            Assert.NotNull(result);
            Assert.DoesNotContain("\n", result); // 改行なし

            // データの整合性確認
            Assert.Contains(testData._ALPHABETS, result);
            Assert.Contains(testData._Numbers, result);
        }

        /// <summary>
        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容:SerializeBytes でも Formatting=Indented が有効であることを確認
        /// </summary>
        [Fact]
        public void SerializeBytes_WithIndented_ReturnsFormattedJson()
        {
            // Given
            var serializer = new SysRuntimeJsonSerializer();
            var testData = new TestData_SRJ();
            var settings = new JsonSerializerSettings
            {
                Formatting = JsonFormatting.Indented
            };

            // When
            var resultBytes = serializer.SerializeBytes(testData, settings);
            var result = System.Text.Encoding.UTF8.GetString(resultBytes);

            // Then
            _output.WriteLine("Serialized JSON Bytes (Indented):");
            _output.WriteLine(result);

            // インデント付き出力の検証
            Assert.NotNull(resultBytes);
            Assert.Contains("\n", result); // 改行が含まれる
            Assert.Contains("  ", result); // インデントが含まれる
        }

        /// <summary>
        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容:Formatting=Indented でシリアライズしたJSONがデシリアライズ可能であることを確認
        /// </summary>
        [Fact]
        public void Serialize_WithIndented_CanDeserialize()
        {
            // Given
            var serializer = new SysRuntimeJsonSerializer();
            var testData = new TestData_SRJ();
            var settings = new JsonSerializerSettings
            {
                Formatting = JsonFormatting.Indented
            };

            // When
            var serialized = serializer.Serialize(testData, settings);
            var deserialized = serializer.Deserialize<TestData_SRJ>(serialized);

            // Then
            _output.WriteLine("Serialized JSON (Indented):");
            _output.WriteLine(serialized);

            // ラウンドトリップの検証
            Assert.NotNull(deserialized);
            Assert.Equal(testData._ALPHABETS, deserialized._ALPHABETS);
            Assert.Equal(testData._alphabets, deserialized._alphabets);
            Assert.Equal(testData._Controls, deserialized._Controls);
            Assert.Equal(testData._Japanese, deserialized._Japanese);
            Assert.Equal(testData._Kanas, deserialized._Kanas);
            Assert.Equal(testData._Numbers, deserialized._Numbers);
            Assert.Equal(testData._Symbols, deserialized._Symbols);
            Assert.Equal(testData._Symbols2B, deserialized._Symbols2B);
        }

        /// <summary>
        /// SysRuntimeJsonSerializerTest のテスト
        /// テスト内容:settings が null の場合、デフォルト動作(コンパクト出力)になることを確認
        /// </summary>
        [Fact]
        public void Serialize_WithNullSettings_ReturnsCompactJson()
        {
            // Given
            var serializer = new SysRuntimeJsonSerializer();
            var testData = new TestData_SRJ();

            // When
            var result = serializer.Serialize(testData, null);

            // Then
            _output.WriteLine("Serialized JSON (null settings):");
            _output.WriteLine(result);

            // コンパクト出力の検証
            Assert.NotNull(result);
            Assert.DoesNotContain("\n", result); // 改行なし
        }
    }
}
