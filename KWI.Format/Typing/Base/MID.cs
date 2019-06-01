using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    /// <summary>
    /// 1.2.14 Manufacturer Identification Type
    /// </summary>
    public class MID : BinarySerializable
    {
        [ValueName("Latitude and longitude at which manufacturer's office is located")]
        public PID ParcelID { get; private set; }
        [ValueName("Information about floor on which manufacturer's office is located")]
        public byte Floor { get; set; }
        [ValueName("Date at which manufacturer's identification was set")]
        public DateTime Date { get; set; }


        public override void Read(BinaryReader br, int length = 0)
        {
            var pid = new PID();
            pid.Read(br);
            ParcelID = pid;

            Floor = br.ReadByte();

            br.ReadByte();

            Date = ReadDate(br);
        }

        public override string ToString()
        {
            return $"PID={ParcelID} Floor={Floor} Date={Date}";
        }
    }
}
