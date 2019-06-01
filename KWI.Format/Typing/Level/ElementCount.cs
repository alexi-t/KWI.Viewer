using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Level
{
    public class ElementCount : BinarySerializable
    {
        [ValueName("Latitudinal")]
        public int Latitudinal { get; set; }
        [ValueName("Longitudinal")]
        public int Longitudinal { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            Latitudinal = br.ReadByte() + 1;
            Longitudinal = br.ReadByte() + 1;
        }
    }
}
