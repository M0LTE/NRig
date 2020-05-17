using Xunit;

namespace NRig.Tests
{
    public class RigStatusTests
    {
        [Fact]
        public void TestEquals_1()
        {
            var rigStatus1 = new RigStatus { VfoA = new VfoStatus { Frequency = 123.45 } };
            var rigStatus2 = new RigStatus { VfoA = new VfoStatus { Frequency = 123.45 } };

            Assert.True(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_2()
        {
            var rigStatus1 = new RigStatus { VfoA = new VfoStatus { Frequency = 123.45 } };
            var rigStatus2 = new RigStatus { VfoA = new VfoStatus { Frequency = 123.46 } };

            Assert.False(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_3()
        {
            var rigStatus1 = new RigStatus { VfoA = null };
            var rigStatus2 = new RigStatus { VfoA = null };

            Assert.True(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_4()
        {
            var rigStatus1 = new RigStatus { VfoA = null };
            var rigStatus2 = new RigStatus { VfoA = new VfoStatus() };

            Assert.False(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_5()
        {
            var rigStatus1 = new RigStatus { VfoB = new VfoStatus { Frequency = 123.45 } };
            var rigStatus2 = new RigStatus { VfoB = new VfoStatus { Frequency = 123.45 } };

            Assert.True(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_6()
        {
            var rigStatus1 = new RigStatus { VfoB = new VfoStatus { Frequency = 123.45 } };
            var rigStatus2 = new RigStatus { VfoB = new VfoStatus { Frequency = 123.46 } };

            Assert.False(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_7()
        {
            var rigStatus1 = new RigStatus { VfoB = null };
            var rigStatus2 = new RigStatus { VfoB = null };

            Assert.True(rigStatus1.Equals(rigStatus2));
        }

        [Fact]
        public void TestEquals_8()
        {
            var rigStatus1 = new RigStatus { VfoB = null };
            var rigStatus2 = new RigStatus { VfoB= new VfoStatus() };

            Assert.False(rigStatus1.Equals(rigStatus2));
        }
    }
}
