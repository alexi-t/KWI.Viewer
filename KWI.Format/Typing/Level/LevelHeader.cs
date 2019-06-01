using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Level
{
    public class LevelHeader : BinarySerializable
    {
        public enum HigherIntegrated : int
        {
            x11 = 0b0000,
            x22 = 0b0001,
            x44 = 0b0010,
            x88 = 0b0011,
            x1616 = 0b0100,
            x3232 = 0b0101,
            Unknow = 0b1111
        }
        public enum LowerIntegrated : int
        {
            x11 = 0b0000,
            x14 = 0b0001,
            x116 = 0b0010,
            x164 = 0b0011,
            x1256 = 0b0100,
            x11024 = 0b0101,
            Unknow = 0b1111
        }

        [ValueName("Level Number")]
        public int LevelCode { get; set; }
        [ValueName("Number of Regular Parcels Integrated on the Next-higher Level")]
        public HigherIntegrated HighIntegrated { get; set; }
        [ValueName("Number of Regular Parcels Divided on the Next-lower Level")]
        public LowerIntegrated LowIntegrated { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            LevelCode = br.ReadByte() >> 2;
            var parcelInfo = br.ReadByte();
            var highLevel = parcelInfo >> 4;
            var lowerLevel = parcelInfo << 28 >> 28;
            if (highLevel <= 0b0101)
                HighIntegrated = (HigherIntegrated)highLevel;
            else
                HighIntegrated = HigherIntegrated.Unknow;

            if (lowerLevel <= 0b0101)
                LowIntegrated = (LowerIntegrated)lowerLevel;
            else
                LowIntegrated = LowerIntegrated.Unknow;

        }
    }
}
