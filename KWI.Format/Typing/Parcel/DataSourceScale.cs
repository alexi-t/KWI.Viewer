using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class DataSourceScale:BinarySerializable
    {
        [ValueName("Height Information Flag")]
        public bool HeightInformation { get; set; }
        [ValueName("Data Source Scale Standard")]
        public int ScaleType { get; set; }
        [ValueName("Data Source Identifier")]
        public int Scale { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            HeightInformation = (firstByte >> 7) == 1;
            ScaleType = (firstByte >> 6) & 0b01;
            var secondByte = br.ReadByte();
            Scale = ((firstByte & 0b00111111) << 8) | secondByte;
        }
    }
}
