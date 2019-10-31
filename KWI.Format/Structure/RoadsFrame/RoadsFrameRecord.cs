using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Roads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.RoadsFrame
{
    public class RoadsFrameRecord : FrameBase
    {
        private bool _hasChilds = false;
        private readonly string _fileName = string.Empty;

        public RoadsFrameRecord(FrameBase parentFrame)
        {
            _fileName = parentFrame.FileName;
        }

        public override string Name => "Roads";

        public override bool HasChilds => _hasChilds;

        public override string FileName => _fileName;

        protected override void ReadInternal(BinaryReader br)
        {
            CreateField<SWS>("Size of Roads Distribution Header", br);
            CreateField<N>("Total Number of Intersections", br);
            CreateField<N>("Number of Display Classes", br, 1);
            CreateField<N>("Count of Additional Data", br, 1);
            CreateField<RouteDataLevel>("Level of Route Planning Data Corresponding to Parcel Data", br);
            var parentLevel = FindParentOfType<LevelRecord>();
            var roadsDisplayClassCount = parentLevel.RoadDisplayClassCount;
            for (int i = 0; i < roadsDisplayClassCount; i++)
            {
                AddRecord(new RoadDisplayClassManagementRecord(i, this), br);
            }
        }
    }
}
