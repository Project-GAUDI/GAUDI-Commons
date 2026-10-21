using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TICO.GAUDI.Commons.Test;

/// <summary>SysRuntimeJsonSerializer系テストで共有する補助ロジック。</summary>
internal static class SysRuntimeJsonSerializerTestHelper
{
    public static string BuildIndentedJson<T>(T target)
    {
        var serializer = new DataContractJsonSerializer(typeof(T));
        using var ms = new MemoryStream();
        using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, ownsStream: false, indent: true))
        {
            serializer.WriteObject(writer, target);
            writer.Flush();
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    /// <summary>Trace.TraceWarning等の出力をテスト中だけ捕捉するリスナー。</summary>
    public sealed class TraceCapture : TraceListener
    {
        public List<string> Messages { get; } = new();

        public TraceCapture()
        {
            Trace.Listeners.Add(this);
        }

        public override void Write(string? message)
        {
            if (message != null) Messages.Add(message);
        }

        public override void WriteLine(string? message)
        {
            if (message != null) Messages.Add(message);
        }

        protected override void Dispose(bool disposing)
        {
            Trace.Listeners.Remove(this);
            base.Dispose(disposing);
        }
    }
}
