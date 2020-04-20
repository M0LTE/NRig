using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRig
{
    [JsonConverter(typeof(FrequencyConverter))]
    public struct Frequency
    {
        private long ValueHz { get; set; }

        public static implicit operator long(Frequency frequency) => frequency.ValueHz;
        public static implicit operator Frequency(long hz) => Hz(hz);
        public override bool Equals(object obj) => ((Frequency)obj).ValueHz == ValueHz;
        public override string ToString() => ValueHz.ToString();
        public string ToString(string format) => ValueHz.ToString(format);
        public override int GetHashCode() => ValueHz.GetHashCode();
        public static bool operator ==(Frequency f1, Frequency f2) => f1.Equals(f2);
        public static bool operator !=(Frequency f1, Frequency f2) => !f1.Equals(f2);
        public static Frequency Hz(long hz) => new Frequency { ValueHz = hz };
        public static Frequency KHz(long kHz) => new Frequency { ValueHz = kHz * 1000 };
        public static Frequency MHz(long mHz) => new Frequency { ValueHz = mHz * 1000000 };
        public static Frequency GHz(long gHz) => new Frequency { ValueHz = gHz * 1000000000 };
        public static Frequency THz(long tHz) => new Frequency { ValueHz = tHz * 1000000000000 };

        private class FrequencyConverter : JsonConverter<Frequency>
        {
            public override Frequency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Frequency.Hz(reader.GetInt64());

            public override void Write(Utf8JsonWriter writer, Frequency value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }
    }
}