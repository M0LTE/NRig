using HamLibSharp;
using System;

namespace NRig.Rigs.Hamlib
{
    internal interface IHamlibRig : IDisposable
    {
        double GetFrequency(int vfo);
        (RigMode, long width) GetMode(int vfo);
        void Open(string port, BaudRate baud, Handshake handshake, int dataBits, int stopBits);
        int PassbandNormal(RigMode mode);
        uint GetCtcss(int vfo);
        void SetCTCSS(uint tone, int vfo);
        void SetFrequency(double freq, int vfo);
        void SetMode(RigMode mode, long width, int vfo);
        void SetPtt(PttMode onData, int current);
        int GetRepeaterOffset(int vfo);
        void SetRepeaterOffset(int rptr_offs, int vfo);
        RepeaterShift GetRepeaterShift(int vfo);
        void SetRepeaterShift(RepeaterShift rptr_shift, int vfo);
        void SetVFO(int v);
        PttMode GetPtt(int vfo);
    }

    public enum BaudRate
    {
        BaudNone = 0,
        Baud2400 = 2400,
        Baud4800 = 4800,
        Baud9600 = 9600,
        Baud14400 = 14400,
        Baud19200 = 19200,
        Baud28800 = 28800,
        Baud38400 = 38400,
        Baud56000 = 56000,
        Baud57600 = 57600,
        Baud115200 = 115200,
        Baud128000 = 128000,
        Baud256000 = 256000
    }

    public enum Handshake
    {
        None = 0,
        XonXoff = 1,
        Hardware = 2
    }
}