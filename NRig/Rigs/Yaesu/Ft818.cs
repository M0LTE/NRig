using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace NRig.Rigs.Yaesu
{
    public sealed class Ft818 : IRigController
    {
        public event EventHandler<FrequencyEventArgs> FrequencyChanged;

        private readonly ISerialPort serialPort;
        private TimeSpan rigPollInterval = TimeSpan.FromSeconds(1);
        private readonly static object rigCommunicationLock = new object();
        private Frequency freqHz;
        private Action<RigStatus> rigStatusCallback;

        internal Ft818(ISerialPort serialPort) => this.serialPort = serialPort;

        public Ft818(string comPort, int baudRate)
        {
            var serialPort = new SerialPort(comPort, baudRate, Parity.None, 8, StopBits.Two);
            serialPort.ReadTimeout = 1000;
            serialPort.Open();
            this.serialPort = new SerialPortWrapper(serialPort);

            Task.Factory.StartNew(PollRig, TaskCreationOptions.LongRunning);
        }

        public Task BeginRigStatusUpdates(Action<RigStatus> callback, TimeSpan updateFrequency)
        {
            rigStatusCallback = callback;
            rigPollInterval = updateFrequency;

            return Task.CompletedTask;
        }

        public Task EndRigStatusUpdates()
        {
            rigStatusCallback = null;
            return Task.CompletedTask;
        }

        private void PollRig()
        {
            while (true)
            {
                //TODO: all the other rig status calls

                (int hz, Mode mode) freqAndModeStatus;
                (bool squelched, bool ctcssOrDcsCodeIsUnmatched, bool discriminatorIsOffCentre, byte sMeter) rxStatus;

                try
                {
                    freqAndModeStatus = ReadFrequencyAndModeFromRig(serialPort);
                    rxStatus = ReadRxStatus(serialPort);
                }
                catch (TimeoutException)
                {
                    continue;
                }

                if (freqHz != freqAndModeStatus.hz)
                {
                    if (freqHz != 0)
                    {
                        FrequencyChanged?.Invoke(null, new FrequencyEventArgs { Frequency = freqAndModeStatus.hz, Vfo = Vfo.A });
                    }
                    freqHz = freqAndModeStatus.hz;
                }

                rigStatusCallback?.Invoke(new RigStatus
                {
                    VfoA = new VfoStatus { Frequency = freqAndModeStatus.hz, Mode = freqAndModeStatus.mode },
                    Squelched = rxStatus.squelched
                });

                Thread.Sleep(rigPollInterval);
            }
        }

        private static (bool squelched, bool ctcssOrDcsCodeIsUnmatched, bool discriminatorIsOffCentre, byte sMeter) ReadRxStatus(ISerialPort serialPort)
        {
            lock (rigCommunicationLock)
            {
                serialPort.Write(0, 0, 0, 0, 0xe7);

                var responseByte = new BitArray(new[] { serialPort.ReadByte() });

                var sMeterBits = new BitArray(new[] { responseByte[3], responseByte[2], responseByte[1], responseByte[0] });
                byte[] tmp = new byte[1];
                sMeterBits.CopyTo(tmp, 0);

                return (squelched: responseByte[7], 
                    ctcssOrDcsCodeIsUnmatched: responseByte[6], 
                    discriminatorIsOffCentre: responseByte[5], 
                    sMeter: tmp[0]);
            }
        }

        private static (int hz, Mode mode) ReadFrequencyAndModeFromRig(ISerialPort serialPort)
        {
            lock (rigCommunicationLock)
            {
                serialPort.Write(0, 0, 0, 0, 0x03);

                var rxbuf = new List<byte>
                {
                    (byte)serialPort.ReadByte(),
                    (byte)serialPort.ReadByte(),
                    (byte)serialPort.ReadByte(),
                    (byte)serialPort.ReadByte(),
                    (byte)serialPort.ReadByte(),
                };

                string first = GetFreqChars(rxbuf, 0);
                string second = GetFreqChars(rxbuf, 1);
                string third = GetFreqChars(rxbuf, 2);
                string fourth = GetFreqChars(rxbuf, 3);

                if (!int.TryParse($"{first}{second}{third}{fourth}", out int deciHertz))
                {
                    return (default, default);
                }

                return (deciHertz * 10, modeMap[rxbuf[4]]);
            }
        }

        private static readonly Dictionary<byte, Mode> modeMap = new Dictionary<byte, Mode> {
            { 0, Mode.Lsb },
            { 1, Mode.Usb },
            { 2, Mode.UsbCw },
            { 3, Mode.LsbCw },
            { 4, Mode.Am },
            { 6, Mode.BroadcastFm },
            { 8, Mode.Fm },
            { 0x0a, Mode.UsbData },
            { 0x0c, Mode.Packet },
        };

        private static string GetFreqChars(List<byte> rxbuf, int index)
        {
            string hex = rxbuf[index].ToString("X");
            if (hex.Length == 1)
            {
                hex = "0" + hex;
            }

            return hex;
        }

        public Task<Frequency> GetFrequency(Vfo vfo)
        {
            return Task.FromResult<Frequency>(freqHz);
        }

        public Task SetPttState(bool value)
        {
            lock (rigCommunicationLock)
            {
                byte command = value ? (byte)0x08 : (byte)0x88;
                serialPort.Write(0, 0, 0, 0, command);
            }

            return Task.CompletedTask;
        }

        public Task SetFrequency(Vfo vfo, Frequency hz)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(5))
            {
                if (hz == freqHz)
                    return Task.CompletedTask;

                // to set 439700000 send
                // 0x43 0x97 0x00 0x00 followed by opcode 0x01

                if (hz >= 1000000000)
                {
                    throw new ArgumentException("Frequency too high", nameof(hz));
                }

                string hertzStr = hz.ToString("000000000");

                byte[] digits = new byte[5];

                digits[0] = byte.Parse(hertzStr.Substring(0, 2), NumberStyles.HexNumber);
                digits[1] = byte.Parse(hertzStr.Substring(2, 2), NumberStyles.HexNumber);
                digits[2] = byte.Parse(hertzStr.Substring(4, 2), NumberStyles.HexNumber);
                digits[3] = byte.Parse(hertzStr.Substring(6, 2), NumberStyles.HexNumber);
                digits[4] = 0x01;

                lock (rigCommunicationLock)
                {
                    serialPort.Write(digits);
                    freqHz = hz;
                    try
                    {
                        serialPort.ReadByte();
                        return Task.CompletedTask;
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }

            throw new TimeoutException();
        }

        public static (string port, int baud) FindComPort()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string s in ports)
            {
                foreach (int baud in new[] { 4800, 9600, 38400 })
                {
                    using (var sp = new SerialPort(s, baud, Parity.None, 8, StopBits.Two))
                    using (var spw = new SerialPortWrapper(sp))
                    {
                        sp.ReadTimeout = 1000;

                        try
                        {
                            sp.Open();
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        try
                        {
                            var freq = ReadFrequencyAndModeFromRig(spw);

                            return (s, baud);
                        }
                        catch (TimeoutException)
                        {
                            continue;
                        }
                    }
                }
            }

            return (null, 0);
        }

        public Task SetCtcss(Frequency? frequency)
        {
            if (frequency != null && (frequency.Value < 67.0 || frequency.Value > 257.0))
            {
                throw new ArgumentOutOfRangeException();
            }

            //const byte dcsOn = 0x0a;
            //const byte encodeAndDecodeCtcssOn = 0x2a; // T+SQL
            const byte value_ctcssOn = 0x4a; // T
            const byte value_ctcssOff = 0x8a;
            const byte opcode_ctcssState = 0x0a;

            lock (rigCommunicationLock)
            {
                //byte cmd1_p1 = frequency == null ? value_ctcssOff : value_ctcssOn;

                serialPort.Write(value_ctcssOff, 0, 0, 0, opcode_ctcssState);
                serialPort.ReadByte(); // f0 if tone was on and turning on, 00 if tone was off and turning on...

                if (frequency != null)
                {
                    const byte ctcssFreqOpcode = 0x0b;

                    // for 88.5hz, p1 is 0x08, p2 is 0x85
                    byte cmd2_p1 = GetByteCtcss(0, frequency.Value);
                    byte cmd2_p2 = GetByteCtcss(2, frequency.Value);

                    serialPort.Write(cmd2_p1, cmd2_p2, 0, 0, ctcssFreqOpcode);

                    serialPort.ReadByte();

                    //serialPort.Write(value_ctcssOn, 0, 0, 0, opcode_ctcssState);
                    //serialPort.ReadByte();
                }
            }

            return Task.CompletedTask;
        }

        private byte GetByteCtcss(int startIndex, Frequency value) 
        { 
            string hertzStr = value.ToString("000.0").Replace(".", "");
            byte result = byte.Parse(hertzStr.Substring(startIndex, 2), NumberStyles.HexNumber);
            return result;
        }

        private byte GetByteRpt(int startIndex, Frequency value)
        {
            string hertzStr = value.ToString("000000000");
            string str = hertzStr.Substring(startIndex, 2);
            byte result = byte.Parse(str, NumberStyles.HexNumber);
            return result;
        }

        public Task SetRepeaterShift(Frequency? frequency)
        {
            const byte neg = 0x09;
            const byte pos = 0x49;
            const byte simplex = 0x89;
            const byte repeaterOffsetDirectionOpcode = 0x09;

            lock (rigCommunicationLock)
            {
                byte cmd1_p1;
                if (frequency == null || frequency == 0)
                {
                    cmd1_p1 = simplex;
                }
                else if (frequency > 0)
                {
                    cmd1_p1 = pos;
                }
                else
                {
                    cmd1_p1 = neg;
                }

                serialPort.Write(cmd1_p1, 0, 0, 0, repeaterOffsetDirectionOpcode);
                serialPort.ReadByte(); // ??

                if (frequency != null)
                {
                    const byte shiftFreqOpcode = 0xf9;

                    // for 17.654321 MHz - 0x17 0x65 0x43 0x21
                    byte cmd2_p1 = GetByteRpt(0, Math.Abs(frequency.Value));
                    byte cmd2_p2 = GetByteRpt(2, Math.Abs(frequency.Value));
                    byte cmd2_p3 = GetByteRpt(4, Math.Abs(frequency.Value));
                    byte cmd2_p4 = GetByteRpt(6, Math.Abs(frequency.Value));

                    serialPort.Write(cmd2_p1, cmd2_p2, cmd2_p3, cmd2_p4, shiftFreqOpcode);
                }
            }

            return Task.CompletedTask;
        }

        public Task<(bool enabled, Frequency tone)> GetCtcssState() => throw new NotImplementedException();
        public Task<(bool enabled, Frequency offset)> GetRepeaterShiftState() => throw new NotImplementedException();
        public Task SetActiveVfo(Vfo bfo) => throw new NotImplementedException();
        public Task<bool> GetPttState() => throw new NotImplementedException();
        public Task SetMode(Vfo vfo, Mode mode) => throw new NotImplementedException();
        public Task SetTunerState(bool value) => throw new NotImplementedException();
        public Task<bool> GetTunerState() => throw new NotImplementedException();
        public Task RunTuningCycle() => throw new NotImplementedException();
        public Task<MeterReadings> ReadMeters() => throw new NotImplementedException();
        public Task SetAgcState(AgcMode agcMode) => throw new NotImplementedException();
        public Task<AgcMode> GetAgcState() => throw new NotImplementedException();
        public Task SetNoiseBlankerState(bool value) => throw new NotImplementedException();
        public Task<bool> GetNoiseBlankerState() => throw new NotImplementedException();
        public Task BeginTransmitTuningCarrier(TimeSpan maxDuration) => throw new NotImplementedException();
        public Task EndTransmitTuningCarrier() => throw new NotImplementedException();
        public Task SetAttenuatorState(bool value) => throw new NotImplementedException();
        public Task<bool> GetAttenuatorState() => throw new NotImplementedException();
        public Task SetPreampState(bool value) => throw new NotImplementedException();
        public Task<bool> GetPreampState() => throw new NotImplementedException();
        public Task SetClarifierOffset(Frequency frequency) => throw new NotImplementedException();
        public Task<Frequency> GetClarifierOffset() => throw new NotImplementedException();

        public void Dispose() => Dispose(true);
        private bool disposedValue;
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                disposedValue = true;
            }
        }
    }
}
