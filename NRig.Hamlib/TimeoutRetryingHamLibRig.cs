using HamLibSharp;
using System;
using System.Reflection;

namespace NRig.Rigs.Hamlib
{
    internal class TimeoutRetryingHamLibRig : IHamlibRig
    {
        private readonly IHamlibRig innerRig;

        public TimeoutRetryingHamLibRig(IHamlibRig innerRig)
        {
            this.innerRig = innerRig;
        }

        public void Dispose()
        {
            innerRig.Dispose();
        }

        public void Open(string port, BaudRate baudRate, Handshake handshake, int dataBits, int stopBits) => innerRig.Open(port, baudRate, handshake, dataBits, stopBits);
        public int PassbandNormal(RigMode mode) => innerRig.PassbandNormal(mode);

        public uint GetCtcss(int vfo) => RunWithRetry(innerRig.GetCtcss, vfo);
        public double GetFrequency(int vfo) => RunWithRetry(innerRig.GetFrequency, vfo);
        public (RigMode, long width) GetMode(int vfo) => RunWithRetry(innerRig.GetMode, vfo);
        public PttMode GetPtt(int vfo) => RunWithRetry(innerRig.GetPtt, vfo);
        public int GetRepeaterOffset(int vfo) => RunWithRetry(innerRig.GetRepeaterOffset, vfo);
        public RepeaterShift GetRepeaterShift(int vfo) => RunWithRetry(innerRig.GetRepeaterShift, vfo);
        public void SetCTCSS(uint tone, int vfo) => RunWithRetry(innerRig.SetCTCSS, tone, vfo);
        public void SetFrequency(double freq, int vfo) => RunWithRetry(innerRig.SetFrequency, freq, vfo);
        public void SetMode(RigMode mode, long width, int vfo) => RunWithRetry(innerRig.SetMode, mode, width, vfo);
        public void SetPtt(PttMode onData, int current) => RunWithRetry(innerRig.SetPtt, onData, current);
        public void SetRepeaterOffset(int rptr_offs, int vfo) => RunWithRetry(innerRig.SetRepeaterOffset, rptr_offs, vfo);
        public void SetRepeaterShift(RepeaterShift rptr_shift, int vfo) => RunWithRetry(innerRig.SetRepeaterShift, rptr_shift, vfo);
        public void SetVFO(int v) => RunWithRetry(innerRig.SetVFO, v);

        private static TOut RunWithRetry<TIn1, TOut>(Func<TIn1, TOut> func, TIn1 arg1) => RunDelegate<TOut>(func, arg1);
        private static void RunWithRetry<TIn1>(Action<TIn1> action, TIn1 arg1) => RunDelegate<object>(action, arg1);
        private static void RunWithRetry<TIn1, TIn2>(Action<TIn1, TIn2> action, TIn1 arg1, TIn2 arg2) => RunDelegate<object>(action, arg1, arg2);
        private static void RunWithRetry<TIn1, TIn2, TIn3>(Action<TIn1, TIn2, TIn3> action, TIn1 arg1, TIn2 arg2, TIn3 arg3) => RunDelegate<object>(action, arg1, arg2, arg3);

        private static TOut RunDelegate<TOut>(Delegate d, params object[] args)
        {
            while (true)
            {
                try
                {
                    var result = d.DynamicInvoke(args);

                    if (result != null)
                    {
                        return (TOut)result;
                    }

                    return default;
                }
                catch (TargetInvocationException ex) when (ex.InnerException is RigException rex && rex.Message == "Communication timed out")
                {
                    continue;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }
    }
}