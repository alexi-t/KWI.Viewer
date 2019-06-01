using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Level
{
    public class RecordCount : BinarySerializable
    {
        [ValueName("Number of Basic Data Frame Management Records of Main Map Data Frame")]
        public int BasicMapRecords { get; set; }
        [ValueName("Number of Extended Data Frame Management Records of Main Map Data Frame")]
        public int ExtendedMapRecords { get; set; }
        [ValueName("Number of Basic Data Frame Management Records of Route Guidance Data Frame")]
        public int BasicRouteRecords { get; set; }
        [ValueName("Number of Extended Data Frame Management Records of Route Guidance Data Frame")]
        public int ExtededRouteRecords { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            BasicMapRecords = firstByte >> 4;
            ExtendedMapRecords = firstByte & 0b00001111;
            var secondByte = br.ReadByte();
            BasicRouteRecords = secondByte >> 4;
            ExtededRouteRecords = secondByte & 0b00001111;
        }
    }
}
