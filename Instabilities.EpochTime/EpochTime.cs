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
            Assert.That(result, Is.LessThanOrEqualTo((DateTime.UtcNow - s_Epoch).TotalMilliseconds));
        }

        static DateTime s_Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

        private static long GetEpochUtcNowInMilliseconds()
        {
            return Convert.ToInt64((DateTime.UtcNow - s_Epoch).TotalMilliseconds);
        }
    }
}
