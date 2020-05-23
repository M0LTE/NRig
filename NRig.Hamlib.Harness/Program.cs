using NRig.Rigs.Hamlib;
using System;
using System.IO;
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
            HandleHamlib();
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
                        
                        Console.SetCursorPosition(0, 1);
                        Console.ForegroundColor = status.Ptt ? ConsoleColor.Red : ConsoleColor.Green;
                        Console.Write(status.Ptt ? "TX" : "RX");
                        Console.ResetColor();

                        Console.SetCursorPosition(0, 2);
                        Console.Write($"CTCSS {(status.CtcssEnabled ? "on " : "off")} {status.Ctcss}    ");

                        Console.SetCursorPosition(0, 3);
                        Console.Write($"RPT   {(status.RepeaterShiftEnabled ? "on " : "off")} {status.RepeaterShift}    ");

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
                    await rig.SetCtcss(Frequency.Hz(114.8));
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
                else if (key.Key == ConsoleKey.P)
                {
                    await rig.SetPttState(!oldStatus.Ptt);
                }
            }
        }

        private static void HandleHamlib()
        {
            var codebase = typeof(Program).Assembly.CodeBase.Replace("file://", "");
            Console.WriteLine($"Codebase: {codebase}");
            var runningFromDir = Path.GetDirectoryName(codebase);
            const string renamedLibFilename = "libhamlib-2.dll.so";
            const string unrenamedLibFilename = "libhamlib.so.2";
            var targetLib = Path.Combine(runningFromDir, renamedLibFilename);
            var unrenamedSourceLib = Path.Combine(Environment.CurrentDirectory, unrenamedLibFilename);
            var renamedSourceLib = Path.Combine(Environment.CurrentDirectory, renamedLibFilename);
            if (File.Exists(renamedSourceLib))
            {
                File.Copy(renamedSourceLib, targetLib, true);
                Console.WriteLine($"Copied {renamedSourceLib} to {targetLib}");
            }
            else if (File.Exists(unrenamedSourceLib))
            {
                File.Copy(unrenamedSourceLib, targetLib, true);
                Console.WriteLine($"Copied {unrenamedSourceLib} to {targetLib}");
            }

            if (!File.Exists(targetLib))
            {
                Console.WriteLine($"Warning: {targetLib} not found- hamlib needs to be installed system-wide or {renamedSourceLib} or {unrenamedSourceLib} needs to exist.");
            }
        }
    }
}