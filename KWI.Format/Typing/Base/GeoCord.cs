using System;
using System.Collections.Generic;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public struct GeoCord
    {
        private readonly int _seconds;
        private readonly short _eights;
        private readonly bool _negative;

        public GeoCord(int degree, int minute, double seconds, int sign = 1)
        {
            if (degree < 0)
                throw new ArgumentException("Set sign in constructor parameter");
            if (minute < 0 || minute > 60)
                throw new ArgumentException($"Minute out of range: {minute}");
            
            if (seconds < 0 || seconds > 60)
                throw new ArgumentException($"Second out of range: {seconds}");

            var secondsInt = (int)Math.Floor(seconds);
            _seconds = sign * (degree * 3600 + minute * 60 + secondsInt);
            _eights = (short)(sign * (seconds - secondsInt) / 0.125);
            _negative = sign < 0;
            _cached = string.Empty;
        }

        public GeoCord(double seconds)
        {
            _seconds = (int)Math.Floor(seconds);
            _eights = (short)((seconds - _seconds) / 0.125);
            _negative = seconds < 0;
            _cached = string.Empty;
        }

        private string _cached;
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_cached))
                return _cached;
            var degree = _seconds / 3600;
            var minute = _seconds % 3600 / 60;
            var second = _seconds % 3600 % 60;
            return _cached = $"{(_negative ? "-" : "")}{degree}⁰ {minute}' {second}.{Math.Abs(_eights) * 0.125}''";
        }

        public double GetAsEightSeconds() => _seconds * 8 + _eights;

        public static GeoCord operator -(GeoCord left, GeoCord right) => new GeoCord(left._seconds + 0.125 * left._eights - (right._seconds + 0.125 * right._eights));
        public static GeoCord operator +(GeoCord left, GeoCord right) => new GeoCord(left._seconds + 0.125 * left._eights + right._seconds + 0.125 * right._eights);
        public static bool operator >(GeoCord left, GeoCord right) => left._seconds > right._seconds;
        public static bool operator <(GeoCord left, GeoCord right) => left._seconds < right._seconds;

        public GeoCord Divide(int nominator)
        {
            var divided = (_seconds + (_negative ? -_eights : _eights) * 0.125) / nominator;
            return new GeoCord(divided);
        }

        public GeoCord Multiply(int amount)
        {
            var multiplied = (_seconds + (_negative ? -_eights : _eights) * 0.125) * amount;
            return new GeoCord(multiplied);
        }
    }
}
