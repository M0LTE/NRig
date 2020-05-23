using HamLibSharp;
using System.Collections.Concurrent;

namespace NRig.Rigs.Hamlib
{
    internal class CapabilityFakingHamLibRig : IHamlibRig
    {
        private readonly ConcurrentDictionary<int, int> repeaterOffsets = new ConcurrentDictionary<int, int>();
        private readonly ConcurrentDictionary<int, RepeaterShift> repeaterShifts = new ConcurrentDictionary<int, RepeaterShift>();
        private readonly ConcurrentDictionary<int, uint> ctcssTones = new ConcurrentDictionary<int, uint>();
        private readonly IHamlibRig innerRig;

        public CapabilityFakingHamLibRig(IHamlibRig innerRig)
        {
            this.innerRig = innerRig;
        }

        public void Dispose()
        {
            innerRig.Dispose();
        }

        public uint GetCtcss(int vfo)
        {
            try
            {
                return innerRig.GetCtcss(vfo);
            }
            catch (RigException ex) when (ex.Message == "Feature not available")
            {
                ctcssTones.TryGetValue(vfo, out uint value);
                return value;
            }
        }

        public double GetFrequency(int vfo)
        {
            return innerRig.GetFrequency(vfo);
        }

        public RigMode GetMode(ref long width, int vfo)
        {
            return innerRig.GetMode(ref width, vfo);
        }

        public PttMode GetPtt(int vfo)
        {
            return innerRig.GetPtt(vfo);
        }

        public int GetRepeaterOffset(int vfo)
        {
            try
            {
                return innerRig.GetRepeaterOffset(vfo);
            }
            catch (RigException ex) when (ex.Message == "Feature not available")
            {
                repeaterOffsets.TryGetValue(vfo, out int value);
                return value;
            }
        }

        public RepeaterShift GetRepeaterShift(int vfo)
        {
            try
            {
                return innerRig.GetRepeaterShift(vfo);
            }
            catch (RigException ex) when (ex.Message == "Feature not available")
            {
                repeaterShifts.TryGetValue(vfo, out RepeaterShift value);
                return value;
            }
        }

        public void Open(string port)
        {
            innerRig.Open(port);
        }

        public int PassbandNormal(RigMode mode)
        {
            return innerRig.PassbandNormal(mode);
        }

        public void SetCTCSS(uint tone, int vfo)
        {
            ctcssTones[vfo] = tone;

            innerRig.SetCTCSS(tone, vfo);
        }

        public void SetFrequency(double freq, int vfo)
        {
            innerRig.SetFrequency(freq, vfo);
        }

        public void SetMode(RigMode mode, long width, int vfo)
        {
            innerRig.SetMode(mode, width, vfo);
        }

        public void SetPtt(PttMode onData, int current)
        {
            innerRig.SetPtt(onData, current);
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

        public void SetVFO(int v)
        {
            innerRig.SetVFO(v);
        }
    }
}