using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Graphics
{
    public class StartCoordinate : BinarySerializable
    {
        [ValueName("Relative Position within an Integrated Parcel")]
        public int RelativePositionWithinParcel { get; set; }
        [ValueName("Starting coordinate")]
        public int Coordinate { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            var secondByte = br.ReadByte();

            RelativePositionWithinParcel = firstByte >> 5;
            Coordinate = ((firstByte & 0b00011111) << 8) | secondByte;
        }
    }
}
