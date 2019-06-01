using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class N : BinarySerializable
    {
        [ValueName("N")]
        public uint Value { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var len = length > 0 ? length : 2;
            var bytes = br.ReadBytes(len);
            if (len == 2)
                Value = BitConverter.ToUInt16(bytes.Reverse().ToArray());
            else if (len == 4)
                Value = BitConverter.ToUInt32(bytes.Reverse().ToArray());

            IsNull = bytes.All(b => b == 0xff);
        }
    }
}
