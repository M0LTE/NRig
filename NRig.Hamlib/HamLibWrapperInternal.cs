using HamLibSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NRig.Rigs.Hamlib
{
    internal class HamLibWrapperInternal : IRigController
    {
        private readonly IHamlibRig rig;

        public HamLibWrapperInternal(string rigName, string port)
        {
            rig = new HamlibRigFacade(rigName);
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
            rig.SetPtt(value ? PttMode.On : PttMode.Off, RigVfo.Current);
            return Task.CompletedTask;
        }

        public Task<bool> GetPttState() => throw new NotImplementedException();

        public Task SetMode(Vfo vfo, Mode mode)
        {
            var rm = GetHamlibMode(mode);
            rig.SetMode(rm, rig.PassbandNormal(rm), GetVfo(vfo));
            return Task.CompletedTask;
        }

        private static RigMode GetHamlibMode(Mode nrigMode)
        {
            switch (nrigMode)
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

            throw new NotImplementedException($"NRig mode {nrigMode} not mapped to HamLib mode");
        }

        private static Mode GetNRigMode(RigMode hamlibMode)
        {
            switch (hamlibMode)
            {
                case RigMode.AM: return Mode.Am;
                case RigMode.WFM: return Mode.BroadcastFm;
                case RigMode.FM: return Mode.Fm;
                case RigMode.LSB: return Mode.Lsb;
                case RigMode.CWR: return Mode.LsbCw;
                case RigMode.PacketFM: return Mode.Packet;
                case RigMode.USB: return Mode.Usb;
                case RigMode.CW: return Mode.UsbCw;
                case RigMode.PacketUSB: return Mode.UsbData;
            }

            throw new NotImplementedException($"Hamlib mode {hamlibMode} not mapped to NRig mode");
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

        Timer timer;

        public Task BeginRigStatusUpdates(Action<RigStatus> callback, TimeSpan updateFrequency)
        {
            timer = new Timer(Tick, callback, TimeSpan.Zero, updateFrequency);

            return Task.CompletedTask;
        }

        private void Tick(object state)
        {
            Action<RigStatus> callback = (Action<RigStatus>)state;

            long width = default;
            var rigStatus = new RigStatus
            {
                VfoA = new VfoStatus
                {
                    Frequency = Frequency.Hz(rig.GetFrequency(RigVfo.Current)),
                    Mode = GetNRigMode(rig.GetMode(ref width, RigVfo.Current))
                },
                Ptt = rig.GetPtt(RigVfo.Current) != PttMode.Off,
            };

            callback(rigStatus);
        }

        public Task EndRigStatusUpdates()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            
            return Task.CompletedTask;
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