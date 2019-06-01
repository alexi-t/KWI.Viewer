using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Block;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    public class ParcelInformationRecord : RecordBase
    {
        private long _mainMapParcelListOffset;
        private int _parcelDivisionType;
        private int _parcelListType;
        private uint _routeGuidanceListOffset;
        private bool _noRouteData;



        public ParcelInformationRecord(FrameBase frame) : base(frame)
        {
        }

        public override string Name => "ParcelInfo";

        public override bool HasChilds => true;

        protected override void ReadInternal(BinaryReader br)
        {
            var parcelManagementInformation = CreateField<ParcelManagementType>("Parcel Management Information", br);
            var routeGuidanceListOffsetField = CreateField<D>("Offset to Route Guidance Parcel Management List", br);
            _routeGuidanceListOffset = !routeGuidanceListOffsetField.IsNull ? routeGuidanceListOffsetField.DValue : 0;
            _noRouteData = routeGuidanceListOffsetField.IsNull;

            _mainMapParcelListOffset = br.BaseStream.Position;
            _parcelDivisionType = parcelManagementInformation.TypeNumber;
            _parcelListType = parcelManagementInformation.ListTypeNumber;
        }

        protected override void LoadChildsInternal()
        {
            if (_parcelDivisionType > 3)
                return;

            using var file = _frame.OpenAt(_mainMapParcelListOffset, asAbsoluteOffset: true);
            using var br = new BinaryReader(file);
            {
                if (_routeGuidanceListOffset > 0 || _noRouteData)
                {
                    ReadMainMapParcelList(br);                        
                }
                if (!_noRouteData)
                {
                    file.Position = Offset + _routeGuidanceListOffset;
                    ReadRouteGuidanceList(br);
                }
                
            }
        }

        private void ReadRouteGuidanceList(BinaryReader br)
        {
            var parentLevel = FindParentOfType<LevelRecord>();

            int parcelsCount;
            if (_parcelDivisionType == 0)
                parcelsCount = parentLevel.ParcelCount;
            else
                parcelsCount = parentLevel.DividedBlockCountByDivisionType[_parcelDivisionType];

            for (int i = 0; i < parcelsCount; i++)
            {
                AddRecord(new RouteGuidanceRecord(i, _parcelListType, _frame), br);
            }
        }

        private void ReadMainMapParcelList(BinaryReader br)
        {
            var parentLevel = FindParentOfType<LevelRecord>();

            int parcelsCount;
            if (_parcelDivisionType == 0)
                parcelsCount = parentLevel.ParcelCount;
            else
                parcelsCount = parentLevel.DividedBlockCountByDivisionType[_parcelDivisionType];

            for (int i = 0; i < parcelsCount; i++)
            {
                AddRecord(new ParcelRecord(i, _parcelListType, _frame), br);
            }
        }
    }
}
