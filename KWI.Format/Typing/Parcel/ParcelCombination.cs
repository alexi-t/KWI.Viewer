using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class ParcelCombination : BinarySerializable
    {
        [ValueName("Divided")]
        public bool Divided { get; set; }
        [ValueName("Integrated")]
        public bool Integrated { get; set; }
        [ValueName("Whole")]
        public bool Whole { get; set; }
        [ValueName("Adjacent Parcel Address Information Flag")]
        public bool HasAdjancent { get; set; }
        [ValueName("Type Number given by the Number of Managed Parcels")]
        public int ParcelDivideType { get; set; }

        [ValueName("Location Y for divided, integrated Y otherwise")]
        public int CombinationYInfo { get; set; }
        [ValueName("Location X for divided, integrated X otherwise")]
        public int CombinationXInfo { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();

            Divided = (firstByte >> 6) == 1;
            Integrated = (firstByte >> 6) == 0b10;
            Whole = (firstByte >> 6) == 0b11;

            HasAdjancent = ((firstByte >> 5) & 1) == 0;

            ParcelDivideType = firstByte & 0b11;

            var secondByte = br.ReadByte();
            CombinationYInfo = secondByte >> 4;
            CombinationXInfo = secondByte & 0b1111;
        }
    }
}
