using System;


namespace TICO.GAUDI.Commons
{
    public enum SerializerType {
        Default,
        SysRuntimeSerialization,
        NewtonsoftJson
    }

    public class JsonSerializerFactory 
    {
        protected const string ENVNAME_DEFAULT_SERIALIZER = "IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER";
        protected const string ENVVALUE_SERIALIZER_SYSRUNTIME = "SYSRUNTIMESERIALIZATION";
        protected const string ENVVALUE_SERIALIZER_NEWTONSOFT = "NEWTONSOFTJSON";

        public static IJsonSerializer GetJsonSerializer(SerializerType serializerType = SerializerType.Default)
        {
            IJsonSerializer retSerializer = null;

            // デフォルト指定の場合、有効とするシリアライザタイプを決定する。
            SerializerType validSerializerType = serializerType;
            if (validSerializerType == SerializerType.Default) {
                validSerializerType = GetDefaultSerializerType();
            }

            // タイプ毎にシリアライザを取得
            switch (validSerializerType){
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
            if ( envDefaultSerializer != null ) {
                switch ( envDefaultSerializer.ToUpper() ) {
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
