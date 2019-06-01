using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing
{
    public class MidC : BinarySerializable
    {
        [ValueName("Maker Identification")]
        public MID MID { get; set; }
        [ValueName("System-specific Identification defined by the maker")]
        public string Str { get; set; }

        public override void Read(BinaryReader br, int length)
        {
            var mid = new MID();
            mid.Read(br);
            MID = mid;

            Str = Encoding.Default.GetString(br.ReadBytes(length - 12)).Trim('\0');
        }

        public override string ToString()
        {
            return $"{MID} {Str}";
        }
    }
}
