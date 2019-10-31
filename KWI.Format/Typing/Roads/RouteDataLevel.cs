using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Roads
{
    public class RouteDataLevel : BinarySerializable
    {
        [ValueName("Data level")]
        public int Level { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var b1 = br.ReadByte();
            var b2 = br.ReadByte();
            if (b1 == 0b11111100)
                IsNull = true;
            else
                Level = b1 >> 7 == 1 ? 0b11000000 | (b1 >> 2) : (b1 >> 2);
        }
    }
}
