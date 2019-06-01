using System;
using System.Collections.Generic;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class GeoCord
    {
        private readonly double _eights = 0;
        private int _sign;

        public GeoCord(int degree, int minute, double second, int sign = 1)
        {
            Degree = degree;
            if (Degree < 0)
                throw new ArgumentException("Set sign in constructor parameter");
            if (minute < 0 || minute > 60)
                throw new ArgumentException($"Minute out of range: {minute}");
            Minute = minute;

            if (second < 0 || second > 60)
                throw new ArgumentException($"Second out of range: {second}");
            Second = second;

            _sign = Math.Sign(sign) < 0 ? -1 : 1;

            _eights = _sign * (Degree * 3600 + Minute * 60 + Second) * 8;
        }

        private GeoCord(double eights)
        {

        }

        public int Degree { get; }
        public int Minute { get; }
        public double Second { get; }

        public override string ToString()
        {
            return $"{(_sign < 0 ? "-" : "")}{Degree}⁰ {Minute}' {Second}''";
        }

        public double GetAsEightsS() => _eights;

        public static GeoCord FromEights(double eights)
        {
            var mod = eights % 8;
            if (mod > 0 && eights > 1)
                eights += mod;
            var seconds = Math.Abs(eights / 8);
            var degrees = (int)(seconds / 3600);
            var minutes = (int)((seconds - degrees * 3600) / 60);
            var remainder = seconds - minutes * 60 - degrees * 3600;
            return new GeoCord(degrees, minutes, remainder, Math.Sign(eights));
        }

        public GeoCord Divide(int nominator)
        {
            var divided = _eights / nominator;
            return FromEights(divided);
        }

        public static GeoCord operator -(GeoCord left, GeoCord right) => FromEights(left._eights - right._eights);
        public static GeoCord operator +(GeoCord left, GeoCord right) => FromEights(left._eights + right._eights);
        public static bool operator >(GeoCord left, GeoCord right) => left._eights > right._eights;
        public static bool operator <(GeoCord left, GeoCord right) => left._eights < right._eights;

        public GeoCord Multiply(int amount)
        {
            var multiplied = _eights * amount;
            return FromEights(multiplied);
        }
    }
}
