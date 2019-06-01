using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public enum AdjacentPosition: int
    {
        Upper = 0,
        UpperRight = 1,
        Right =2,
        BottomRight = 3,
        Bottom = 4,
        BottomLeft = 5,
        Left = 6,
        UpperLeft = 7
    };

    public class AdjacentParcelInfo : BinarySerializable
    {
        [ValueName("Parcel Location")]
        public DSA ParcelLocation { get; set; }
        [ValueName("Parcel Size")]
        public BS ParcelSize { get; set; }

        public D OffsetToDividedInfo { get; set; }
        public DividedParcelInfo DividedParcelInfo  { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            ParcelLocation = new DSA();
            ParcelSize = new BS();

            ParcelLocation.Read(br);
            ParcelSize.Read(br);

            if (ParcelSize.Size == 0 && !ParcelSize.IsNull && !ParcelLocation.IsNull)
            {
                ParcelLocation = null;
                br.BaseStream.Position -= 6;

                OffsetToDividedInfo = new D();
                DividedParcelInfo = new DividedParcelInfo();
                OffsetToDividedInfo.Read(br);
                DividedParcelInfo.Read(br);
                br.BaseStream.Position += 2;
            }
        }
    }
}
