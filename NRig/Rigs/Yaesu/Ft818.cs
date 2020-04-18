using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NRig.Rigs.Yaesu
{
    public sealed class Ft818 : IRigController
    {
        public Task<bool> BeginTransmitTuningCarrier(TimeSpan maxDuration)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EndTransmitTuningCarrier()
        {
            throw new NotImplementedException();
        }

        public Task<AgcMode> GetAgcState()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetAttenuatorState()
        {
            throw new NotImplementedException();
        }

        public Task<Frequency> GetClarifierOffset()
        {
            throw new NotImplementedException();
        }

        public Task<Frequency> GetFrequency(Vfo vfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetNoiseBlankerState()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPreampState()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPttState()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTunerState()
        {
            throw new NotImplementedException();
        }

        public Task<MeterReadings> ReadMeters()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RunTuningCycle()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetActiveVfo(Vfo bfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetAgcState(AgcMode agcMode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetAttenuatorState(bool value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetClarifierOffset(Frequency frequency)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetFrequency(Vfo vfo, Frequency frequency)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetMode(Vfo vfo, Mode mode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetNoiseBlankerState(bool value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetPreampState(bool value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetPttState(bool value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetTunerState(bool value)
        {
            throw new NotImplementedException();
        }

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
