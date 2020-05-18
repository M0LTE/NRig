using NRig.Rigs.Hamlib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NRig.Hamlib.Harness
{
    class Program
    {
        static object lockObj = new object();
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Codebase: {typeof(Program).Assembly.CodeBase.Replace("file:///", "")}");
            Console.WriteLine($"Managed version: {HamLibWrapper.ManagedVersion}");
            Console.WriteLine($"Native version:  {HamLibWrapper.NativeVersion}");

            var rig = new HamLibWrapper("FT-857", args[0]);

            RigStatus oldStatus = null;
            await rig.BeginRigStatusUpdates(status =>
            {
                lock (lockObj)
                {
                    if (oldStatus == null || !oldStatus.Equals(status))
                    {

                        Console.SetCursorPosition(0, 0);
                        Console.Write(status.VfoA.Frequency.ToString(FrequencyStyle.Yaesu));
                        oldStatus = status;

                    }
                }
            }, TimeSpan.FromSeconds(0.1));

            Console.Clear();
            
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.C)
                {
                    await rig.SetCtcss(Frequency.Hz(118.8));
                } 
                else if (key.Key == ConsoleKey.N)
                {
                    await rig.SetCtcss(null);
                }
                else if (key.Key == ConsoleKey.R)
                {
                    await rig.SetRepeaterShift(Frequency.MHz(1.6));
                }
                else if (key.Key == ConsoleKey.S)
                {
                    await rig.SetRepeaterShift(null);
                }
            }
        }
    }
}