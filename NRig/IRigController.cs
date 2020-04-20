using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NRig
{
    public interface IRigController : IDisposable
    {
        event EventHandler<FrequencyEventArgs> FrequencyChanged;
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
        Task BeginRigStatusUpdates(Action<RigStatus> callback, TimeSpan updateFrequency);
        Task EndRigStatusUpdates();
    }

    public class RigStatus
    {
        public VfoStatus VfoA { get; set; }
        public VfoStatus VfoB { get; set; }
        public Vfo ActiveVfo { get; set; }
        public bool Ptt { get; set; }
        public bool Tuner { get; set; }
        public MeterReadings MeterReadings { get; set; }
        public AgcMode Agc { get; set; }
        public bool NoiseBlanker { get; set; }
        public bool Attenuator { get; set; }
        public bool Preamp { get; set; }
        public Frequency ClarifierOffset { get; set; }
    }

    public class VfoStatus
    {
        public Frequency Frequency { get; set; }
        public Mode Mode { get; set; }
    }

    public class FrequencyEventArgs : EventArgs
    {
        public Vfo Vfo { get; set; }
        public Frequency Frequency { get; set; }
    }
}