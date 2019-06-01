using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing
{
    public class BNCoord : BinarySerializable
    {
        [ValueName("Coord")]
        public GeoCord Cord { get; set; }

        [ValueName("North/South East/West Flag")]
        public uint Flag { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            uint b1 = br.ReadByte();
            uint b2 = br.ReadByte();
            uint b3 = br.ReadByte();

            Flag = b1 >> 7;

            uint cords = (b1 << 25 >> 9) | (b2 << 8) | b3;

            var totalDegree = cords * 0.125;
            int degree = (int)totalDegree / 3600;
            int minute = ((int)totalDegree - degree * 3600) / 60;
            var seconds = totalDegree - degree * 3600 - minute * 60;
            Cord = new GeoCord(degree, minute, seconds);
        }
    }
}
