using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class SWS : BinarySerializable
    {
        [ValueName("SWS")]
        public uint SWSValue { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var len = length > 0 ? length : 2;
            if (len == 2)
                SWSValue = (uint)(BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray()) << 1);
            else
                SWSValue = (BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray()) << 1);
        }

        public override string ToString()
        {
            return $"SWS={SWSValue}";
        }
    }
}
