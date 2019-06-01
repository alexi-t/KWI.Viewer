using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Block
{
    public class ParcelManagementType:BinarySerializable
    {
        [ValueName("Parcels Type Number given by the Number of Managed Parcels")]
        public int TypeNumber { get; set; }
        [ValueName("Parcel Management List Type Number")]
        public int ListTypeNumber { get; set; }
        public override void Read(BinaryReader br, int length = 0)
        {
            TypeNumber = br.ReadByte();
            ListTypeNumber = br.ReadByte();
        }
    }
}
