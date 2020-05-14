using NRig.Rigs.Hamlib;
using System;
using System.Threading.Tasks;

namespace NRig.Hamlib.Harness
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rig = new HamLibWrapper("FT-857", "COM12");

            if (args[0] == "off")
            {
                await rig.SetCtcss(null);
            }
            else
            {
                await rig.SetCtcss(Frequency.Hz(decimal.Parse(args[0])));
            }
        }
    }
}