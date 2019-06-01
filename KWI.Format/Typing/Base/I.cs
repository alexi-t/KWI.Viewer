using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class I : BinarySerializable
    {
        [ValueName("I")]
        public int IValue { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            IValue = BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray());
        }
    }
}
