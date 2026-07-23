using System;

namespace TICO.GAUDI.Commons
{
    /// <summary>
    /// シリアライザタイプ
    /// </summary>
    public enum SerializerType
    {
        /// <summary>
        /// デフォルトのシリアライザタイプ
        /// </summary>
        Default,
        /// <summary>
        /// System.Runtime を使用するシリアライザタイプ
        /// </summary>
        SysRuntimeSerialization,
        /// <summary>
        /// Newtonsoft.Json を使用するシリアライザタイプ
        /// </summary>
        NewtonsoftJson
    }

    /// <summary>
    /// Jsonシリアライザファクトリークラス
    /// </summary>
    public class JsonSerializerFactory
    {
        /// <summary>
        /// デフォルトシリアライザ環境変数名
        /// </summary>
        protected const string ENVNAME_DEFAULT_SERIALIZER = "IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER";
        /// <summary>
        /// システムランタイムシリアライザ環境変数値
        /// </summary>
        protected const string ENVVALUE_SERIALIZER_SYSRUNTIME = "SYSRUNTIMESERIALIZATION";
        /// <summary>
        /// NNewtonsoft.Json シリアライザ環境変数値
        /// </summary>
        protected const string ENVVALUE_SERIALIZER_NEWTONSOFT = "NEWTONSOFTJSON";

        /// <summary>
        /// 指定したシリアライザタイプに対応した IJsonSerializer クラスのインスタンスを取得する。
        /// シリアライザタイプが Default の場合、ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER が指定されていれば、それを使用する。
        /// ENV:IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER が指定されていなければ、"NEWTONSOFTJSON" を使用する。
        /// </summary>
        /// <param name="serializerType">シリアライザタイプ（デフォルト：Default）</param>
        /// <returns>IJsonSerializerインスタンス</returns>
        public static IJsonSerializer GetJsonSerializer(SerializerType serializerType = SerializerType.Default)
        {
            IJsonSerializer retSerializer = null;

            // デフォルト指定の場合、有効とするシリアライザタイプを決定する。
            SerializerType validSerializerType = serializerType;
            if (validSerializerType == SerializerType.Default)
            {
                validSerializerType = GetDefaultSerializerType();
            }

            // タイプ毎にシリアライザを取得
            switch (validSerializerType)
            {
                case SerializerType.SysRuntimeSerialization:
                    retSerializer = new SysRuntimeJsonSerializer();
                    break;
                case SerializerType.NewtonsoftJson:
                    retSerializer = new NewtonsoftJsonSerializer();
                    break;
            }

            return retSerializer;
        }

        /// <summary>
        /// デフォルトシリアライザタイプ取得
        /// </summary>
        /// <remarks>
        /// 通常は、NewtonsoftJsonタイプを返す。
        /// 環境変数（”IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER”）で、切り替え可能。
        /// </remarks>
        /// <returns>デフォルトシリアライザタイプ</returns>
        protected static SerializerType GetDefaultSerializerType()
        {
            // 通常のデフォルトNewtonsoftJsonとする
            SerializerType retSerializeType = SerializerType.NewtonsoftJson;

            // 環境変数が設定されている場合、そちらを優先する
            string envDefaultSerializer = Environment.GetEnvironmentVariable(ENVNAME_DEFAULT_SERIALIZER);
            if (envDefaultSerializer != null)
            {
                switch (envDefaultSerializer.ToUpper())
                {
                    case ENVVALUE_SERIALIZER_SYSRUNTIME:
                        retSerializeType = SerializerType.SysRuntimeSerialization;
                        break;
                    case ENVVALUE_SERIALIZER_NEWTONSOFT:
                        retSerializeType = SerializerType.NewtonsoftJson;
                        break;
                }
            }

            return retSerializeType;
        }
    }
}
