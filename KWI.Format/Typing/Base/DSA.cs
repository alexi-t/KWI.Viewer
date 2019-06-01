using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class DSA : BinarySerializable
    {
        public enum DiskSide { A, B }
        public enum StorageType { SingleLayer, DoubleLayer }
        [ValueName("Sector address")]
        public uint SectorAddress { get; set; }
        [ValueName("Side")]
        public DiskSide Side { get; set; }
        [ValueName("Disk type")]
        public StorageType DiskType { get; set; }
        [ValueName("Sector count")]
        public uint SectorCount { get; set; }
        [ValueName("Computed Address")]
        public uint ComputedAddress { get; set; }
        [ValueName("Computed Address Hex")]
        public string ComputeAddressHex { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var addressBytes = br.ReadBytes(3);
            SectorAddress = BitConverter.ToUInt32(addressBytes.Reverse().Concat(new byte[] { 0 }).ToArray());
            uint lastByte = br.ReadByte();
            Side = lastByte >> 7 == 0 ? DiskSide.A : DiskSide.B;
            DiskType = lastByte >> 6 == 0 ? StorageType.SingleLayer : StorageType.DoubleLayer;
            SectorCount = lastByte << 26 >> 26;

            ComputedAddress = SectorAddress * 2048 + SectorCount * 32;
            ComputeAddressHex = "0x" + Convert.ToString(ComputedAddress, toBase: 16).ToUpper();

            IsNull = addressBytes.All(b => b == 0xff) && lastByte == 0xff;
        }

        public override string ToString() => ComputeAddressHex;
    }
}
