using HamLibSharp;
using System;

namespace NRig.Rigs.Hamlib
{
    internal interface IHamlibRig : IDisposable
    {
        double GetFrequency(int vfo);
        RigMode GetMode(ref long width, int vfo);
        void Open(string port);
        int PassbandNormal(RigMode mode);
        void SetCTCSS(uint tone, int vfo);
        void SetFrequency(double freq, int vfo);
        void SetMode(RigMode mode, long width, int vfo);
        void SetPtt(PttMode onData, int current);
        void SetRepeaterOffset(int rptr_offs, int vfo);
        void SetRepeaterShift(RepeaterShift rptr_shift, int vfo);
        void SetVFO(int v);
    }
}