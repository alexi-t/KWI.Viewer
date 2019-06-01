using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Typing.BlockSet
{
    public class BlockSetHeader : BinarySerializable
    {
        [ValueName("Level number")]
        public int LevelNumber { get; set; }
        [ValueName("BlockSet number")]
        public int BlockSetNumber { get; set; }

        public override void Read(BinaryReader br, int length = 0)
        {
            var firstByte = br.ReadByte();
            LevelNumber = firstByte >> 2;
            BlockSetNumber = br.ReadByte();
        }
    }
}
