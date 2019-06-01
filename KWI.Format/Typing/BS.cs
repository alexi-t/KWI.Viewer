using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing
{
    public class BS:BinarySerializable
    {
        [ValueName("Size")]
        public uint Size { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var sectorsCount = new N();
            sectorsCount.Read(br);
            Size = 32 * (uint)sectorsCount.Value;
        }

        public override string ToString()
        {
            return $"Size = {Size}";
        }
    }
}
