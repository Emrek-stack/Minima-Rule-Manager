using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CampaignEngine.Core.Models
{
    [DebuggerDisplay("{Currency}{Value}")]
    [JsonConverter(typeof(PriceJsonConverter))]
    public struct Price : IEquatable<Price>, IComparable<Price>
    {
        public decimal Value { get; set; }
        private string _currency;

        public string Currency
        {
            get { return _currency; }
            set
            {
                if (value != null && value.Length != 3)
                    throw new ArgumentException("The currency must be in ISO 4217 format (3 chars length)", nameof(Currency));
                _currency = value;
            }
        }

        public Price(decimal value, string currency) : this()
        {
            Value = value;
            Currency = currency;
        }

        private static readonly Regex _isoFormatRegex = new Regex("(?<currency>[ A-Za-z]{3})(?<integer>[-]?[0-9]+)[.]?(?<precision>[0-9]+)?", RegexOptions.Compiled);

        public static Price FromString(string priceString)
        {
            if (priceString == "0") return Zero;
            var match = _isoFormatRegex.Match(priceString);
            return new Price
            {
                Currency = match.Groups["currency"].Value,
                Value = decimal.Parse(match.Groups["integer"].Value.Trim() + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + match.Groups["precision"].Value.Trim(), CultureInfo.CurrentCulture)
            };
        }

        public override string ToString() => string.Format(new NumberFormatInfo { NumberDecimalSeparator = "." }, "{0}{1}", Currency, Value);

        public static Price operator +(Price left, Price right)
        {
            if (right.Value == 0) return left;
            if (left.Value == 0) return right;
            if (left.Currency != right.Currency) throw new ArgumentException("Cannot add prices with different currencies");
            return new Price { Currency = left.Currency, Value = left.Value + right.Value };
        }

        public static Price operator -(Price left, Price right)
        {
            if (right.Value == 0) return left;
            if (left.Value == 0) return new Price(-right.Value, right.Currency);
            if (left.Currency != right.Currency) throw new ArgumentException("Cannot subtract prices with different currencies");
            return new Price { Currency = left.Currency, Value = left.Value - right.Value };
        }

        public static Price operator *(Price left, decimal right) => right == 0 || left.Value == 0 ? Zero : new Price { Currency = left.Currency, Value = left.Value * right };
        public static Price operator /(Price left, decimal right) => right == 0 ? throw new DivideByZeroException() : left.Value == 0 ? Zero : new Price { Currency = left.Currency, Value = left.Value / right };
        public static bool operator ==(Price left, Price right) => left.Currency == right.Currency && left.Value == right.Value;
        public static bool operator !=(Price left, Price right) => !(left == right);

        public bool Equals(Price other) => this == other;
        public int CompareTo(Price other) => Currency != other.Currency ? throw new ArgumentException("Cannot compare prices with different currencies") : Value.CompareTo(other.Value);
        public override bool Equals(object obj) => obj is Price price && Equals(price);
        public override int GetHashCode() => HashCode.Combine(Value, Currency);

        public static Price Zero => new Price(0, null);

        internal class PriceJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(Price);
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var priceStr = serializer.Deserialize<string>(reader);
                return priceStr == null ? Zero : FromString(priceStr);
            }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue(value.ToString());
        }
    }
}
