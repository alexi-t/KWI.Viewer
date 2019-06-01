using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class DividedParcelInfo:BinarySerializable
    {
        public int DividedParcelsX { get; set; }
        public int DividedParcelsY { get; set; }
        public int AdjacentCount { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            DividedParcelsY = (firstByte >> 4) + 1;
            DividedParcelsX = (firstByte & 0b00001111) + 1;
            var secondByte = br.ReadByte();
            AdjacentCount = (secondByte & 0b00001111) + 1;
        }
    }
}
