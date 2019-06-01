using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Graphics
{
    public enum ShapeType : int
    {
        Point = 0,
        Line = 1,
        Area = 2
    }

    public class BgGraphicsInfo : BinarySerializable
    {
        [ValueName("Share type")]
        public ShapeType ShapeType { get; set; }
        [ValueName("Has height")]
        public bool HasHeight { get; set; }
        [ValueName("Data count")]
        public int DataCount { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            ShapeType = (ShapeType)(firstByte >> 6);
            HasHeight = ((firstByte >> 5) & 0b001) == 1;
            var secondByte = br.ReadByte();
            DataCount = ((firstByte & 0b00001111) << 8) | secondByte;
        }
    }
}
