using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class D : BinarySerializable
    {
        [ValueName("D")]
        public uint DValue { get; set; }
        public override void Read(BinaryReader br, int length = 0)
        {
            var len = length > 0 ? length : 2;

            byte[] bytes = br.ReadBytes(len);

            if (len == 2)
                DValue = BitConverter.ToUInt16(bytes.Reverse().ToArray()) * (uint)2;
            else
                DValue = BitConverter.ToUInt32(bytes.Reverse().ToArray()) * 2;

            IsNull = bytes.All(b => b == 0xff);
        }

        public override string ToString()
        {
            return $"D={DValue}";
        }
    }
}
