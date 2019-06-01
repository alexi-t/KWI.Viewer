using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class DataFrameInfo : BinarySerializable
    {
        [ValueName("Offset to Main Map Data Frame")]
        public D Displacement { get; set; }
        [ValueName("Size of Main Map Data Frame")]
        public SWS Size { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            Displacement = new D();
            Displacement.Read(br, 4);
            Size = new SWS();
            Size.Read(br);
        }
    }
}
