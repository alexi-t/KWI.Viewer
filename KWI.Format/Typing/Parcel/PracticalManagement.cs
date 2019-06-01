using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Parcel
{
    public class PracticalManagement : BinarySerializable
    {
        [ValueName("Area code")]
        public int AreaCode { get; set; }
        [ValueName("Infrastructure 1")]
        public bool Infrastructure1Exist { get; set; }
        [ValueName("Infrastructure 2")]
        public bool Infrastructure2Exist { get; set; }
        [ValueName("DataForAllRoadsHasBeenCreated")]
        public bool DataForAllRoadsHasBeenCreated { get; set; }
        [ValueName("SuburbOrCityFlagExist")]
        public bool SuburbOrCityFlagExist { get; set; }
        [ValueName("SubUrbCityFlag")]
        public bool SubUrbCityFlag { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            AreaCode = br.ReadByte();

            var secondByte = br.ReadByte();
            Infrastructure1Exist = secondByte >> 7 == 1;
            Infrastructure2Exist = ((secondByte >> 6) & 0b01) == 1;

            var thirdByte = br.ReadByte();
            DataForAllRoadsHasBeenCreated = (thirdByte >> 7) == 1;
            SuburbOrCityFlagExist = ((thirdByte >> 6) & 0b01) == 1;
            SubUrbCityFlag = ((thirdByte >> 5) & 0b001) == 1;

            br.ReadByte();
        }
    }
}
