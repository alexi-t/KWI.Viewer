using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Graphics
{
    public class GraphicsDataHeader:BinarySerializable
    {
        [ValueName("Delete Flag")]
        public bool DeleteFlag { get; set; }
        [ValueName("Temp Flag")]
        public bool TempInfoFlag { get; set; }
        [ValueName("Extended data Flag")]
        public bool ExtendedDataFlag { get; set; }
        [ValueName("Record size")]
        public int RecordSize { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            var secondByte = br.ReadByte();

            DeleteFlag = firstByte >> 7 == 1;
            TempInfoFlag = ((firstByte >> 6) & 1) == 1;
            ExtendedDataFlag = ((firstByte >> 5) & 1) == 1;

            RecordSize = (((firstByte & 0b00001111) << 8) | secondByte) << 1;
        }
    }
}
