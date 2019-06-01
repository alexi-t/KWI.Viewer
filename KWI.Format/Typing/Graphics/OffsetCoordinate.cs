using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Graphics
{
    public class OffsetCoordinate : BinarySerializable
    {
        [ValueName("X")]
        public int XOffset { get; set; }
        [ValueName("Y")]
        public int YOffset { get; set; }
        public override void Read(BinaryReader br, int length = 0)
        {
            var x = br.ReadByte();
            XOffset = (x >> 7) == 1 ? (-1 << 8) | x : x;
            var y = br.ReadByte();
            YOffset = (y >> 7) == 1 ? (-1 << 8) | y : y;
        }
    }
}
