using System;
using System.Threading.Tasks;

namespace NRig
{
    public interface IRigController : IDisposable
    {
        Task<Frequency> GetFrequency(Vfo vfo);
        Task<bool> SetFrequency(Vfo vfo, Frequency frequency);
        Task<bool> SetActiveVfo(Vfo bfo);
        Task<bool> SetPttState(bool value);
        Task<bool> GetPttState();
        Task<bool> SetMode(Vfo vfo, Mode mode);
        Task<bool> SetTunerState(bool value);
        Task<bool> GetTunerState();
        Task<bool> RunTuningCycle();
        Task<MeterReadings> ReadMeters();
        Task<bool> SetAgcState(AgcMode agcMode);
        Task<AgcMode> GetAgcState();
        Task<bool> SetNoiseBlankerState(bool value);
        Task<bool> GetNoiseBlankerState();
        Task<bool> BeginTransmitTuningCarrier(TimeSpan maxDuration);
        Task<bool> EndTransmitTuningCarrier();
        Task<bool> SetAttenuatorState(bool value);
        Task<bool> GetAttenuatorState();
        Task<bool> SetPreampState(bool value);
        Task<bool> GetPreampState();
        Task<bool> SetClarifierOffset(Frequency frequency);
        Task<Frequency> GetClarifierOffset();
    }

    public struct Frequency
    {
        private long ValueHz { get; set; }

        public static implicit operator long(Frequency frequency) => frequency.ValueHz;
        public static implicit operator Frequency(long hz) => Hz(hz);
        public override bool Equals(object obj) => ((Frequency)obj).ValueHz == ValueHz;
        public override string ToString() => ValueHz.ToString();
        public override int GetHashCode() => ValueHz.GetHashCode();
        public static bool operator ==(Frequency f1, Frequency f2) => f1.Equals(f2);
        public static bool operator !=(Frequency f1, Frequency f2) => !f1.Equals(f2);
        public static Frequency Hz(long hz) => new Frequency { ValueHz = hz };
        public static Frequency KHz(long kHz) => new Frequency { ValueHz = kHz * 1000 };
        public static Frequency MHz(long mHz) => new Frequency { ValueHz = mHz * 1000000 };
        public static Frequency GHz(long gHz) => new Frequency { ValueHz = gHz * 1000000000 };
        public static Frequency THz(long tHz) => new Frequency { ValueHz = tHz * 1000000000000 };
    }

    public enum Vfo
    {
        A, B
    }

    public enum AgcMode
    {
        Off, AutoFast, Fast, Slow
    }

    public enum Mode
    {
        Usb, UsbData, Lsb, Fm, UsbCw, Am
    }

    public class MeterReadings
    {
        public double Power { get; set; }
        public double Swr { get; set; }
        public double Alc { get; set; }
    }
}