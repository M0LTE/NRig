using System;
using System.IO.Ports;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NRig.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace NRig
{
    internal interface ISerialPort : IDisposable
    {
        void Write(params byte[] bytes);
        byte ReadByte();
    }

    internal sealed class SerialPortWrapper : ISerialPort
    {
        private readonly SerialPort serialPort;

        public SerialPortWrapper(SerialPort serialPort) => this.serialPort = serialPort;

        public byte ReadByte() => (byte)serialPort.ReadByte();
        public void Write(byte[] bytes) => serialPort.Write(bytes);
        public void Dispose() => serialPort?.Dispose();
    }
}