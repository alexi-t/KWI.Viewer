using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.BlockSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    public class BlockSetRecord : RecordBase
    {
        private int _blockSetNumber;
        private uint _blockTableOffset;
        private uint _blockTableSize;

        public int Index { get; }

        public BlockSetRecord(int index, FrameBase parcelDataFrame) : base(parcelDataFrame)
        {
            Index = index;
        }

        public override string Name => $"BlockSet {_blockSetNumber}";

        public override bool HasChilds => true;

        protected override void ReadInternal(BinaryReader br)
        {
            _blockSetNumber = CreateField<BlockSetHeader>("Block Set Management Header", br).BlockSetNumber;
            var blockTableAddress = CreateField<D>("Offset to Block Management Table", br, 4);
            Hidden = blockTableAddress.IsNull;
            _blockTableOffset = blockTableAddress.DValue;
            _blockTableSize = CreateField<SWS>("Size of Block Management Table", br, 4).SWSValue;
            if (!blockTableAddress.IsNull)
                ReadBlockTable();
        }

        protected void ReadBlockTable()
        {
            var parentLevel = FindParentOfType<LevelRecord>();

            using (var file = _frame.OpenAt(_blockTableOffset))
            using (var br = new BinaryReader(file))
            {
                int index = 0;
                for (int i = 0; i < parentLevel.BlocksCountLat; i++)
                {
                    for (int j = 0; j < parentLevel.BlocksCountLong; j++)
                    {
                        AddRecord(new BlockRecord(index++, _frame), br);
                    }
                }
            }
        }
    }
}
