using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Level;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    public class LevelRecord : RecordBase
    {
        private readonly uint _recordSize;

        private uint _blockSetHeaderOffset;
        private int _blockSetCountX = 0;
        private int _blockSetCountY = 0;

        public int BasicDataFrameCount { get; set; }
        public int ExtendedDataFrameCount { get; set; }
        public int RouteGuidanceBasicFrameCount { get; set; }
        public int ParcelCount { get; private set; }

        public int RoadDisplayClassCount { get; set; }
        public int BgDisplayClassCount { get; set; }
        public int NameClassCount { get; set; }
        public Dictionary<int, int> DividedBlockCountByDivisionType { get; } = new Dictionary<int, int>();

        public LevelHeader Header { get; set; }

        public int BlockSetsCountLong { get; set; }
        public int BlockSetsCountLat { get; set; }
        public int BlocksCountLong { get; set; }
        public int BlocksCountLat { get; set; }
        public int ParcelsCountLong { get; set; }
        public int ParcelsCountLat { get; set; }

        public List<string> Scales { get; } = new List<string>();

        public LevelRecord(int number, uint recordSize, FrameBase parcelDataFrame) : base(parcelDataFrame)
        {
            Name = $"Level {number}";
            _recordSize = recordSize;
        }

        public override string Name { get; }

        public override bool HasChilds => true;

        private string GetScaleName(uint value)
        {
            if (value % 100_000 < 100_000)
                return $"1:{value / 100_000} km";
            if (value % 100 == 0)
                return $"1:{value / 100} m";
            return $"1:{value} cm";
        }

        protected override void ReadInternal(BinaryReader br)
        {
            Header = CreateField<LevelHeader>("Level Management Header", br);
            var recordCount = CreateField<RecordCount>("Number of Basic/Extended Data Frame ManagementRecords, of the Parcel Entity of the Level", br);

            BasicDataFrameCount = recordCount.BasicMapRecords;
            ExtendedDataFrameCount = recordCount.ExtendedMapRecords;
            RouteGuidanceBasicFrameCount = recordCount.BasicRouteRecords;

            var scale1 = CreateField<N>("Display Scale Flag 1", br, 4);
            var scale2 = CreateField<N>("Display Scale Flag 2", br, 4);
            var scale3 = CreateField<N>("Display Scale Flag 3", br, 4);
            var scale4 = CreateField<N>("Display Scale Flag 4", br, 4);
            var scale5 = CreateField<N>("Display Scale Flag 5", br, 4);

            if (!scale5.IsNull)
                Scales.Add(GetScaleName(scale5.Value));
            if (!scale4.IsNull)
                Scales.Add(GetScaleName(scale4.Value));
            if (!scale3.IsNull)
                Scales.Add(GetScaleName(scale3.Value));
            if (!scale2.IsNull)
                Scales.Add(GetScaleName(scale2.Value));
            if (!scale1.IsNull)
                Scales.Add(GetScaleName(scale1.Value));

            var blockSetCount = CreateField<ElementCount>("Number of Latitudinal/Longitudinal Block Sets", br);
            _blockSetCountX = blockSetCount.Longitudinal;
            _blockSetCountY = blockSetCount.Latitudinal;
            BlockSetsCountLong = blockSetCount.Longitudinal;
            BlockSetsCountLat = blockSetCount.Latitudinal;

            var blocksCount = CreateField<ElementCount>("Number of Latitudinal/Longitudinal Blocks", br);
            BlocksCountLong = blocksCount.Longitudinal;
            BlocksCountLat = blocksCount.Latitudinal;

            var parcelsCount = CreateField<ElementCount>("Number of Latitudinal/Longitudinal Parcels", br);
            ParcelCount = parcelsCount.Latitudinal * parcelsCount.Longitudinal;
            ParcelsCountLong = parcelsCount.Longitudinal;
            ParcelsCountLat = parcelsCount.Latitudinal;

            var div1 = CreateField<ElementCount>("Number of Latitudinal/Longitudinal Divided Parcels (Parcel Division Type 1)", br);
            var div2 = CreateField<ElementCount>("Number of Latitudinal/Longitudinal Divided Parcels (Parcel Division Type 2)", br);
            var div3 = CreateField<ElementCount>("Number of Latitudinal/Longitudinal Divided Parcels (Parcel Division Type 3)", br);
            DividedBlockCountByDivisionType.Add(1, div1.Latitudinal * div1.Longitudinal);
            DividedBlockCountByDivisionType.Add(2, div2.Latitudinal * div2.Longitudinal);
            DividedBlockCountByDivisionType.Add(3, div3.Latitudinal * div3.Longitudinal);

            _blockSetHeaderOffset = CreateField<D>("Offset to the Top of the Block Set Management Records of the Level", br).DValue;
            CreateField<SWS>("Node Record Size", br);
            var expansionCounts = CreateField<Expansion>("Expnasion counts", br);
            for (int i = 0; i < expansionCounts.RoadsCount; i++)
            {
                CreateField<N>("Road display class code " + i, br);
            }
            for (int i = 0; i < expansionCounts.BackgroundsCount; i++)
            {
                CreateField<N>("Background display class code " + i, br);
            }
            for (int i = 0; i < expansionCounts.DisplayClassCount; i++)
            {
                CreateField<N>("Name display class code " + i, br);
            }

            BgDisplayClassCount = expansionCounts.BackgroundsCount;
            RoadDisplayClassCount = expansionCounts.RoadsCount;
            NameClassCount = expansionCounts.DisplayClassCount;

            var currentLength = br.BaseStream.Position - Offset;
            if (currentLength < _recordSize)
            {
                br.BaseStream.Position += _recordSize - currentLength;
            }

            ReadBlockSets();
        }

        private void ReadBlockSets()
        {
            using (var file = _frame.OpenAt(_blockSetHeaderOffset))
            using (var br = new BinaryReader(file))
            {
                var index = 0;
                for (int i = 0; i < _blockSetCountY; i++)
                {
                    for (int j = 0; j < _blockSetCountX; j++)
                    {
                        AddRecord(new BlockSetRecord(index++, _frame), br);
                    }
                }
            }
        }
    }
}
