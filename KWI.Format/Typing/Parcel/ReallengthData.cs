using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class RealLengthData : BinarySerializable
    {
        [ValueName("Magnification")]
        public bool Folded { get; set; }
        [ValueName("Actual Distance")]
        public int Value { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            Folded = (firstByte >> 7) == 0;
            Value = ((firstByte & 0b01111111) << 8) | br.ReadByte();
        }
    }
}
