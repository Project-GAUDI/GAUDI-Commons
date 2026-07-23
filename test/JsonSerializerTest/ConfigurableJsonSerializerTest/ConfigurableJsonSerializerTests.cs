using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class ConfigurableJsonSerializerTests
    {
        [Fact]
        public void GetJsonSerializer_ReturnsIntegratedSerializerImplementation()
        {
            IJsonSerializer serializer = JsonSerializerFactory.GetJsonSerializer();

            Assert.True(
                serializer is NewtonsoftJsonSerializer || serializer is SysRuntimeJsonSerializer,
                $"Unexpected serializer type: {serializer?.GetType().FullName}");
        }

        [Fact]
        public void ToNewtonsoftSettings_MapsConfiguredValues()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = JsonNullValueHandling.Ignore,
                DefaultValueHandling = JsonDefaultValueHandling.IgnoreAndPopulate,
                Formatting = JsonFormatting.Indented,
                ReferenceLoopHandling = JsonReferenceLoopHandling.Ignore,
                DateParseHandling = JsonDateParseHandling.None,
                DateFormatHandling = JsonDateFormatHandling.MicrosoftDateFormat,
                StringEscapeHandling = JsonStringEscapeHandling.EscapeHtml,
                ExcludeEmptyCollections = true,
            };

            Newtonsoft.Json.JsonSerializerSettings converted = settings.ToNewtonsoftSettings();

            Assert.Equal(NullValueHandling.Ignore, converted.NullValueHandling);
            Assert.Equal(DefaultValueHandling.IgnoreAndPopulate, converted.DefaultValueHandling);
            Assert.Equal(Formatting.Indented, converted.Formatting);
            Assert.Equal(ReferenceLoopHandling.Ignore, converted.ReferenceLoopHandling);
            Assert.Equal(DateParseHandling.None, converted.DateParseHandling);
            Assert.Equal(DateFormatHandling.MicrosoftDateFormat, converted.DateFormatHandling);
            Assert.Equal(StringEscapeHandling.EscapeHtml, converted.StringEscapeHandling);
            Assert.IsType<ExcludeEmptyContractResolver>(converted.ContractResolver);
        }

        [Fact]
        public void NewtonsoftSerialize_AppliesNullAndEmptyValueSettings()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var target = new NewtonsoftModel
            {
                Name = "device",
                OptionalText = null,
                EmptyText = string.Empty,
                EmptyValues = new List<int>(),
                Values = new List<int> { 1 },
            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = JsonNullValueHandling.Ignore,
                ExcludeEmptyCollections = true,
            };

            string json = serializer.Serialize(target, settings);

            Assert.Contains("\"Name\":\"device\"", json);
            Assert.Contains("\"Values\":[1]", json);
            Assert.DoesNotContain("\"OptionalText\"", json);
            Assert.DoesNotContain("\"EmptyText\"", json);
            Assert.DoesNotContain("\"EmptyValues\"", json);
        }

        [Fact]
        public void NewtonsoftSerialize_ExcludeEmptyCollectionsPreservesModelShouldSerialize()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var target = new ConditionalSerializationModel
            {
                Name = "device",
                InternalValue = "must-not-be-serialized",
            };
            var settings = new JsonSerializerSettings
            {
                ExcludeEmptyCollections = true,
            };

            string json = serializer.Serialize(target, settings);

            Assert.Contains("\"Name\":\"device\"", json);
            Assert.DoesNotContain("\"InternalValue\"", json);
        }

        [Fact]
        public void NewtonsoftDeserialize_DateParseNonePreservesDateText()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = JsonDateParseHandling.None,
            };

            Dictionary<string, object> result = serializer.Deserialize<Dictionary<string, object>>(
                "{\"Timestamp\":\"2026-07-22T12:34:56Z\"}",
                settings);

            Assert.NotNull(result);
            Assert.Equal("2026-07-22T12:34:56Z", Assert.IsType<string>(result["Timestamp"]));
        }

        [Fact]
        public void NewtonsoftDeserializeBytes_DateParseNonePreservesDateText()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = JsonDateParseHandling.None,
            };
            byte[] jsonBytes = Encoding.UTF8.GetBytes("{\"Timestamp\":\"2026-07-22T12:34:56Z\"}");

            Dictionary<string, object> result = serializer.Deserialize<Dictionary<string, object>>(
                jsonBytes,
                settings);

            Assert.NotNull(result);
            Assert.Equal("2026-07-22T12:34:56Z", Assert.IsType<string>(result["Timestamp"]));
        }

        [Fact]
        public void NewtonsoftSerializeBytes_AppliesFormattingSetting()
        {
            var serializer = new NewtonsoftJsonSerializer();
            var settings = new JsonSerializerSettings
            {
                Formatting = JsonFormatting.Indented,
            };

            byte[] bytes = serializer.SerializeBytes(
                new NewtonsoftModel { Name = "device" },
                settings);

            Assert.NotNull(bytes);
            Assert.Contains("\n", Encoding.UTF8.GetString(bytes));
        }

        [Fact]
        public void SysRuntimeSerialize_SupportsFormattingIndented()
        {
            var serializer = new SysRuntimeJsonSerializer();
            var target = new RuntimeModel { Name = "device", Value = 1 };
            var settings = new JsonSerializerSettings
            {
                Formatting = JsonFormatting.Indented,
            };
            var listener = new RecordingTraceListener();
            Trace.Listeners.Add(listener);

            try
            {
                string baseline = serializer.Serialize(target);
                string actual = serializer.Serialize(target, settings);

                // Formatting=Indented はサポートされるため、インデント付き出力になる
                Assert.NotEqual(baseline, actual);
                Assert.Contains("\n", actual);  // 改行が含まれる
                Assert.Contains("  ", actual);  // インデントが含まれる

                // Formatting はサポートされているため警告されない
                Assert.DoesNotContain("Formatting", listener.Output);
            }
            finally
            {
                Trace.Listeners.Remove(listener);
                listener.Dispose();
            }
        }

        private sealed class NewtonsoftModel
        {
            public string Name { get; set; }
            public string OptionalText { get; set; }
            public string EmptyText { get; set; }
            public List<int> EmptyValues { get; set; }
            public List<int> Values { get; set; }
        }

        private sealed class ConditionalSerializationModel
        {
            public string Name { get; set; }
            public string InternalValue { get; set; }

            public bool ShouldSerializeInternalValue()
            {
                return false;
            }
        }

        [DataContract]
        private sealed class RuntimeModel
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public int Value { get; set; }
        }

        private sealed class RecordingTraceListener : TraceListener
        {
            private readonly StringBuilder _output = new StringBuilder();

            public string Output => _output.ToString();

            public override void Write(string message)
            {
                _output.Append(message);
            }

            public override void WriteLine(string message)
            {
                _output.AppendLine(message);
            }
        }
    }
}
