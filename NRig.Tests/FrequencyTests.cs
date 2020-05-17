using Xunit;

namespace NRig.Tests
{
    public class FrequencyTests
    {
        [Theory]
        [InlineData(-100,      "-100")]
        [InlineData(999999999, "999.999.99")]
        [InlineData(130472000, "130.472.00")]
        [InlineData(130472300, "130.472.30")]
        [InlineData(130472306, "130.472.30")]
        [InlineData(30472306,  " 30.472.30")]
        [InlineData(3047230,   "  3.047.23")]
        [InlineData(909000,    "    909.00")]
        [InlineData(100000,    "    100.00")]
        [InlineData(99000,     "     99.00")]
        [InlineData(9900,      "      9.90")]
        [InlineData(900,       "        90")]
        [InlineData(90,        "         9")]
        [InlineData(9,         "         0")]
        public void TestYaesuStandard(double hz, string expected)
        {
            var freq = Frequency.Hz(hz);
            Assert.Equal(expected, freq.ToString(FrequencyStyle.Yaesu, 8));
        }
    }
}