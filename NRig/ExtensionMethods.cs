using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace NRig
{
    internal static class ExtensionMethods
    {
        public static void Write(this SerialPort serialPort, params byte[] bytes) => serialPort.Write(bytes, 0, bytes.Length);
    }
}
