using System;
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
        private readonly SerialPort serialPort;
        private readonly static byte[] freqRequestCommand = new byte[] { 0, 0, 0, 0, 0x03 };
        private readonly TimeSpan rigPollInterval;
        private readonly static object lockObj = new object();
        private long freqHz;

        public Ft818(string comPort, int baudRate, TimeSpan rigPollInterval)
        {
            this.rigPollInterval = rigPollInterval;
            serialPort = new SerialPort(comPort, baudRate, Parity.None, 8, StopBits.Two);
            serialPort.ReadTimeout = 1000;
            serialPort.Open();

            Task.Factory.StartNew(PollRig, TaskCreationOptions.LongRunning);
        }

        private void PollRig()
        {
            while (true)
            {
                int hz;

                try
                {
                    hz = ReadFrequencyFromRig(serialPort);
                }
                catch (TimeoutException)
                {
                    continue;
                }

                if (freqHz != hz)
                {
                    freqHz = hz;
                }

                Thread.Sleep(rigPollInterval);
            }
        }

        private static int ReadFrequencyFromRig(SerialPort serialPort)
        {
            lock (lockObj)
            {
                serialPort.Write(freqRequestCommand, 0, freqRequestCommand.Length);

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
                    return 0;
                }

                int hz = deciHertz * 10;

                return hz;
            }
        }

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

                string hertzStr = hz.ToString("D9");

                byte[] digits = new byte[5];

                digits[0] = byte.Parse(hertzStr.Substring(0, 2), NumberStyles.HexNumber);
                digits[1] = byte.Parse(hertzStr.Substring(2, 2), NumberStyles.HexNumber);
                digits[2] = byte.Parse(hertzStr.Substring(4, 2), NumberStyles.HexNumber);
                digits[3] = byte.Parse(hertzStr.Substring(6, 2), NumberStyles.HexNumber);
                digits[4] = 0x01;

                lock (lockObj)
                {
                    serialPort.Write(digits, 0, digits.Length);
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
                            var freq = ReadFrequencyFromRig(sp);

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

        public Task SetActiveVfo(Vfo bfo) => throw new NotImplementedException();
        public Task SetPttState(bool value) => throw new NotImplementedException();
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
