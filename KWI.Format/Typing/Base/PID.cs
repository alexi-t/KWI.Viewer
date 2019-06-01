using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Base
{
    /// <summary>
    /// 1.2.13 Parcel ID Number Type
    /// </summary>
    public class PID : BinarySerializable
    {
        [ValueName("Lattitude")]
        public GeoCord Lattitude { get; set; }
        [ValueName("Longitude")]
        public GeoCord Longitude { get; set; }

        private GeoCord WordsToCord(byte[] bytes)
        {
            uint w1 = bytes[0];
            uint w2 = bytes[1];
            uint w3 = bytes[2];
            uint w4 = bytes[3];

            var sign = w1 >> 7 == 1 ? -1 : 1;
            uint seconds = ((w1 << 25) >> 12) | (w2 << 5) | (w3 >> 3);
            var secondsEight = (w3 << 29) >> 29;

            var degree = seconds / 3600;
            var minutes = seconds / 60 - degree * 60;
            var remainSeconds = seconds - minutes * 60 - degree * 3600 + 0.125 * secondsEight;
            return new GeoCord((int)degree, (int)minutes, remainSeconds, sign);
        }

        public override void Read(BinaryReader br, int length = 0)
        {
            Lattitude = WordsToCord(br.ReadBytes(4));
            Longitude = WordsToCord(br.ReadBytes(4));
        }

        public override string ToString()
        {
            return $"Lat={Lattitude} Long={Longitude}";
        }
    }
}
