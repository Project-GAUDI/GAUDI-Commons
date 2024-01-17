using System;
using Xunit;
using Xunit.Abstractions;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TICO.GAUDI.Commons
{
    public class JsonMessageTest
    {
        private readonly ITestOutputHelper _output;


        public JsonMessageTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private string message;
        JsonMessage resultJsonMessage;
        JsonMessage.RecordInfo resultRecordInfo;

        string resultMessage;

        ///JSONメッセージのデシリアライズ
        ///その１
        [Fact]
        public void DeserializeJsonMessageTest001()
        {

        //Given

            message = @"{""RecordList"":[
				{
					""RecordHeader"":[
                        ""Test : TEST"",
                        ""test header1"",
                        ""test header2""
                    ],
					""RecordData"":[
                        ""test data1"",
                        ""test data2"",
                        ""test data3""
                    ]
				}
			]}";
        //When
            resultJsonMessage = JsonMessage.DeserializeJsonMessage(message);

        //Then

           List<JsonMessage.RecordInfo> recordInfos = resultJsonMessage.RecordList;
            int recordCount;
            int headerCount;
            int dataCount;

            recordCount = 0;
           foreach(JsonMessage.RecordInfo o in recordInfos){
                recordCount++;
                headerCount = 0;
                dataCount = 0;

               foreach(string header in o.RecordHeader){
                   if("Test : TEST".Equals(header)){
                       Assert.Equal("Test : TEST", header);
                       headerCount++;
                   }else if("test header1".Equals(header)){
                       Assert.Equal("test header1", header);
                       headerCount++;
                   }else if("test header2".Equals(header)){
                        Assert.Equal("test header2", header);
                       headerCount++;
                   }else{
                       Assert.True(false);//いずれにも該当しない場合エラーとする
                   }
               }

               Assert.True(3 == headerCount);//ヘッダーが３件であることを確認する。

               foreach(string header in o.RecordData){
                   if("test data1".Equals(header)){
                       Assert.Equal("test data1", header);
                       dataCount++;
                   }else if("test data2".Equals(header)){
                       Assert.Equal("test data2", header);
                       dataCount++;
                   }else if("test data3".Equals(header)){
                        Assert.Equal("test data3", header);
                       dataCount++;
                   }else{
                       Assert.True(false);//いずれにも該当しない場合エラーとする
                   }
               }
               Assert.True(3 == dataCount);//データが３件であることを確認する。
           }
           Assert.True(1 == recordCount);//レコード件数が１件であることを確認する。
        }

        ///JSONメッセージのデシリアライズ
        //異常系 NULLを返す。
        [Fact]
        public void DeserializeJsonMessageErrTest001()
        {
        //var w = new System.IO.StringWriter();
        //Console.SetOut(w);

        //Given
            //JSONの構文が不正（カンマがない）
            message = @"{""RecordList"":[
				{
					""RecordHeader"":[
                        ""Test : TEST""
                        ""test header1""
                        ""test header2""
                    ]
					""RecordData"":[
                        ""test data1""
                        ""test data2""
                        ""test data3""
                    ]
				}
			]}";
        //When
            resultJsonMessage = JsonMessage.DeserializeJsonMessage(message);

        //Then

           Assert.True(null == resultJsonMessage);
        }

        ///JSONメッセージのデシリアライズ
        ///異常系その２
        ///空文字のデシリアライズ => nullを返す
        [Fact]
        public void DeserializeJsonMessageErrTest002()
        {

        //Given
            message = "";

        //When
            resultJsonMessage = JsonMessage.DeserializeJsonMessage(message);

        //Then
           Assert.True(null == resultJsonMessage);
        }

        ///JSONメッセージのデシリアライズ
        ///異常系その3
        ///NULLのデシリアライズ => 例外ArgumentNullExceptionを返す
        [Fact]
        public void DeserializeJsonMessageErrTest003()
        {

        //Given
            message = null;

        //When
            resultJsonMessage = JsonMessage.DeserializeJsonMessage(message);
            Assert.True(resultJsonMessage == null);
        }

        ///JSONメッセージ(Byte)のデシリアライズ
        ///異常系その1
        ///NULLのデシリアライズ => nullを返す
        [Fact]
        public void DeserializeJsonMessageByteErrTest()
        {

        //Given
            Byte[] byteMessage = null;

        //When
            resultJsonMessage = JsonMessage.DeserializeJsonMessage(byteMessage);

        //Then
            Assert.True(null == resultJsonMessage);

        }

        ///JSONメッセージのシリアライズ
        ///正常系　その１
        ///RecordList、RecordHeader、RecordDataの要素がある　=> デシリアライズ後、シリアライズしなおしてもとの文字列に戻る。
        [Fact]
        public void SerializeJsonMessageTest001()
        {
        //Given
            //デシリアライズする。
            StringBuilder builder = new StringBuilder();

            builder.Append("{\"RecordList\":[");
				builder.Append("{");
					builder.Append("\"RecordHeader\":[");
                        builder.Append("\"Test : TEST\",");
                        builder.Append("\"test header1\",");
                        builder.Append("\"test header2\"");
                    builder.Append("],");
					builder.Append("\"RecordData\":[");
                        builder.Append("\"test data1\",");
                        builder.Append("\"test data2\",");
                        builder.Append("\"test data3\"");
                    builder.Append("]");
				builder.Append("}");
			builder.Append("]}");

            message = builder.ToString();

            JsonMessage jsonMessage;
            var serializer = new DataContractJsonSerializer(typeof(JsonMessage));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                jsonMessage = (JsonMessage)serializer.ReadObject(ms);
            }

        //When
            resultMessage = JsonMessage.SerializeJsonMessage(jsonMessage);

        //Then
            Assert.Equal(message, resultMessage);
        }

        ///JSONメッセージのシリアライズ
        ///正常系　その２
        ///RecordListに要素がない場合 =>　{"RecordList":null}　を返す
        [Fact]
        public void SerializeJsonMessageTest002()
        {
        //Given
            JsonMessage jsonMessage = new JsonMessage();

        //When
            resultMessage = JsonMessage.SerializeJsonMessage(jsonMessage);

        //Then
            Assert.Equal("{\"RecordList\":null}",resultMessage);
        }

        ///JSONメッセージのシリアライズ
        ///正常系　その２
        ///RecordHeader, Recorddataに要素がない場合 =>　{"RecordList":[]}　を返す
        [Fact]
        public void SerializeJsonMessageTest003()
        {
        //Given
            JsonMessage jsonMessage = new JsonMessage();
            jsonMessage.RecordList = new List<Commons.JsonMessage.RecordInfo>();

        //When
            resultMessage = JsonMessage.SerializeJsonMessage(jsonMessage);

        //Then
            Assert.Equal("{\"RecordList\":[]}",resultMessage);
        }

        ///JSONメッセージのシリアライズ
        ///異常系　その１
        ///不正なオブジェクト => NULLを返す
        [Fact]
        public void SerializeJsonMessageErrTest001()
        {
        //Given
            JsonMessage jsonMessage = (JsonMessage)(new JsonmessageDummy());

        //When
            resultMessage = JsonMessage.SerializeJsonMessage(jsonMessage);

        //Then
            // 旧API仕様
            // Assert.True(resultMessage == null);

            // 新API仕様
            Assert.Equal("{\"RecordList\":null}", resultMessage);
        }

        ///JsonMessageのシリアライズ
        ///異常系　その２
        ///(string)NULLをシリアライズする => nullを返す
        [Fact]
        public void SerializeJsonMessageErrTest002(){
        //Given

           JsonMessage jsonMessage = null;

        //When
            String resultMessage = JsonMessage.SerializeJsonMessage(jsonMessage);

        //Then
            Assert.True(resultMessage == null);

        }

        ///JSONメッセージのシリアライズ
        ///異常系 その１
        ///不正なオブジェクト => NULLを返す
        [Fact]
        public void SerializeJsonMessageByteErrTest001()
        {
            byte[] resultByteMessage;
        //Given
            JsonMessage jsonMessage = (JsonMessage)(new JsonmessageDummy());

        //When
            resultByteMessage = JsonMessage.SerializeJsonMessageByte(jsonMessage);

        //Then
            // 旧API仕様
            // Assert.True(null == resultByteMessage);

            // 新API仕様
            Assert.Equal("{\"RecordList\":null}", System.Text.Encoding.UTF8.GetString(resultByteMessage));
        }


        ///JsonMessageのシリアライズ
        ///異常系　その２
        ///(byte[])NULLをシリアライズする => nullを返す
        [Fact]
        public void SerializeJsonMessageByteErrTest002(){
        //Given

           JsonMessage jsonMessage = null;

        //When
            byte[] resultByte = JsonMessage.SerializeJsonMessageByte(jsonMessage);

        //Then
            Assert.True(resultByte == null);

        }

        ///RecordInfoのデシリアライズ
        ///正常系
        ///任意のJSON文字列
        [Fact]
        public void DeserializeRecordInfoTest(){
        //Given
            string messageRecordInfo = @"{

					""RecordHeader"":[
                        ""Test : TEST"",
                        ""test header1"",
                        ""test header2""
                    ],
					""RecordData"":[
                        ""test data1"",
                        ""test data2"",
                        ""test data3""
                    ]

			}";

        //When
            resultRecordInfo = JsonMessage.DeserializeRecordInfo(messageRecordInfo);

        //Then
            //int recordCount;
            int headerCount;
            int dataCount;

            headerCount = 0;
            dataCount = 0;

            if(resultRecordInfo.RecordHeader == null){
                return;
            }

            foreach(string header in resultRecordInfo.RecordHeader){
                if("Test : TEST".Equals(header)){
                    Assert.Equal("Test : TEST", header);
                    headerCount++;
                }else if("test header1".Equals(header)){
                    Assert.Equal("test header1", header);
                    headerCount++;
                }else if("test header2".Equals(header)){
                    Assert.Equal("test header2", header);
                    headerCount++;
                }else{
                    Assert.True(false);//いずれにも該当しない場合エラーとする
                }
            }

            Assert.True(3 == headerCount);//ヘッダーが３件であることを確認する。

            foreach(string header in resultRecordInfo.RecordData){
                if("test data1".Equals(header)){
                    Assert.Equal("test data1", header);
                    dataCount++;
                }else if("test data2".Equals(header)){
                    Assert.Equal("test data2", header);
                    dataCount++;
                }else if("test data3".Equals(header)){
                    Assert.Equal("test data3", header);
                    dataCount++;
                }else{
                    Assert.True(false);//いずれにも該当しない場合エラーとする
                }
            }
            Assert.True(3 == dataCount);//データが３件であることを確認する。

        }

        ///RecordInfoのデシリアライズ
        ///異常系 NULLを返す
        [Fact]
        public void DeserializeRecordInfoErrTest001(){
        //Given
            //JSON構文が不正（カンマがない）
            string messageRecordInfo = @"{

					""RecordHeader"":[
                        ""Test : TEST""
                        ""test header1""
                        ""test header2""
                    ],
					""RecordData"":[
                        ""test data1""
                        ""test data2""
                        ""test data3""
                    ]

			}";

        //When
            resultRecordInfo = JsonMessage.DeserializeRecordInfo(messageRecordInfo);

        //Then
           Assert.True(resultRecordInfo == null);

        }

        ///RecordInfoのデシリアライズ
        ///異常系
        ///から文字のデシリアライズ => NULLを返す
        [Fact]
        public void DeserializeRecordInfoErrTest002(){
        //Given

            string messageRecordInfo = "";

        //When
            resultRecordInfo = JsonMessage.DeserializeRecordInfo(messageRecordInfo);

        //Then
            Assert.True(resultRecordInfo == null);

        }

        ///RecordInfoのデシリアライズ
        ///異常系
        ///(string)nullのデシリアライズ => 例外を返す
        [Fact]
        public void DeserializeRecordInfoErrTest003(){
        //Given

            string messageRecordInfo = null;

        //When

            resultRecordInfo = JsonMessage.DeserializeRecordInfo(messageRecordInfo);
            Assert.True(resultRecordInfo == null);

        }

        ///RecordInfoのデシリアライズ
        ///異常系
        ///(Byte)nullのデシリアライズ => NULLを返す
        [Fact]
        public void DeserializeRecordInfoByteErrTest(){
        //Given
            byte[] messageRecordInfo = null;

        //When
            resultRecordInfo = JsonMessage.DeserializeRecordInfo(messageRecordInfo);

        //Then
            Assert.True(null == resultRecordInfo);

        }

        ///RecordInfoのシリアライズ
        ///正常系
        ///その１
        [Fact]
        public void SerializeRecordInfoTest001(){
        //Given

            //デシリアライズされたJsonMessageを作成する
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
					builder.Append("\"RecordHeader\":[");
                        builder.Append("\"Test : TEST\",");
                        builder.Append("\"test header1\",");
                        builder.Append("\"test header2\"");
                    builder.Append("],");
					builder.Append("\"RecordData\":[");
                        builder.Append("\"test data1\",");
                        builder.Append("\"test data2\",");
                        builder.Append("\"test data3\"");
                     builder.Append("]");
			builder.Append("}");

            message = builder.ToString();

            var serializer = new DataContractJsonSerializer(typeof(JsonMessage.RecordInfo));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(message)))
                {
                    resultRecordInfo = (JsonMessage.RecordInfo)serializer.ReadObject(ms);
                }

        //When
            //シリアライズする
            resultMessage = JsonMessage.SerializeRecordInfo(resultRecordInfo);

        //Then
            Assert.Equal(message, resultMessage);
        }

        ///RecordInfoのシリアライズ
        ///正常系
        ///その２
        ///RecordHeader,RecordDataがNULL => {"RecordHeader":null,"RecordData":null} を返す
        [Fact]
        public void SerializeRecordInfoTest002(){
        //Given

            JsonMessage.RecordInfo resultRecordInfo = new JsonMessage.RecordInfo();

        //When
            //シリアライズする
            resultMessage = JsonMessage.SerializeRecordInfo(resultRecordInfo);

        //Then
            Assert.Equal("{\"RecordHeader\":null,\"RecordData\":null}", resultMessage);
        }

        ///RecordInfoのシリアライズ
        ///正常系
        ///その3
        ///RecordHeader,RecordDataの要素がない => {"RecordHeader":[],"RecordData":[]} を返す
        [Fact]
        public void SerializeRecordInfoTest003(){
        //Given

            JsonMessage.RecordInfo resultRecordInfo = new JsonMessage.RecordInfo();
            resultRecordInfo.RecordData = new List<string>();
            resultRecordInfo.RecordHeader = new List<string>();

        //When
            //シリアライズする
            resultMessage = JsonMessage.SerializeRecordInfo(resultRecordInfo);

        //Then
            Assert.Equal("{\"RecordHeader\":[],\"RecordData\":[]}", resultMessage);
        }

        ///RecordInfoのシリアライズ
        ///異常系　
        ///不正なオブジェクト => nullを返す
        [Fact]
        public void SerializeRecordInfoErrTest001(){
        //Given
           JsonMessage.RecordInfo recordInfo = (JsonMessage.RecordInfo)(new RecordInfoDummy());

        //When
            resultMessage = JsonMessage.SerializeRecordInfo(recordInfo);

        //Then
            // 旧API仕様
            // Assert.True(resultMessage == null);

            // 新API
            Assert.Equal("{\"RecordHeader\":null,\"RecordData\":null}", resultMessage);
        }

        ///RecordInfoのシリアライズ
        ///異常系　
        ///(string)NULLをシリアライズする => nullを返す
        [Fact]
        public void SerializeRecordInfoErrTest002(){
        //Given

           JsonMessage.RecordInfo recordInfo = null;

        //When
            String resultMessage = JsonMessage.SerializeRecordInfo(recordInfo);

        //Then
            Assert.True(resultMessage == null);

        }

        ///RecordInfoByteのシリアライズ
        ///異常系　
        ///不正なオブジェクト => nullを返す
        [Fact]
        public void SerializeRecordInfoByteErrTest001(){
        //Given

           JsonMessage.RecordInfo recordInfo = (JsonMessage.RecordInfo)(new RecordInfoDummy());

        //When
            byte[] resultByteMessage = JsonMessage.SerializeRecordInfoByte(recordInfo);

        //Then
            // 旧API仕様
            // Assert.True(resultByteMessage == null);

            // 新API
            Assert.Equal("{\"RecordHeader\":null,\"RecordData\":null}", System.Text.Encoding.UTF8.GetString(resultByteMessage));
        }

        ///RecordInfoのシリアライズ
        ///異常系　
        ///(byte[])NULLをシリアライズする => nullを返す
        [Fact]
        public void SerializeRecordInfoByteErrTest002(){
        //Given

           JsonMessage.RecordInfo recordInfo = null;

        //When
            byte[] resultByte = JsonMessage.SerializeRecordInfoByte(recordInfo);

        //Then
            Assert.True(resultByte == null);

        }
    }

    /*******************************************************
     テスト用
     *******************************************************/

    ///エラーを返すためのダミークラス
    [DataContract]
    public class JsonmessageDummy: JsonMessage{

    }

    ///エラーを返すためのダミークラス
    [DataContract]
    public class RecordInfoDummy: JsonMessage.RecordInfo{

    }
}
