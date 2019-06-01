using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    public class MinimunGraphicsDataRecord : RecordBase
    {
        private readonly int _number;

        public int StartX { get; set; }
        public int StartY { get; set; }
        public List<Point> Offsets { get; } = new List<Point>();

        public int IntegratedX { get; set; }
        public int IntegratedY { get; set; }

        public int MultConst { get; set; }

        public MinimunGraphicsDataRecord(int number, FrameBase frame) : base(frame)
        {
            _number = number;
        }

        public override string Name => $"Graphics {_number}";

        public override bool HasChilds => false;

        protected override void ReadInternal(BinaryReader br)
        {
            CreateField<GraphicsDataHeader>("Minimum Graphics Data Header", br);
            var pointsCount = CreateField<ScaleAndPointsCount>("Scale and Points count", br).PointCount;
            CreateField<N>("Type Code", br);
            var info = CreateField<BgInfoAndMultConst>("Add background info", br);
            MultConst = (int)Math.Pow(2, info.MultConstant);
            var x = CreateField<StartCoordinate>("Starting X-coordinate (Longitude)", br);
            var y =  CreateField<StartCoordinate>("Starting Y-coordinate (Latitude)", br);
            StartX = x.Coordinate;
            StartY = y.Coordinate;
            IntegratedX = x.RelativePositionWithinParcel;
            IntegratedY = y.RelativePositionWithinParcel;
            for (int i = 0; i < pointsCount; i++)
            {
                var offset = CreateField<OffsetCoordinate>($"Offset coord {i}", br);
                Offsets.Add(new Point(offset.XOffset, offset.YOffset));
            }
            //CreateField<D>("Name Offset", br);
            //CreateField<N>("Auxiliary Data", br);
        }
    }
}
