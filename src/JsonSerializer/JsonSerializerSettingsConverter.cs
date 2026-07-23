using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// <see cref="JsonSerializerSettings"/> を Newtonsoft.Json の
    /// <see cref="Newtonsoft.Json.JsonSerializerSettings"/> へ変換する内部ユーティリティ。
    /// </summary>
    internal static class JsonSerializerSettingsConverter
    {
        /// <summary>
        /// <see cref="JsonSerializerSettings"/> を <see cref="Newtonsoft.Json.JsonSerializerSettings"/> に変換する。
        /// </summary>
        internal static Newtonsoft.Json.JsonSerializerSettings ToNewtonsoftSettings(this JsonSerializerSettings source)
        {
            return new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = (NullValueHandling)source.NullValueHandling,
                DefaultValueHandling = (DefaultValueHandling)source.DefaultValueHandling,
                Formatting = (Formatting)source.Formatting,
                ReferenceLoopHandling = (ReferenceLoopHandling)source.ReferenceLoopHandling,
                DateParseHandling = (DateParseHandling)source.DateParseHandling,
                DateFormatHandling = (DateFormatHandling)source.DateFormatHandling,
                StringEscapeHandling = (StringEscapeHandling)source.StringEscapeHandling,
                ContractResolver = source.ExcludeEmptyCollections
                    ? new ExcludeEmptyContractResolver()
                    : null,
            };
        }
    }

    /// <summary>
    /// 空文字列・空コレクションをシリアライズ対象から除外する ContractResolver。
    /// <see cref="JsonSerializerSettings.ExcludeEmptyCollections"/> が <c>true</c> の場合に使用される。
    /// </summary>
    internal sealed class ExcludeEmptyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var originalShouldSerialize = property.ShouldSerialize;

            property.ShouldSerialize = instance =>
            {
                if (instance == null) return false;

                if (originalShouldSerialize != null && !originalShouldSerialize(instance)) return false;

                var value = GetPropertyValue(instance, member);

                if (value == null) return false;

                if (value is string str && string.IsNullOrEmpty(str)) return false;

                if (value is ICollection collection && collection.Count == 0) return false;

                return true;
            };

            return property;
        }

        private static object GetPropertyValue(object instance, MemberInfo member)
        {
            if (member is PropertyInfo prop) return prop.GetValue(instance);
            if (member is FieldInfo field) return field.GetValue(instance);
            return null;
        }
    }
}
