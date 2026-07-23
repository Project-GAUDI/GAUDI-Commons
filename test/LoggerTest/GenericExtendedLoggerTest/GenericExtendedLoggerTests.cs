#nullable enable

using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace TICO.GAUDI.Commons.Test
{
    public class GenericExtendedLoggerTests
    {
        [Fact]
        public void Constructor_ThrowsWhenInnerLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtendedLogger<TestCategory>(null!));
        }

        [Fact]
        public void InnerLogger_ReturnsLoggerPassedToConstructor()
        {
            var innerLogger = new RecordingLogger<TestCategory>();
            var logger = new ExtendedLogger<TestCategory>(innerLogger);

            Assert.Same(innerLogger, logger.InnerLogger);
        }

        [Theory]
        [InlineData(LogLevel.Trace, "7")]
        [InlineData(LogLevel.Debug, "6")]
        [InlineData(LogLevel.Information, "5")]
        [InlineData(LogLevel.Warning, "4")]
        [InlineData(LogLevel.Error, "3")]
        [InlineData(LogLevel.Critical, "3")]
        public void BasicMethods_MapLevelsAndTicoTags(LogLevel level, string expectedTag)
        {
            var innerLogger = new RecordingLogger<TestCategory>();
            var logger = new ExtendedLogger<TestCategory>(innerLogger);

            WriteBasicLog(logger, level, "Member");

            RecordedLog entry = Assert.Single(innerLogger.Entries);
            Assert.Equal(level, entry.Level);
            Assert.StartsWith($"<{expectedTag}> ", entry.Message);
            Assert.Contains(" [TestCategory][Member] message", entry.Message);
        }

        [Fact]
        public void Info_UsesCallerMemberName()
        {
            var innerLogger = new RecordingLogger<TestCategory>();
            var logger = new ExtendedLogger<TestCategory>(innerLogger);

            logger.Info("message");

            RecordedLog entry = Assert.Single(innerLogger.Entries);
            Assert.Contains("[TestCategory][Info_UsesCallerMemberName] message", entry.Message);
        }

        [Theory]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        public void ExceptionMethods_PreserveException(LogLevel level)
        {
            var innerLogger = new RecordingLogger<TestCategory>();
            var logger = new ExtendedLogger<TestCategory>(innerLogger);
            var exception = new InvalidOperationException("failure");

            if (level == LogLevel.Warning)
            {
                logger.Warn(exception, "message", "Member");
            }
            else
            {
                logger.Error(exception, "message", "Member");
            }

            RecordedLog entry = Assert.Single(innerLogger.Entries);
            Assert.Equal(level, entry.Level);
            Assert.Same(exception, entry.Exception);
            Assert.Contains("[TestCategory][Member] message", entry.Message);
        }

        [Theory]
        [InlineData(LogLevel.Trace, "7")]
        [InlineData(LogLevel.Debug, "6")]
        [InlineData(LogLevel.Information, "5")]
        [InlineData(LogLevel.Warning, "4")]
        [InlineData(LogLevel.Error, "3")]
        public void WithArgsMethods_FormatSelectedProperties(LogLevel level, string expectedTag)
        {
            var innerLogger = new RecordingLogger<TestCategory>();
            var logger = new ExtendedLogger<TestCategory>(innerLogger);

            WriteLogWithArgs(logger, level, "Member");

            RecordedLog entry = Assert.Single(innerLogger.Entries);
            Assert.Equal(level, entry.Level);
            Assert.StartsWith($"<{expectedTag}> ", entry.Message);
            Assert.Contains(
                "[TestCategory][Member] message | args: { Text = ab c, Count = 2 }",
                entry.Message);
        }

        [Fact]
        public void TraceMethodEntryAndExit_WriteOnlyWhenTraceIsEnabled()
        {
            var innerLogger = new RecordingLogger<TestCategory> { IsOutputEnabled = false };
            var logger = new ExtendedLogger<TestCategory>(innerLogger);

            logger.TraceMethodEntry(new { Value = 1 }, "EntryMember");
            logger.TraceMethodExit(new { Value = 2 }, "ExitMember");

            Assert.Empty(innerLogger.Entries);

            innerLogger.IsOutputEnabled = true;
            logger.TraceMethodEntry(new { Value = 1 }, "EntryMember");
            logger.TraceMethodExit(new { Value = 2 }, "ExitMember");

            Assert.Equal(2, innerLogger.Entries.Count);
            Assert.Contains("[TestCategory][EntryMember] Enter args: { Value = 1 }", innerLogger.Entries[0].Message);
            Assert.Contains("[TestCategory][ExitMember] Exit result: { Value = 2 }", innerLogger.Entries[1].Message);
        }

        [Fact]
        public void SanitizeForLogging_ReplacesLineBreaksAndRemovesPipes()
        {
            string result = ExtendedLogger<TestCategory>.SanitizeForLogging("a|b\r\nc\nd\re");

            Assert.Equal("ab c d e", result);
            Assert.Equal(string.Empty, ExtendedLogger<TestCategory>.SanitizeForLogging(null));
        }

        [Fact]
        public void FormatArgs_HandlesNullPrimitiveAndPropertyGetterFailure()
        {
            Assert.Equal("null", ExtendedLogger<TestCategory>.FormatArgs(null));
            Assert.Equal("ab c", ExtendedLogger<TestCategory>.FormatArgs("a|b\nc"));
            Assert.Equal(
                "{ Value = <error> }",
                ExtendedLogger<TestCategory>.FormatArgs(new ThrowingProperty()));
        }

        [Fact]
        public void Info_DoesNotAutomaticallySanitizeOrdinaryMessages()
        {
            var innerLogger = new RecordingLogger<TestCategory>();
            var logger = new ExtendedLogger<TestCategory>(innerLogger);

            logger.Info("a|b\nc", "Member");

            RecordedLog entry = Assert.Single(innerLogger.Entries);
            Assert.Contains("[TestCategory][Member] a|b\nc", entry.Message);
        }

        private static void WriteBasicLog(ExtendedLogger<TestCategory> logger, LogLevel level, string memberName)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    logger.Trace("message", memberName);
                    break;
                case LogLevel.Debug:
                    logger.Debug("message", memberName);
                    break;
                case LogLevel.Information:
                    logger.Info("message", memberName);
                    break;
                case LogLevel.Warning:
                    logger.Warn("message", memberName);
                    break;
                case LogLevel.Error:
                    logger.Error("message", memberName);
                    break;
                case LogLevel.Critical:
                    logger.Fatal("message", memberName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private static void WriteLogWithArgs(ExtendedLogger<TestCategory> logger, LogLevel level, string memberName)
        {
            var args = new { Text = "a|b\r\nc", Count = 2 };

            switch (level)
            {
                case LogLevel.Trace:
                    logger.TraceWithArgs("message", args, memberName);
                    break;
                case LogLevel.Debug:
                    logger.DebugWithArgs("message", args, memberName);
                    break;
                case LogLevel.Information:
                    logger.InfoWithArgs("message", args, memberName);
                    break;
                case LogLevel.Warning:
                    logger.WarnWithArgs("message", args, memberName);
                    break;
                case LogLevel.Error:
                    logger.ErrorWithArgs("message", args, memberName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private sealed class TestCategory
        {
        }

        private sealed class ThrowingProperty
        {
            public string Value => throw new InvalidOperationException();
        }
    }
}
