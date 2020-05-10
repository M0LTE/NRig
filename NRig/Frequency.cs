using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRig
{
    [JsonConverter(typeof(FrequencyConverter))]
    public struct Frequency
    {
        private decimal ValueHz { get; set; }

        public static implicit operator long(Frequency frequency) => (long)frequency.ValueHz;
        public static implicit operator decimal(Frequency frequency) => frequency.ValueHz;
        public static implicit operator double(Frequency frequency) => (double)frequency.ValueHz;
        public static implicit operator Frequency(decimal hz) => Hz(hz);
        public static implicit operator Frequency(double hz) => Hz((decimal)hz);
        public static implicit operator Frequency(long hz) => Hz((decimal)hz);
        public override bool Equals(object obj) => ((Frequency)obj).ValueHz == ValueHz;
        public override string ToString() => ValueHz.ToString();
        public string ToString(string format) => ValueHz.ToString(format);
        public override int GetHashCode() => ValueHz.GetHashCode();
        public static bool operator ==(Frequency f1, Frequency f2) => f1.Equals(f2);
        public static bool operator !=(Frequency f1, Frequency f2) => !f1.Equals(f2);
        public static bool operator >(Frequency f1, Frequency f2) => f1.ValueHz > f2.ValueHz;
        public static bool operator <(Frequency f1, Frequency f2) => f1.ValueHz < f2.ValueHz;
        public static bool operator >=(Frequency f1, Frequency f2) => f1.ValueHz >= f2.ValueHz;
        public static bool operator <=(Frequency f1, Frequency f2) => f1.ValueHz <= f2.ValueHz;
        public static Frequency Hz(decimal hz) => new Frequency { ValueHz = hz };
        public static Frequency Hz(double hz) => new Frequency { ValueHz = (decimal)hz };
        public static Frequency KHz(decimal kHz) => new Frequency { ValueHz = kHz * 1000 };
        public static Frequency MHz(decimal mHz) => new Frequency { ValueHz = mHz * 1000000 };
        public static Frequency MHz(double mHz) => new Frequency { ValueHz = (decimal)mHz * 1000000 };
        public static Frequency MHz(long mHz) => new Frequency { ValueHz = (decimal)mHz * 1000000 };
        public static Frequency GHz(decimal gHz) => new Frequency { ValueHz = gHz * 1000000000 };
        public static Frequency THz(decimal tHz) => new Frequency { ValueHz = tHz * 1000000000000 };

        private class FrequencyConverter : JsonConverter<Frequency>
        {
            public override Frequency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Frequency.Hz(reader.GetDecimal());

            public override void Write(Utf8JsonWriter writer, Frequency value, JsonSerializerOptions options) => writer.WriteNumberValue(value.ValueHz);
        }
    }
}