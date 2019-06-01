using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Level
{
    public class Expansion:BinarySerializable
    {
        [ValueName("Number of Road Display Classes")]
        public int RoadsCount { get; set; }
        [ValueName("Number of Background Display Classes")]
        public int BackgroundsCount { get; set; }
        [ValueName("Number of Name Display Classes")]
        public int DisplayClassCount { get; set; }
        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            RoadsCount = ((firstByte & 0b00111100) >> 2) + 1;
            var secondByte = br.ReadByte();
            BackgroundsCount = Math.Min(32, (((firstByte & 0b00000011) << 4) | (secondByte >> 4)) + 1);
            DisplayClassCount = (secondByte & 0b00001111) + 1;
        }
    }
}
