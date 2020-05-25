using HamLibSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NRig.Rigs.Hamlib
{
    internal class CapabilityFakingHamLibRig : IHamlibRig
    {
        private readonly ConcurrentDictionary<int, int> repeaterOffsets = new ConcurrentDictionary<int, int>();
        private readonly ConcurrentDictionary<int, RepeaterShift> repeaterShifts = new ConcurrentDictionary<int, RepeaterShift>();
        private readonly ConcurrentDictionary<int, uint> ctcssTones = new ConcurrentDictionary<int, uint>();
        private readonly List<string> unavailableFeatures = new List<string>();
        private readonly object lockObj = new object();
        private readonly IHamlibRig innerRig;

        public CapabilityFakingHamLibRig(IHamlibRig innerRig) => this.innerRig = innerRig;

        public void Dispose() => innerRig.Dispose();
        public void Open(string port) => innerRig.Open(port);

        public uint GetCtcss(int vfo) => RunWithFeatureCheck(innerRig.GetCtcss, GetCtcssFromDictionary, vfo);
        public double GetFrequency(int vfo) => RunWithFeatureCheck(innerRig.GetFrequency, null, vfo);
        public (RigMode, long width) GetMode(int vfo) => RunWithFeatureCheck(innerRig.GetMode, null, vfo);
        public PttMode GetPtt(int vfo) => RunWithFeatureCheck(innerRig.GetPtt, null, vfo);
        public int GetRepeaterOffset(int vfo) => RunWithFeatureCheck(innerRig.GetRepeaterOffset, GetRepeaterOffsetFromDictionary, vfo);
        public RepeaterShift GetRepeaterShift(int vfo) => RunWithFeatureCheck(innerRig.GetRepeaterShift, GetRepeaterShiftFromDictionary, vfo);
        public int PassbandNormal(RigMode mode) => RunWithFeatureCheck(innerRig.PassbandNormal, null, mode);

        public void SetFrequency(double freq, int vfo) => innerRig.SetFrequency(freq, vfo);

        public void SetMode(RigMode mode, long width, int vfo) => innerRig.SetMode(mode, width, vfo);

        public void SetPtt(PttMode onData, int current) => innerRig.SetPtt(onData, current);

        public void SetCTCSS(uint tone, int vfo)
        {
            ctcssTones[vfo] = tone;
            innerRig.SetCTCSS(tone, vfo);
        }

        public void SetRepeaterOffset(int rptr_offs, int vfo)
        {
            repeaterOffsets[vfo] = rptr_offs;
            innerRig.SetRepeaterOffset(rptr_offs, vfo);
        }

        public void SetRepeaterShift(RepeaterShift rptr_shift, int vfo)
        {
            repeaterShifts[vfo] = rptr_shift;
            innerRig.SetRepeaterShift(rptr_shift, vfo);
        }

        public void SetVFO(int v) => innerRig.SetVFO(v);

        private TOut GetFromDictionary<TKey, TOut>(IDictionary<TKey, TOut> dict, TKey key)
        {
            dict.TryGetValue(key, out TOut value);
            return value;
        }

        private uint GetCtcssFromDictionary(int vfo) => GetFromDictionary(ctcssTones, vfo);
        private int GetRepeaterOffsetFromDictionary(int vfo) => GetFromDictionary(repeaterOffsets, vfo);
        private RepeaterShift GetRepeaterShiftFromDictionary(int vfo) => GetFromDictionary(repeaterShifts, vfo);

        private TOut RunDelegateWithFeatureCheck<TOut>(Delegate d, Delegate fallback, string caller, params object[] args)
        {
            try
            {
                object result;
                if (FeatureIsAvailable(caller))
                {
                    try
                    {
                        result = d.DynamicInvoke(args);
                    }
                    catch (TargetInvocationException ex) when (ex.InnerException is RigException rex && rex.Message == "Feature not available")
                    {
                        MarkFeatureAsUnavailable(caller);
                        result = fallback.DynamicInvoke(args);
                    }
                }
                else
                {
                    result = fallback.DynamicInvoke(args);
                }

                if (result != null)
                {
                    return (TOut)result;
                }

                return default;

            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private void MarkFeatureAsUnavailable(string v)
        {
            lock (lockObj)
            {
                unavailableFeatures.Add(v);
            }
        }

        private bool FeatureIsAvailable(string v)
        {
            lock (lockObj)
            {
                return !unavailableFeatures.Contains(v);
            }
        }

        private TOut RunWithFeatureCheck<TIn1, TOut>(Func<TIn1, TOut> func, Func<TIn1, TOut> fallback, TIn1 arg1, [CallerMemberName] string caller = null)
            => RunDelegateWithFeatureCheck<TOut>(func, fallback, caller, arg1);
        /*private void RunWithFeatureCheck<TIn1>(Action<TIn1> action, Action<TIn1> fallback, TIn1 arg1, [CallerMemberName] string caller = null)
            => RunDelegateWithFeatureCheck<object>(action, fallback, caller, arg1);
        private void RunWithFeatureCheck<TIn1, TIn2>(Action<TIn1, TIn2> action, Action<TIn1, TIn2> fallback, TIn1 arg1, TIn2 arg2, [CallerMemberName] string caller = null)
            => RunDelegateWithFeatureCheck<object>(action, fallback, caller, arg1, arg2);
        private void RunWithFeatureCheck<TIn1, TIn2, TIn3>(Action<TIn1, TIn2, TIn3> action, Action<TIn1, TIn2, TIn3> fallback, TIn1 arg1, TIn2 arg2, TIn3 arg3, [CallerMemberName] string caller = null)
            => RunDelegateWithFeatureCheck<object>(action, fallback, caller, arg1, arg2, arg3);*/
    }
}
