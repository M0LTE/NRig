using HamLibSharp;

namespace NRig.Rigs.Hamlib
{
    internal class HamlibRigFacade : IHamlibRig
    {
        private readonly Rig rig;
        private readonly object lockObj = new object();

        public HamlibRigFacade(string rigName)
        {
            rig = new Rig(rigName);
        }

        public void Dispose()
        {
            rig.Dispose();
        }

        public void Open(string port)
        {
            rig.Open(port);
        }

        public void SetFrequency(double freq, int vfo)
        {
            lock (lockObj)
            {
                rig.SetFrequency(freq, vfo);
            }
        }

        public void SetVFO(int v)
        {
            lock (lockObj)
            {
                rig.SetVFO(v);
            }
        }

        public void SetPtt(PttMode onData, int current)
        {
            lock (lockObj)
            {
                rig.SetPtt(onData, current);
            }
        }

        public void SetMode(RigMode mode, long width, int vfo)
        {
            lock (lockObj)
            {
                rig.SetMode(mode, width, vfo);
            }
        }

        public int PassbandNormal(RigMode mode)
        {
            return rig.PassbandNormal(mode);
        }

        public double GetFrequency(int vfo)
        {
            lock (lockObj)
            {
                return rig.GetFrequency(vfo);
            }
        }

        public RigMode GetMode(ref long width, int vfo)
        {
            lock (lockObj)
            {
                return rig.GetMode(ref width, vfo);
            }
        }

        public void SetCTCSS(uint tone, int vfo)
        {
            lock (lockObj)
            {
                rig.SetCTCSS(tone, vfo);
            }
        }

        public void SetRepeaterShift(RepeaterShift rptr_shift, int vfo)
        {
            lock (lockObj)
            {
                rig.SetRepeaterShift(rptr_shift, vfo);
            }
        }

        public void SetRepeaterOffset(int rptr_offs, int vfo)
        {
            lock (lockObj)
            {
                rig.SetRepeaterOffset(rptr_offs, vfo);
            }
        }
    }
}