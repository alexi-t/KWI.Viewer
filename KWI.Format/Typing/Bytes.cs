using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing
{
    public class Bytes : BinarySerializable
    {
        [ValueName("HEX")]
        public string Hex { get; set; }
        [ValueName("Binary")]
        public string Binary { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var bytes = br.ReadBytes(length);
            Hex = string.Join("", bytes.Select(b => b.ToString("X2")));
            Binary = string.Join(" ", bytes.Select(b => Convert.ToString(b, toBase: 2)));
        }
    }
}
