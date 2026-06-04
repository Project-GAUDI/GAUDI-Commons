namespace TICO.GAUDI.Commons.Test.MessageTest.JsonMessageTest;

public abstract class JsonMessageTesterBase
{
    protected const string ENVNAME_DEFAULT_SERIALIZER = "IOTEDGE_COMMON_DEFAULT_JSONSERIALIZER";

    protected const string ENVVALUE_SERIALIZER_NEWTONSOFT = "NEWTONSOFTJSON";

    protected static JsonMessage CreateDummyMessage()
    {
        var obj = new JsonMessage() { RecordList = [] };
        for (var i = 0; i < 3; ++i)
        {
            obj.RecordList.Add(CreateDummyRecord());
        }
        obj.RecordList.AddRange(Enumerable.Range(0, 3).Select(_ => CreateDummyRecord()));
        return obj;
    }

    protected static JsonMessage.RecordInfo CreateDummyRecord()
    {
            var record = new JsonMessage.RecordInfo() { RecordHeader = [], RecordData = [] };
            for (var j = 0; j < 5; ++j)
            {
                record.RecordHeader.Add(Guid.NewGuid().ToString());
                record.RecordData.Add(Guid.NewGuid().ToString());
            }
            return record;
    }
}
