using HamLibSharp;
using System;
using System.Threading.Tasks;

namespace NRig.Rigs.Hamlib
{
    public sealed class HamLibWrapper : IRigController
    {
        private readonly HamLibWrapperInternal hamLibWrapper;

        public HamLibWrapper(string rigName, string port)
        {
            hamLibWrapper = new HamLibWrapperInternal(rigName, port);
        }

        public event EventHandler<FrequencyEventArgs> FrequencyChanged
        {
            add
            {
                ((IRigController)hamLibWrapper).FrequencyChanged += value;
            }

            remove
            {
                ((IRigController)hamLibWrapper).FrequencyChanged -= value;
            }
        }

        private static TOut ExecuteWithExceptionHandling<TOut>(Func<TOut> func)
        {
            try
            {
                return func();
            }
            catch (RigException ex)
            {
                throw new NRigException(ex);
            }
        }

        private static TOut ExecuteWithExceptionHandling<TIn1, TOut>(Func<TIn1, TOut> func, TIn1 arg1)
        {
            try
            {
                return func(arg1);
            }
            catch (RigException ex)
            {
                throw new NRigException(ex);
            }
        }

        private static TOut ExecuteWithExceptionHandling<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> func, TIn1 arg1, TIn2 arg2)
        {
            try
            {
                return func(arg1, arg2);
            }
            catch (RigException ex)
            {
                throw new NRigException(ex);
            }
        }

        public Task BeginRigStatusUpdates(Action<RigStatus> callback, TimeSpan updateFrequency) => ExecuteWithExceptionHandling((a, b) => hamLibWrapper.BeginRigStatusUpdates(a, b), callback, updateFrequency);
        public Task BeginTransmitTuningCarrier(TimeSpan maxDuration) => ExecuteWithExceptionHandling(a => hamLibWrapper.BeginTransmitTuningCarrier(a), maxDuration);
        public Task SetActiveVfo(Vfo bfo) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetActiveVfo(a), bfo);
        public Task SetClarifierOffset(Frequency frequency) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetClarifierOffset(a), frequency);
        public Task SetCtcss(Frequency? frequency) => ExecuteWithExceptionHandling((a) => hamLibWrapper.SetCtcss(a), frequency);

        public void Dispose()
        {
            hamLibWrapper.Dispose();
        }

        public Task EndRigStatusUpdates()
        {
            return hamLibWrapper.EndRigStatusUpdates();
        }

        public Task EndTransmitTuningCarrier()
        {
            return hamLibWrapper.EndTransmitTuningCarrier();
        }

        public Task<AgcMode> GetAgcState()
        {
            return hamLibWrapper.GetAgcState();
        }

        public Task<bool> GetAttenuatorState()
        {
            return hamLibWrapper.GetAttenuatorState();
        }

        public Task<Frequency> GetClarifierOffset()
        {
            return hamLibWrapper.GetClarifierOffset();
        }

        public Task<(bool enabled, Frequency tone)> GetCtcssState()
        {
            return hamLibWrapper.GetCtcssState();
        }

        public Task<Frequency> GetFrequency(Vfo vfo)
        {
            return hamLibWrapper.GetFrequency(vfo);
        }

        public Task<bool> GetNoiseBlankerState()
        {
            return hamLibWrapper.GetNoiseBlankerState();
        }

        public Task<bool> GetPreampState()
        {
            return hamLibWrapper.GetPreampState();
        }

        public Task<bool> GetPttState()
        {
            return hamLibWrapper.GetPttState();
        }

        public Task<(bool enabled, Frequency offset)> GetRepeaterShiftState()
        {
            return hamLibWrapper.GetRepeaterShiftState();
        }

        public Task<bool> GetTunerState()
        {
            return hamLibWrapper.GetTunerState();
        }

        public Task<MeterReadings> ReadMeters()
        {
            return hamLibWrapper.ReadMeters();
        }

        public Task RunTuningCycle() => ExecuteWithExceptionHandling(() => hamLibWrapper.RunTuningCycle());

        public Task SetAgcState(AgcMode agcMode) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetAgcState(a), agcMode);

        public Task SetAttenuatorState(bool value) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetAttenuatorState(a), value);

        public Task SetFrequency(Vfo vfo, Frequency frequency) => ExecuteWithExceptionHandling((a, b) => hamLibWrapper.SetFrequency(a, b), vfo, frequency);

        public Task SetMode(Vfo vfo, Mode mode) => ExecuteWithExceptionHandling((a, b) => hamLibWrapper.SetMode(a, b), vfo, mode);

        public Task SetNoiseBlankerState(bool value) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetNoiseBlankerState(a), value);

        public Task SetPreampState(bool value) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetPreampState(a), value);

        public Task SetPttState(bool value) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetPttState(a), value);

        public Task SetRepeaterShift(Frequency? frequency) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetRepeaterShift(a), frequency);

        public Task SetTunerState(bool value) => ExecuteWithExceptionHandling(a => hamLibWrapper.SetTunerState(a), value);
    }
}