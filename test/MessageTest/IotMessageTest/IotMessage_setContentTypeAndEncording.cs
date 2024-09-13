using System;
using System.IO;
using Microsoft.Azure.Devices.Client;
using Xunit;
using TICO.GAUDI.Commons;

namespace TICO.GAUDI.Commons.Test
{
    public class IotMessage_setContentTypeAndEncording
    {

        [Fact]
        public void setContentTypeAndEncodingTest001()
        {
            IotMessage MyIotMessage = new IotMessage();

            Assert.Equal("application/json", MyIotMessage.GetContentType());
            Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());

        }
        [Fact]
        public void setContentTypeAndEncodingTest002()
        {
            Byte[] byteArray = new Byte[10];
            IotMessage MyIotMessage = new IotMessage(byteArray);

            Assert.Equal("application/json", MyIotMessage.GetContentType());
            Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
        }
        [Fact]
        public void setContentTypeAndEncodingTest003()
        {
            string body = "test";
            IotMessage MyIotMessage = new IotMessage(body);

            Assert.Equal("application/json", MyIotMessage.GetContentType());
            Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
        }

        [Fact]
        public void setContentTypeAndEncodingTest004()
        {
            Stream bodyStream = new MemoryStream();
            IotMessage MyIotMessage = new IotMessage(bodyStream);

            Assert.Equal("application/json", MyIotMessage.GetContentType());
            Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
        }

        [Fact]
        public void setContentTypeAndEncodingTest005()
        {
            IotMessage orgMessage = new IotMessage();
            IotMessage MyIotMessage = new IotMessage(orgMessage);

            Assert.Equal("application/json", MyIotMessage.GetContentType());
            Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
        }
        [Fact]
        public void setContentTypeAndEncodingTest006()
        {
            Message orgMessage = new Message();
            IotMessage MyIotMessage = new IotMessage(orgMessage);

            Assert.Equal("application/json", MyIotMessage.GetContentType());
            Assert.Equal("utf-8", MyIotMessage.GetContentEncoding());
        }
        [Fact]
        public void setContentTypeAndEncodingTest007()
        {
            Message orgMessage = new Message();
            orgMessage.ContentType = "testContentType";
            orgMessage.ContentEncoding = "testContentEncoding";
            IotMessage MyIotMessage = new IotMessage(orgMessage);

            Assert.Equal(orgMessage.ContentType, MyIotMessage.GetContentType());
            Assert.Equal(orgMessage.ContentEncoding, MyIotMessage.GetContentEncoding());
        }
    }
}