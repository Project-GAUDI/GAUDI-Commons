namespace TICO.GAUDI.Commons
{
    /// <summary>null 値のシリアライズ方法。</summary>
    /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
    public enum JsonNullValueHandling
    {
        /// <summary>null 値をシリアライズ対象に含める。</summary>
        Include = 0,
        /// <summary>null 値をシリアライズ対象から除外する。</summary>
        Ignore = 1,
    }

    /// <summary>デフォルト値のシリアライズ方法。</summary>
    /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
    public enum JsonDefaultValueHandling
    {
        /// <summary>デフォルト値を含める。</summary>
        Include = 0,
        /// <summary>デフォルト値を除外する。</summary>
        Ignore = 1,
        /// <summary>デシリアライズ時にデフォルト値を補完する。</summary>
        Populate = 2,
        /// <summary>デフォルト値を除外し、デシリアライズ時に補完する。</summary>
        IgnoreAndPopulate = 3,
    }

    /// <summary>JSON 出力フォーマット。</summary>
    public enum JsonFormatting
    {
        /// <summary>コンパクト出力。</summary>
        None = 0,
        /// <summary>インデント付き出力。</summary>
        Indented = 1,
    }

    /// <summary>循環参照の処理方法。</summary>
    /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
    public enum JsonReferenceLoopHandling
    {
        /// <summary>循環参照が検出された場合に例外をスローする。</summary>
        Error = 0,
        /// <summary>循環参照を無視する。</summary>
        Ignore = 1,
        /// <summary>循環参照をシリアライズする（無限ループに注意）。</summary>
        Serialize = 2,
    }

    /// <summary>日付文字列の自動パース方式。</summary>
    /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
    public enum JsonDateParseHandling
    {
        /// <summary>日付文字列を自動変換しない。</summary>
        None = 0,
        /// <summary>日付文字列を <see cref="System.DateTime"/> に自動変換する。</summary>
        DateTime = 1,
        /// <summary>日付文字列を <see cref="System.DateTimeOffset"/> に自動変換する。</summary>
        DateTimeOffset = 2,
    }

    /// <summary>日付のシリアライズ形式。</summary>
    /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
    public enum JsonDateFormatHandling
    {
        /// <summary>ISO 8601 形式（例: <c>2025-01-23T12:34:56Z</c>）。</summary>
        IsoDateFormat = 0,
        /// <summary>Microsoft JSON 形式（例: <c>\/Date(1234567890000)\/</c>）。</summary>
        MicrosoftDateFormat = 1,
    }

    /// <summary>文字列のエスケープ方式。</summary>
    /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
    public enum JsonStringEscapeHandling
    {
        /// <summary>ASCII 制御文字と Unicode サロゲート文字のみエスケープする。</summary>
        Default = 0,
        /// <summary>非 ASCII 文字をすべて <c>\uXXXX</c> 形式にエスケープする。</summary>
        EscapeNonAscii = 1,
        /// <summary>HTML 予約文字（<c>&lt;</c>, <c>&gt;</c>, <c>&amp;</c> など）をエスケープする。</summary>
        EscapeHtml = 2,
    }

    // ── 設定クラス ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Newtonsoft.Json / SysRuntimeSerialization 両方で使用できる、
    /// ライブラリ非依存の JSON シリアライザ設定クラス。
    /// </summary>
    /// <remarks>
    /// <para>
    /// Newtonsoft.Json が選択されている場合、すべてのプロパティが有効です。
    /// </para>
    /// <para>
    /// SysRuntimeSerialization (<see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>)
    /// が選択されている場合、<see cref="Formatting"/> プロパティ（シリアライズ時のみ）がサポートされます。
    /// それ以外の設定はサポートされておらず無視されます。
    /// 非デフォルト値が指定されているプロパティは
    /// シリアライズ/デシリアライズ実行時に <see cref="System.Diagnostics.Trace.TraceWarning(string)"/>
    /// で警告が出力されます。
    /// </para>
    /// </remarks>
    public sealed class JsonSerializerSettings
    {
        /// <summary>null 値の扱い。デフォルト: <see cref="JsonNullValueHandling.Include"/>。</summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public JsonNullValueHandling NullValueHandling { get; set; } = JsonNullValueHandling.Include;

        /// <summary>
        /// デフォルト値の扱い。デフォルト: <see cref="JsonDefaultValueHandling.Include"/>。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public JsonDefaultValueHandling DefaultValueHandling { get; set; } = JsonDefaultValueHandling.Include;

        /// <summary>
        /// 出力フォーマット。デフォルト: <see cref="JsonFormatting.None"/>。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時はシリアライズで有効、デシリアライズでは無視されます。</remarks>
        public JsonFormatting Formatting { get; set; } = JsonFormatting.None;

        /// <summary>
        /// 循環参照の処理方法。デフォルト: <see cref="JsonReferenceLoopHandling.Error"/>。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public JsonReferenceLoopHandling ReferenceLoopHandling { get; set; } = JsonReferenceLoopHandling.Error;

        /// <summary>
        /// 日付文字列の自動パース方式。デフォルト: <see cref="JsonDateParseHandling.DateTime"/>。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public JsonDateParseHandling DateParseHandling { get; set; } = JsonDateParseHandling.DateTime;

        /// <summary>
        /// 日付のシリアライズ形式。デフォルト: <see cref="JsonDateFormatHandling.IsoDateFormat"/>。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public JsonDateFormatHandling DateFormatHandling { get; set; } = JsonDateFormatHandling.IsoDateFormat;

        /// <summary>
        /// 文字列のエスケープ方式。デフォルト: <see cref="JsonStringEscapeHandling.Default"/>。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public JsonStringEscapeHandling StringEscapeHandling { get; set; } = JsonStringEscapeHandling.Default;

        /// <summary>
        /// 空文字列・空コレクション（Dictionary, List 等）をシリアライズ対象から除外する。
        /// デフォルト: <c>false</c>（除外しない）。
        /// </summary>
        /// <remarks>SysRuntimeSerialization 選択時は無視されます。</remarks>
        public bool ExcludeEmptyCollections { get; set; } = false;
    }
}
