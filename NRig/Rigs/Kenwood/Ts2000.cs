using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NRig.Rigs.Kenwood
{
    public class Ts2000 : IRigController
    {
        private readonly SerialPort serialPort;
        private readonly TimeSpan rigPollInterval;
        private readonly object lockObj = new object();
        private long freqHzA;
        private long freqHzB;

        public Ts2000(string comPort, int baudRate, TimeSpan rigPollInterval)
        {
            this.rigPollInterval = rigPollInterval;

            serialPort = new SerialPort(comPort, baudRate);
            serialPort.ReadTimeout = 500;
            serialPort.Open();

            Task.Factory.StartNew(PollRig, TaskCreationOptions.LongRunning);
        }

        public Task<Frequency> GetFrequency(Vfo vfo)
        {
            switch (vfo)
            {
                case Vfo.A:
                    return Task.FromResult<Frequency>(freqHzA);
                case Vfo.B:
                    return Task.FromResult<Frequency>(freqHzB);
            }

            throw new NotSupportedException(vfo.ToString());
        }

        private static readonly Dictionary<Vfo, string> vfoMap = new Dictionary<Vfo, string> {
            { Vfo.A, "A" },
            { Vfo.B, "B" },
        };

        public Task SetFrequency(Vfo vfo, Frequency frequency)
        {
            lock (lockObj)
            {
                serialPort.Write($"F{vfoMap[vfo]}{frequency:D11};");

                if (vfo == Vfo.A)
                {
                    freqHzA = frequency;
                }
                else if (vfo == Vfo.B)
                {
                    freqHzB = frequency;
                }
                else throw new NotSupportedException(vfo.ToString());

                while (true)
                {
                    if (ReadFrequencyFromRig(vfo) == frequency)
                    {
                        return Task.FromResult(true);
                    }
                }
            }
        }

        private void PollRig()
        {
            while (true)
            {
                long hz1 = ReadFrequencyFromRig(Vfo.A);

                if (hz1 != 0)
                {
                    if (freqHzA != hz1)
                    {
                        freqHzA = hz1;
                    }
                }

                long hz2 = ReadFrequencyFromRig(Vfo.B);

                if (hz2 != 0)
                {
                    if (freqHzB != hz2)
                    {
                        freqHzB = hz2;
                    }
                }

                Thread.Sleep(rigPollInterval);
            }
        }

        private long ReadFrequencyFromRig(Vfo vfo)
        {
            string response;

            lock (lockObj)
            {
                while (true)
                {
                    serialPort.Write($"F{vfoMap[vfo]};");

                    try
                    {
                        response = ReadResponse();
                        break;
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }

            if (!response.StartsWith($"F{vfoMap[vfo]}") || response.Length != 14 || !response.EndsWith(";"))
            {
                return 0;
            }

            if (!long.TryParse(new String(response.Skip(2).Take(11).ToArray()), out long hz))
            {
                return 0;
            }

            return hz;
        }

        private string ReadResponse()
        {
            var chars = new List<char>();
            while (true)
            {
                int b = serialPort.ReadByte();
                chars.Add((char)b);
                if (b == ';')
                    break;
            }

            string response = new string(chars.ToArray());

            return response;
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
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serialPort.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}