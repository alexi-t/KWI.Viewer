using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Graphics
{
    public class ScaleAndPointsCount : BinarySerializable
    {
        [ValueName("Display scale flag 1")]
        public bool Scale1 { get; set; }
        [ValueName("Display scale flag 1")]
        public bool Scale2 { get; set; }
        [ValueName("Display scale flag 1")]
        public bool Scale3 { get; set; }
        [ValueName("Display scale flag 1")]
        public bool Scale4 { get; set; }
        [ValueName("Display scale flag 1")]
        public bool Scale5 { get; set; }
        [ValueName("Point count")]
        public int PointCount { get; set; }
        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            var secondByte = br.ReadByte();

            Scale1 = firstByte >> 7 == 1;
            Scale2 = ((firstByte >> 6) & 1) == 1;
            Scale3 = ((firstByte >> 5) & 1) == 1;
            Scale4 = ((firstByte >> 4) & 1) == 1;
            Scale5 = ((firstByte >> 3) & 1) == 1;

            PointCount = ((firstByte & 0b00000111) << 8) | secondByte;
        }
    }
}
