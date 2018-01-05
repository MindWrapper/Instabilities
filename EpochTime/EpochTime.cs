using System;
using NUnit.Framework;

namespace MyNamespace
{
    [TestFixture]
    public class EpochMess
    {
        [Test]
        public void GetUTCNowMs_ReturnsValueLessOrEqualCurrentTime()
        {
            var result = GetEpochUtcNowInMilliseconds();
            var now = (DateTime.UtcNow - s_Epoch).TotalMilliseconds;
            Assert.That(result, Is.LessThanOrEqualTo(now));
        }

        static DateTime s_Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

        private static long GetEpochUtcNowInMilliseconds()
        {
            return Convert.ToInt64((DateTime.UtcNow - s_Epoch).TotalMilliseconds);
        }
    }
}
