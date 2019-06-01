using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class ParcelLocation : BinarySerializable
    {
        [ValueName("X")]
        public int XPos { get; set; }
        [ValueName("Y")]
        public int YPos { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            YPos = br.ReadByte();
            XPos = br.ReadByte();
        }
    }
}
