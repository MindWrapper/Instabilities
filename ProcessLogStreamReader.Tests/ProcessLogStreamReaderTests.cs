using System.IO;
using NUnit.Framework;

namespace Instabilities.Utils.Tests
{
    [TestFixture]
    public class ProcessLogStreamReaderTests
    {
        MemoryStream m_Stream;
        ProcessLogStreamReader m_Reader;
        bool m_CanExpectMoreData;

        [SetUp]
        public void SetUp()
        {
            m_CanExpectMoreData = true;
            m_Stream = new MemoryStream();
            m_Reader = new ProcessLogStreamReader(CanExpectData, m_Stream);
        }

        [Test]
        public void ReadLine_BufferIsEmpty_ReturnsNull()
        {
            Assert.That(m_Reader.ReadLine(), Is.Null);
        }

        [Test]
        public void ReadLine_IncompleteString_ReturnsNull()
        {
            m_Stream.WriteChars('A');
            m_Stream.Position = 0;

            Assert.That(m_Reader.ReadLine(), Is.Null);
        }

        [Test]
        public void ReadLine_CompleteString_ReturnsExpectedString()
        {
            m_Stream.WriteChars('A', '\n');
            m_Stream.Position = 0;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
        }

        [Test]
        public void ReadLine_OneCompleteStringAndOneIncomplete_ReturnsNullForIncompleteString()
        {
            m_Stream.WriteChars('A', '\n', 'B');
            m_Stream.Position = 0;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
            Assert.That(m_Reader.ReadLine(), Is.Null);
        }

        [Test]
        public void ReadLine_StringCompletedAfterReadLineIsInvoked_ReturnsCompleteStringOnFollowingReadline()
        {
            m_Stream.WriteChars('A');
            m_Stream.Position = 0;

            Assert.That(m_Reader.ReadLine(), Is.Null);

            m_Stream.WriteChars('\n');
            m_Stream.Position = 1;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
        }

        [Test]
        public void ReadLine_WritingProcessShutdown_ReturnsAccumulatedString()
        {
            m_Stream.WriteChars('A');
            m_Stream.Position = 0;
            m_CanExpectMoreData = false;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
        }

        [Test]
        public void ReadLine_WriteLineWitnWindowsLineEnd_ReturnsAccumulatedString()
        {
            m_Stream.WriteChars('A', '\r', '\n', '\r', '\n');
            m_Stream.Position = 0;
            m_CanExpectMoreData = false;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
            Assert.That(m_Reader.ReadLine(), Is.Empty);
        }

        [Test]
        public void ReadLine_SeveralIncompleteStringInBetween_ReturnsAccumulatedString()
        {
            m_Stream.WriteChars('A', '\n', '\n', '\n', 'B');
            m_Stream.Position = 0;
            m_CanExpectMoreData = false;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
            Assert.That(m_Reader.ReadLine(), Is.Empty);
            Assert.That(m_Reader.ReadLine(), Is.Empty);
            Assert.That(m_Reader.ReadLine(), Is.EqualTo("B"));
        }

        [Test]
        public void ReadLine_TwoCompleteLines_ReturnsAccumulatedStrings()
        {
            m_Stream.WriteChars('A', '\n', 'B', '\n');
            m_Stream.Position = 0;

            Assert.That(m_Reader.ReadLine(), Is.EqualTo("A"));
            Assert.That(m_Reader.ReadLine(), Is.EqualTo("B"));
        }

        private bool CanExpectData()
        {
            return m_CanExpectMoreData;
        }

        [TearDown]
        public void TearDown()
        {
            m_Reader.Dispose();
            m_Stream.Dispose();
        }
    }

    static class MemoryStreamExt
    {
        internal static void WriteChars(this MemoryStream stream, params char[] chars)
        {
            foreach (var ch in chars)
            {
                stream.WriteByte((byte)ch);
            }
        }
    }
}
