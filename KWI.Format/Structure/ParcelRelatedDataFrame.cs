using KWI.Format.Structure.Base;
using KWI.Format.Typing;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Structure
{
    public class ParcelRelatedDataFrame : FrameBase
    {
        public override string Name => "Parcel-related Data Management Frame";

        private uint _headerSize = 0;
        private uint _levelManagementRecordCount = 0;
        private uint _blockSetManagementRecords = 0;

        public override bool HasChilds => true;

        public override string FileName => "ALLDATA.KWI";

        protected override void ReadInternal(BinaryReader br)
        {
            _headerSize = CreateField<SWS>("Header Size", br).SWSValue;
            br.BaseStream.Position += 2;
            CreateField<Bytes>("File Name Designation", br, 2);
            br.BaseStream.Position += 2;
            CreateField<BNCoord>("Latitude of Upper Edge of Coverage Area", br, 2);
            CreateField<BNCoord>("Latitude of Lower Edge of Coverage Area", br, 2);
            CreateField<BNCoord>("Longitude of Left Edge of Coverage Area", br, 2);
            CreateField<BNCoord>("Longitude of Right Edge of Coverage Area", br, 2);
            var levelSize = CreateField<SWS>("Size of Level Management Record", br);
            CreateField<SWS>("Size of Block Set Management Record", br);
            CreateField<SWS>("Size of Block Management Record", br);
            _levelManagementRecordCount = CreateField<N>("A Sequence of Level Management Records", br).Value;
            _blockSetManagementRecords = CreateField<N>("Total Number of Block Set Management Records", br).Value;

            ReadLevels(br, levelSize.SWSValue);
        }

        private void ReadLevels(BinaryReader br, uint levelSize)
        {
            for (int i = 0; i < _levelManagementRecordCount; i++)
            {
                AddRecord(new LevelRecord(i, levelSize, this), br);
            }
        }
    }
}
