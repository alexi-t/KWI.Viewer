using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.BackgroundFrame
{
    public class BgElementInformation : RecordBase
    {
        private readonly int _number;

        public BgElementInformation(int number, FrameBase frame): base(frame)
        {
            _number = number;
        }

        public override string Name => $"Display class {_number}";
        public int DisplayType => _number;

        public override bool HasChilds => true;

        private uint _dataOffset = 0;

        public long ElementDataAbsoluteOffset => _frame.Offset + _dataOffset;

        protected override void ReadInternal(BinaryReader br)
        {
            var offset = CreateField<D>("Element Unit Offset", br);
            var size = CreateField<SWS>("Element Unit Size", br);

            if (offset.IsNull)
                Hidden = true;

            _dataOffset = offset.DValue;

            if (!offset.IsNull)
            {
                var pos = br.BaseStream.Position;
                br.BaseStream.Position = _frame.Offset + _dataOffset;
                var bgTypesCount = CreateField<N>("Number of Background Types", br);
                for (int i = 0; i < bgTypesCount.Value; i++)
                {
                    AddRecord(new BackgroundUnit(i, _frame), br);
                }
                br.BaseStream.Position = pos;
            }
        }

        protected override void LoadChildsInternal()
        {
            //using var file = _frame.OpenAt(_dataOffset);
            //using (var br = new BinaryReader(file))
            //{
            //    var bgTypesCount = CreateField<N>("Number of Background Types", br);
            //    for (int i = 0; i < bgTypesCount.Value; i++)
            //    {
            //        AddRecord(new BackgroundUnit(i, _frame), br);
            //    }
            //}
        }
    }
}
