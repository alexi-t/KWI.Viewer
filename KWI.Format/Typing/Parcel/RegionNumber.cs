using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class RegionNumber : BinarySerializable
    {
        [ValueName("Level number")]
        public int LevelNumber { get; set; }
        [ValueName("Region number")]
        public uint RouteRegionNumber { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            LevelNumber = br.ReadByte() >> 2;
            br.ReadByte();
            RouteRegionNumber = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray());
        }
    }
}
