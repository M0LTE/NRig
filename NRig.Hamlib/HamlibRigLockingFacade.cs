using HamLibSharp;

namespace NRig.Rigs.Hamlib
{
    internal class HamlibRigLockingFacade : IHamlibRig
    {
        private readonly Rig rig;
        private readonly object lockObj = new object();

        public HamlibRigLockingFacade(string rigName)
        {
            rig = new Rig(rigName);
        }

        public void Dispose()
        {
            rig.Dispose();
        }

        public void Open(string port, BaudRate baudRate, Handshake handshake, int dataBits, int stopBits)
        {
            rig.Open(port, (RigSerialBaudRate)baudRate, (RigSerialHandshake)handshake, dataBits, stopBits);
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

        public void SetPtt(PttMode ptt, int vfo)
        {
            lock (lockObj)
            {
                rig.SetPtt(ptt, vfo);
            }
        }

        public uint GetCtcss(int vfo)
        {
            lock (lockObj)
            {
                return rig.GetCTCSS(vfo);
            }
        }

        public PttMode GetPtt(int vfo)
        {
            lock (lockObj)
            {
                return rig.GetPtt(vfo);
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

        public (RigMode, long width) GetMode(int vfo)
        {
            lock (lockObj)
            {
                long width = default;
                var mode = rig.GetMode(ref width, vfo);
                return (mode, width);
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

        public int GetRepeaterOffset(int vfo)
        {
            lock (lockObj)
            {
                return rig.GetRepeaterOffset(vfo);
            }
        }

        public RepeaterShift GetRepeaterShift(int vfo)
        {
            lock (lockObj)
            {
                return rig.GetRepeaterShift(vfo);
            }
        }
    }
}