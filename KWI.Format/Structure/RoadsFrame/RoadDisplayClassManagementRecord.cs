using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.RoadsFrame
{
    public class RoadDisplayClassManagementRecord : RecordBase
    {
        private readonly int _index;

        public RoadDisplayClassManagementRecord(int index, FrameBase frame) : base(frame)
        {
            _index = index;
        }

        public override string Name => $"Display class {_index}";

        public override bool HasChilds => true;

        protected override void ReadInternal(BinaryReader br)
        {
            var offset = CreateField<D>("Offset by Display Class", br);
            if (offset.IsNull)
                Hidden = true;
            var polylinesCount = CreateField<N>("Number of Polylines by Display Class", br);
        }
    }
}
