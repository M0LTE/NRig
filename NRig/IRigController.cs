using System;
using System.Threading.Tasks;

namespace NRig
{
    public interface IRigController : IDisposable
    {
        Task<Frequency> GetFrequency(Vfo vfo);
        Task SetFrequency(Vfo vfo, Frequency frequency);
        Task SetActiveVfo(Vfo bfo);
        Task SetPttState(bool value);
        Task<bool> GetPttState();
        Task SetMode(Vfo vfo, Mode mode);
        Task SetTunerState(bool value);
        Task<bool> GetTunerState();
        Task RunTuningCycle();
        Task<MeterReadings> ReadMeters();
        Task SetAgcState(AgcMode agcMode);
        Task<AgcMode> GetAgcState();
        Task SetNoiseBlankerState(bool value);
        Task<bool> GetNoiseBlankerState();
        Task BeginTransmitTuningCarrier(TimeSpan maxDuration);
        Task EndTransmitTuningCarrier();
        Task SetAttenuatorState(bool value);
        Task<bool> GetAttenuatorState();
        Task SetPreampState(bool value);
        Task<bool> GetPreampState();
        Task SetClarifierOffset(Frequency frequency);
        Task<Frequency> GetClarifierOffset();
    }
}