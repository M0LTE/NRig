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
        private long freqHz;

        public Ts2000(string comPort, int baudRate, TimeSpan rigPollInterval)
        {
            this.rigPollInterval = rigPollInterval;

            serialPort = new SerialPort(comPort, baudRate);
            serialPort.ReadTimeout = 500;
            serialPort.Open();

            Task.Factory.StartNew(PollRig, TaskCreationOptions.LongRunning);
        }

        public Task<Frequency> GetFrequency(Vfo vfo) => Task.FromResult<Frequency>(freqHz);

        private static readonly Dictionary<Vfo, string> vfoMap = new Dictionary<Vfo, string> {
            { Vfo.A, "A" },
            { Vfo.B, "B" },
        };

        public Task<bool> SetFrequency(Vfo vfo, Frequency frequency)
        {
            lock (lockObj)
            {
                serialPort.Write($"F{vfoMap[vfo]}{frequency:D11};");
                freqHz = frequency;
                while (true)
                {
                    if (ReadFrequencyFromRig() == frequency)
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
                long hz = ReadFrequencyFromRig();

                if (hz == 0)
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

        private long ReadFrequencyFromRig()
        {
            string response;

            lock (lockObj)
            {
                while (true)
                {
                    serialPort.Write("FA;");

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

            if (!response.StartsWith("FA") || response.Length != 14 || !response.EndsWith(";"))
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

        public Task<bool> BeginTransmitTuningCarrier(TimeSpan maxDuration) => throw new NotImplementedException();
        public Task<bool> EndTransmitTuningCarrier() => throw new NotImplementedException();
        public Task<AgcMode> GetAgcState() => throw new NotImplementedException();
        public Task<bool> GetAttenuatorState() => throw new NotImplementedException();
        public Task<Frequency> GetClarifierOffset() => throw new NotImplementedException();
        public Task<bool> GetNoiseBlankerState() => throw new NotImplementedException();
        public Task<bool> GetPreampState() => throw new NotImplementedException();
        public Task<bool> GetPttState() => throw new NotImplementedException();
        public Task<bool> GetTunerState() => throw new NotImplementedException();
        public Task<MeterReadings> ReadMeters() => throw new NotImplementedException();
        public Task<bool> RunTuningCycle() => throw new NotImplementedException();
        public Task<bool> SetActiveVfo(Vfo bfo) => throw new NotImplementedException();
        public Task<bool> SetAgcState(AgcMode agcMode) => throw new NotImplementedException();
        public Task<bool> SetAttenuatorState(bool value) => throw new NotImplementedException();
        public Task<bool> SetClarifierOffset(Frequency frequency) => throw new NotImplementedException();
        public Task<bool> SetMode(Vfo vfo, Mode mode) => throw new NotImplementedException();
        public Task<bool> SetNoiseBlankerState(bool value) => throw new NotImplementedException();
        public Task<bool> SetPreampState(bool value) => throw new NotImplementedException();
        public Task<bool> SetPttState(bool value) => throw new NotImplementedException();
        public Task<bool> SetTunerState(bool value) => throw new NotImplementedException();

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