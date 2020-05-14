using HamLibSharp;
using System;
using System.Threading.Tasks;

namespace NRig.Rigs.Hamlib
{
    internal class HamLibWrapperInternal : IRigController
    {
        private readonly Rig rig;

        public HamLibWrapperInternal(string rigName, string port)
        {
            rig = new Rig(rigName);
            rig.Open(port);
        }

        public Task<Frequency> GetFrequency(Vfo vfo) => throw new NotImplementedException();
        public Task SetFrequency(Vfo vfo, Frequency frequency)
        {
            rig.SetFrequency(frequency, GetVfo(vfo));
            return Task.CompletedTask;
        }

        private int GetVfo(Vfo vfo)
        {
            switch (vfo)
            {
                case Vfo.A: return RigVfo.VfoA;
                case Vfo.B: return RigVfo.VfoB;
            }

            throw new NotImplementedException();
        }

        public Task SetActiveVfo(Vfo vfo)
        {
            rig.SetVFO(GetVfo(vfo));
            return Task.CompletedTask;
        }

        public Task SetPttState(bool value)
        {
            rig.SetPtt(PttMode.OnData, RigVfo.Current);
            return Task.CompletedTask;
        }

        public Task<bool> GetPttState() => throw new NotImplementedException();

        public Task SetMode(Vfo vfo, Mode mode)
        {
            var rm = GetRigMode(mode);
            rig.SetMode(rm, rig.PassbandNormal(rm), GetVfo(vfo));
            return Task.CompletedTask;
        }

        private RigMode GetRigMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Am: return RigMode.AM;
                case Mode.BroadcastFm: return RigMode.WFM;
                case Mode.Fm: return RigMode.FM;
                case Mode.Lsb: return RigMode.LSB;
                case Mode.LsbCw: return RigMode.CWR;
                case Mode.Packet: return RigMode.PacketFM;
                case Mode.Usb: return RigMode.USB;
                case Mode.UsbCw: return RigMode.CW;
                case Mode.UsbData: return RigMode.PacketUSB;
            }

            throw new NotImplementedException($"NRig mode {mode} not mapped to HamLib mode");
        }

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

        public event EventHandler<FrequencyEventArgs> FrequencyChanged;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    rig?.Dispose();
                }
                disposedValue = true;
            }
        }

        public Task BeginRigStatusUpdates(Action<RigStatus> callback, TimeSpan updateFrequency)
        {
            throw new NotImplementedException();
        }

        public Task EndRigStatusUpdates()
        {
            throw new NotImplementedException();
        }

        public Task SetCtcss(Frequency? frequency)
        {
            if (frequency == null)
            {
                rig.SetCTCSS(0, RigVfo.Current);
            }
            else
            {
                uint dHz = (uint)(10 * (decimal)frequency.Value);

                rig.SetCTCSS(dHz, RigVfo.Current);
            }

            return Task.CompletedTask;
        }

        public Task<(bool enabled, Frequency tone)> GetCtcssState()
        {
            throw new NotImplementedException();
        }

        public Task<(bool enabled, Frequency offset)> GetRepeaterShiftState()
        {
            throw new NotImplementedException();
        }

        public Task SetRepeaterShift(Frequency? frequency)
        {
            if (frequency == null)
            {
                rig.SetRepeaterShift(RepeaterShift.None, RigVfo.Current);
            }
            else
            {
                if (frequency > 0)
                {
                    rig.SetRepeaterShift(RepeaterShift.Plus, RigVfo.Current);
                }
                else
                {
                    rig.SetRepeaterShift(RepeaterShift.Minus, RigVfo.Current);
                }

                rig.SetRepeaterOffset(Math.Abs((int)frequency.Value), RigVfo.Current);
            }

            return Task.CompletedTask;
        }
    }
}