using FakeItEasy;
using FluentAssertions;
using NRig.Rigs.Yaesu;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NRig.Tests
{
    public class Ft818Tests
    {
        private readonly ISerialPort fakeSerialPort = A.Fake<ISerialPort>();
        private readonly IRigController rig;

        public Ft818Tests() => rig = new Ft818(fakeSerialPort);

        [Fact]
        public async Task SetCtcss_on_2digit()
        {
            await rig.SetCtcss(80);

            A.CallTo(() => fakeSerialPort.Write(0x4a, 0, 0, 0, 0x0a)).MustHaveHappened();
            A.CallTo(() => fakeSerialPort.Write(0x08, 0x00, 0, 0, 0x0b)).MustHaveHappened();
        }

        [Fact]
        public async Task SetCtcss_on_3digit()
        {
            await rig.SetCtcss(88.5);

            A.CallTo(() => fakeSerialPort.Write(0x4a, 0, 0, 0, 0x0a)).MustHaveHappened();
            A.CallTo(() => fakeSerialPort.Write(0x08, 0x85, 0, 0, 0x0b)).MustHaveHappened();

        }

        [Fact]
        public async Task SetCtcss_on_4digit()
        {
            await rig.SetCtcss(188.5);

            A.CallTo(() => fakeSerialPort.Write(0x4a, 0, 0, 0, 0x0a)).MustHaveHappened();
            A.CallTo(() => fakeSerialPort.Write(0x18, 0x85, 0, 0, 0x0b)).MustHaveHappened();
        }

        [Fact]
        public void SetCtcss_on_toohigh()
        {
            Action action = () => rig.SetCtcss(257.1);

            action.Should().Throw<ArgumentOutOfRangeException>();

            A.CallTo(() => fakeSerialPort.Write(A<byte[]>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void SetCtcss_on_toolow()
        {
            Action action = () => rig.SetCtcss(66.9);

            action.Should().Throw<ArgumentOutOfRangeException>();

            A.CallTo(() => fakeSerialPort.Write(A<byte[]>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SetCtcss_off()
        {
            await rig.SetCtcss(null);

            A.CallTo(() => fakeSerialPort.Write(0x8a, 0, 0, 0, 0x0a)).MustHaveHappened();
        }

        [Fact]
        public async Task SetRepeaterShift_pos()
        {
            await rig.SetRepeaterShift(Frequency.MHz(17.654321));

            A.CallTo(() => fakeSerialPort.Write(0x01, 0x76, 0x54, 0x32, 0xf9)).MustHaveHappened();
            A.CallTo(() => fakeSerialPort.Write(0x49, 0, 0, 0, 0x09)).MustHaveHappened();
        }

        [Fact]
        public async Task SetRepeaterShift_pos2()
        {
            await rig.SetRepeaterShift(Frequency.MHz(0.6));

            A.CallTo(() => fakeSerialPort.Write(0x00, 0x06, 0x00, 0x00, 0xf9)).MustHaveHappened();
            A.CallTo(() => fakeSerialPort.Write(0x49, 0, 0, 0, 0x09)).MustHaveHappened();
        }

        [Fact]
        public async Task SetRepeaterShift_neg()
        {
            await rig.SetRepeaterShift(Frequency.MHz(-0.6));

            A.CallTo(() => fakeSerialPort.Write(0x00, 0x06, 0x00, 0x00, 0xf9)).MustHaveHappened();
            A.CallTo(() => fakeSerialPort.Write(0x09, 0, 0, 0, 0x09)).MustHaveHappened();
        }

        [Fact]
        public async Task SetRepeaterShift_off_zero()
        {
            await rig.SetRepeaterShift(0);

            A.CallTo(() => fakeSerialPort.Write(0x89, 0, 0, 0, 0x09)).MustHaveHappened();
        }

        [Fact]
        public async Task SetRepeaterShift_off_null()
        {
            await rig.SetRepeaterShift(null);

            A.CallTo(() => fakeSerialPort.Write(0x89, 0, 0, 0, 0x09)).MustHaveHappened();
        }

        [Fact]
        public async Task SetFrequency()
        {
            await rig.SetFrequency(Vfo.A, Frequency.MHz(433.012345));

            A.CallTo(() => fakeSerialPort.Write(0x43, 0x30, 0x12, 0x34, 0x01)).MustHaveHappened();
        }
    }
}
