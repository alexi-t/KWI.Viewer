using KWI.Format.Structure.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public class C : BinarySerializable
    {
        [ValueName("Value")]
        public string Str { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            Str = Encoding.Default.GetString(br.ReadBytes(length)).Trim('\0');
        }

        public override string ToString()
        {
            return Str;
        }
    }
}
