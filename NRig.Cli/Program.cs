using NRig.Rigs.Yaesu;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NRig.Cli
{
    class Program
    {
        private static IRigController rig;

        private const string nrigcli = "nrigcli";

        static async Task Main(string[] args)
        {
            string port;
            int baud;

            (args, port, baud) = GetPort(args);

            rig = new Ft818(port, baud);

            try
            {
                await Run(args);
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Not implemented");
            }
        }

        private static Task Run(string[] args)
        {
            switch (args[0])
            {
                case "set":
                    return Set(args.SubArray(1));

                default:
                    Console.WriteLine($"Usage: {nrigcli} {set}");
                    return Task.CompletedTask;
            }
        }

        private const string set = "set";
        private const string ctcss = "ctcss";
        private const string rpt = "rpt";
        private const string off = "off";
        private const string freq = "freq";

        private static (string[] args, string port, int baud) GetPort(string[] args) => (args.SubArray(2), args[0], int.Parse(args[1]));

        private static Task Set(string[] setArgs)
        {
            switch (setArgs[0])
            {
                case ctcss:
                    return SetCtcss(setArgs.SubArray(1));
                case rpt:
                    return SetRepeaterShift(setArgs.SubArray(1));
                case freq:
                    return SetFrequency(setArgs.SubArray(1));
            }

            Console.WriteLine($"Usage: {nrigcli} {set} {ctcss}|{rpt}|{freq}");
            return Task.CompletedTask;
        }

        private static Task SetFrequency(string[] setFrequencyArgs)
        {
            if (double.TryParse(setFrequencyArgs[0], out double d))
            {
                Console.WriteLine($"Setting frequency to {d}");
                return rig.SetFrequency(Vfo.A, Frequency.MHz(d));
            }

            Console.WriteLine($"Usage: {nrigcli} {set} {freq} [number in MHz]");
            return Task.CompletedTask;
        }

        private static Task SetRepeaterShift(string[] setRepeaterShiftArgs)
        {
            if (setRepeaterShiftArgs[0] == off)
            {
                Console.WriteLine("Disabling repeater shift");
                return rig.SetRepeaterShift(null);
            }

            if (double.TryParse(setRepeaterShiftArgs[0], out double d))
            {
                Console.WriteLine($"Setting repeater shift to {d}");
                return rig.SetRepeaterShift(Frequency.MHz(d));
            }

            Console.WriteLine($"Usage: {nrigcli} {set} {rpt} {off}|[number in MHz]");
            return Task.CompletedTask;
        }

        private static Task SetCtcss(string[] setCtcssArgs)
        {
            if (setCtcssArgs[0] == "off")
            {
                Console.WriteLine("Disabling CTCSS");
                return rig.SetCtcss(null);
            }

            if (double.TryParse(setCtcssArgs[0], out double d))
            {
                Console.WriteLine($"Setting CTCSS to {d}");
                return rig.SetCtcss(d);
            }

            Console.WriteLine($"Usage: {nrigcli} {set} {ctcss} {off}|[number in hz]");
            return Task.CompletedTask;
        }
    }

    static class ExtensionMethods
    {
        public static T[] SubArray<T>(this T[] array, int startIndex) => array.Skip(startIndex).ToArray();
    }
}
