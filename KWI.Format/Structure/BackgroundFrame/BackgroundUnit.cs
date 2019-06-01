using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.BackgroundFrame
{
    public class BackgroundUnit : RecordBase
    {
        private string _name = "";
        private readonly int _number;

        public BackgroundUnit(int number, FrameBase frame) : base(frame)
        {
            _number = number;
        }

        public override string Name => $"{_name} {_number}";

        public override bool HasChilds => true;

        private uint _graphicsOffset = 0;
        private int _graphicsCount = 0;

        public int Number => _number;
        public ShapeType Type { get; private set; }

        protected override void ReadInternal(BinaryReader br)
        {
            _graphicsOffset =  CreateField<D>("Background Type Unit Offset", br).DValue;
            var info = CreateField<BgGraphicsInfo>("Graphics info", br);
            _name = info.ShapeType.ToString();
            _graphicsCount = info.DataCount;

            Type = info.ShapeType;

            var pos = br.BaseStream.Position;
            var elementUnit = FindParentOfType<BgElementInformation>();
            br.BaseStream.Position = elementUnit.ElementDataAbsoluteOffset + _graphicsOffset;
            for (int i = 0; i < _graphicsCount; i++)
            {
                AddRecord(new MinimunGraphicsDataRecord(i, _frame), br);
            }
            br.BaseStream.Position = pos;
        }

        protected override void LoadChildsInternal()
        {
            //var elementUnit = FindParentOfType<BgElementInformation>();

            //using var file = _frame.OpenAt(elementUnit.ElementDataAbsoluteOffset + _graphicsOffset, asAbsoluteOffset: true);
            //using (var br = new BinaryReader(file))
            //{
            //    for (int i = 0; i < _graphicsCount; i++)
            //    {
            //        AddRecord(new MinimunGraphicsDataRecord(i, _frame), br);
            //    }
            //}
        }
    }
}
