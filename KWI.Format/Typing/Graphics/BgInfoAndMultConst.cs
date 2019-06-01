using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Graphics
{
    public enum AddInfoType : int
    {
        FrameA = 0,
        FrameB = 1,
        BuildingID = 2
    }

    public class BgInfoAndMultConst : BinarySerializable
    {
        [ValueName("Info type")]
        public AddInfoType InfoType { get; set; }
        [ValueName("Has info flag")]
        public bool HasAddInfoFlag { get; set; }
        [ValueName("Name flag")]
        public bool NameFlag { get; set; }
        [ValueName("Auxiliary data flag")]
        public bool AuxiliaryDataFlag { get; set; }
        [ValueName("Pen up flag")]
        public bool PenUpFlag { get; set; }
        [ValueName("Underground flag")]
        public bool UndergroundFlag { get; set; }
        [ValueName("Multiplication Constant")]
        public int MultConstant { get; set; }
        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            var secondByte = br.ReadByte();

            InfoType = (AddInfoType)(firstByte >> 6);

            HasAddInfoFlag = firstByte >> 5 == 1;
            NameFlag = ((firstByte >> 4) & 1) == 1;
            AuxiliaryDataFlag = ((firstByte >> 3) & 1) == 1;
            PenUpFlag = ((firstByte >> 2) & 1) == 1;
            UndergroundFlag = ((firstByte >> 1) & 1) == 1;

            MultConstant = secondByte & 0b11;
        }
    }
}
