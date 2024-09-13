using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

using Microsoft.Azure.Devices.Shared;

using Newtonsoft.Json.Linq;

using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    [Collection(nameof(DirectMethodCaller_Run))]
    [CollectionDefinition(nameof(DirectMethodCaller_Run), DisableParallelization = true)]
    public class Util_GetRequiredValue
    {
        private readonly ITestOutputHelper _output;
        private IJsonSerializer jsonSerializer = JsonSerializerFactory.GetJsonSerializer();

        public Util_GetRequiredValue(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "異常系：Itemが全く存在しない")]
        public void NoItems_ExceptionThrown()
        {
            string targetJSON = "{}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<Exception>(() =>
            {
                var item1 = Util.GetRequiredValue<JToken>(rootObj, tergetItem);
            });

            Assert.Equal($"Property {tergetItem} dose not exist.", exp.Message);
        }

        [Fact(DisplayName = "異常系：指定Itemが存在しない")]
        public void NoTargetItem_ExceptionThrown()
        {
            string targetJSON = "{\"item2\":\"value1\"}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<Exception>(() =>
            {
                var item1 = Util.GetRequiredValue<JToken>(rootObj, tergetItem);
            });

            Assert.Equal($"Property {tergetItem} dose not exist.", exp.Message);
        }

        [Fact(DisplayName = "異常系：valueをJObjectで受け取り")]
        public void ValueToJObject_InvalidCastExceptionThrown()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<InvalidCastException>(() =>
            {
                var item1 = Util.GetRequiredValue<JObject>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：objectをJValueで受け取り")]
        public void ObjectToJValue_InvalidCastExceptionThrown()
        {
            string targetJSON = "{\"item1\":{\"value1\":1}}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<InvalidCastException>(() =>
            {
                var item1 = Util.GetRequiredValue<JValue>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：objectをstringで受け取り")]
        public void CantCastToString_InvalidCastExceptionThrown()
        {
            string targetJSON = "{\"item1\":{\"value1\":1}}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<InvalidCastException>(() =>
            {
                var item1 = Util.GetRequiredValue<string>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：objectをintで受け取り")]
        public void ObjectToInt_InvalidCastExceptionThrown()
        {
            string targetJSON = "{\"item1\":{\"value1\":1}}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<InvalidCastException>(() =>
            {
                var item1 = Util.GetRequiredValue<int>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：stringからintに変換できない")]
        public void CantConvertToInt_FormatExceptionThrown()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<FormatException>(() =>
            {
                var item1 = Util.GetRequiredValue<int>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：stringからfloatに変換できない")]
        public void CantConvertToFloat_FormatExceptionThrown()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<FormatException>(() =>
            {
                var item1 = Util.GetRequiredValue<float>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：値がnullでintに変換")]
        public void NullToInt_InvalidCastExceptionThrown()
        {
            string targetJSON = "{\"item1\":null}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<InvalidCastException>(() =>
            {
                var item1 = Util.GetRequiredValue<int>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "異常系：値がnullでfloatに変換")]
        public void NullToFloat_InvalidCastExceptionThrown()
        {
            string targetJSON = "{\"item1\":null}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var exp = Assert.Throws<InvalidCastException>(() =>
            {
                var item1 = Util.GetRequiredValue<float>(rootObj, tergetItem);
            });
        }

        [Fact(DisplayName = "正常系：値がnullでstringに変換")]
        public void NullToString_NullReturned()
        {
            string targetJSON = "{\"item1\":null}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = Util.GetRequiredValue<string>(rootObj, tergetItem);
            Assert.Null(item1);
        }

        [Fact(DisplayName = "正常系：値をintで受け取り")]
        public void IntToInt_ValueReturned()
        {
            string targetJSON = "{\"item1\":9}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = Util.GetRequiredValue<int>(rootObj, tergetItem);
            Assert.Equal(9, item1);
        }

        [Fact(DisplayName = "正常系：値をfloatで受け取り")]
        public void FloatToFloat_ValueReturned()
        {
            string targetJSON = "{\"item1\":1.2360679}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = Util.GetRequiredValue<float>(rootObj, tergetItem);
            Assert.Equal(1.2360679, item1, 7);
        }

        [Fact(DisplayName = "正常系：値をstringで受け取り")]
        public void StringToString_ValueReturned()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = Util.GetRequiredValue<string>(rootObj, tergetItem);
            Assert.Equal("value1", item1);
        }

        [Fact(DisplayName = "正常系：値をobjectで受け取り")]
        public void ObjectToObject_ObjectReturned()
        {
            string targetJSON = "{\"item1\":{\"subitem1\":\"subvalue1\",\"subitem2\":\"subvalue2\"}}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = Util.GetRequiredValue<JObject>(rootObj, tergetItem);
            Assert.Equal("subvalue1", Util.GetRequiredValue<string>(item1, "subitem1"));
            Assert.Equal("subvalue2", Util.GetRequiredValue<string>(item1, "subitem2"));
        }

        // JObjectの動作確認
        [Fact(DisplayName = "正常系：値をindexで受け取り")]
        public void StringByIndex_ValueReturned()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item1";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = rootObj[tergetItem];
            Assert.Equal("value1", item1);
        }

        // JObjectの動作確認
        [Fact(DisplayName = "正常系：値を無いindexで受け取り")]
        public void StringByMissingIndex_ValueReturned()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item2";
            JObject rootObj = jsonSerializer.Deserialize<JObject>(targetJSON);

            var item1 = rootObj[tergetItem];
            Assert.Null(item1);
        }

        // TwinCollectionの動作確認
        [Fact(DisplayName = "異常系：値を無いindexで受け取り")]
        public void StringByMissingIndexFromTwinCollection_ValueReturned()
        {
            string targetJSON = "{\"item1\":\"value1\"}";
            string tergetItem = "item2";
            var rootObj = new TwinCollection(targetJSON);

            var exp = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var item1 = rootObj[tergetItem];
            });
        }
    }
}
