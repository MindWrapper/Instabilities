using System;
using System.IO;
using System.Text;

namespace Instabilities.Utils
{
     // When reading & writing data to the same file simultaneously, StreamReader.ReadLine()
    // might return an incomplete string. One of the reasons why it happens is because writing
    // process might decide to flush a  buffer, which might have contained an incomplete string.
    // Reader should somehow know if more data expected to come from the stream
    // Usage example:
    // var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    // var streamReader = new ProcessLogStreamReader(_isWritingProcessStillAlive, fileStream);
    // 
    // m_Reader = new ProcessLogStreamReader(!process.Existed, m_Stream);
    // This class will return only complete strings, if that can be expected
    // Bases on TextReader.ReadLine() implementation 
    // Might be not have a very good performance, but does the job
    public class ProcessLogStreamReader : IDisposable
    {
        readonly Func<bool> m_CanExpectMoreData;
        StreamReader m_Reader;
        StringBuilder m_Sb = new StringBuilder();

        public ProcessLogStreamReader(Func<bool> canExpectMoreData, Stream stream)
        {
            m_CanExpectMoreData = canExpectMoreData;
            m_Reader = new StreamReader(stream, Encoding.UTF8);
        }

        public int Peek()
        {
            return m_Reader.Peek();
        }

        public virtual string ReadLine()
        {
            while (true)
            {
                var ch = m_Reader.Read();
                if (ch == -1)
                {
                    if (m_CanExpectMoreData())
                    {
                        return null;
                    }
                    break;
                }

                if (ch == '\r' || ch == '\n')
                {
                    if (ch == '\r' && Peek() == '\n')
                        m_Reader.Read();
                    var result = m_Sb.ToString();
                    m_Sb.Length = 0;
                    return result;
                }
                m_Sb.Append((char)ch);
            }

            if (m_Sb.Length > 0)
            {
                var result = m_Sb.ToString();
                m_Sb.Length = 0;
                return result;
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (m_Reader == null)
                return;
            m_Reader.Dispose();
            m_Reader = null;
        }
    }
}
