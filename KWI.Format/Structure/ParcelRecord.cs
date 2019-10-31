using KWI.Format.Structure.BackgroundFrame;
using KWI.Format.Structure.Base;
using KWI.Format.Structure.RoadsFrame;
using KWI.Format.Typing;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Parcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    /// <summary>
    /// 6.3.1.1 Main Map Parcel Management Record
    /// </summary>
    public class ParcelRecord : RecordBase
    {
        private string _name = "Parcel";
        private readonly int _number;
        private readonly int _parcelListType;

        private bool _wrapper = false;

        private long _mapDataFrameAddress = 0;
        private uint _mapDataFrameSize = 0;
        private uint _mapBasicDataFrameSize = 0;
        private uint _mapRoadDataFrameSize = 0;

        public int Width { get; set; }
        public int Height { get; set; }

        public bool DividedWrapper => _wrapper;
        public bool IsPartOfDivided { get; set; }
        public bool IsPartOfIntegrated { get; set; }
        public int PartX { get; set; }
        public int PartY { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Index => _number;

        public GeoCord ParcelLong { get; set; }
        public GeoCord ParcelLat { get; set; }

        public ParcelRecord(int number, int parcelListType, FrameBase frame) : base(frame)
        {
            _number = number;
            _parcelListType = parcelListType;
        }

        public override string Name => $"{_name} {_number}";

        public override bool HasChilds => true;

        protected override void ReadInternal(BinaryReader br)
        {
            var parcelDSA = CreateField<DSA>("First Address of Main Map Data Frame", br);
            var frameSize = CreateField<BS>("Size of Main Map Data Frame (parcel)", br);

            if (_parcelListType == 1)
                _mapBasicDataFrameSize = CreateField<BS>("Size 2 of Main Map Data Frame (basic data frame)", br).Size;
            if (_parcelListType == 2)
                _mapRoadDataFrameSize = CreateField<BS>("Size 2 of Main Map Data Frame (Size of Road Data Only)", br).Size;

            if (_parcelListType == 100)
                CreateField<C>("File Name of Main Map Data Frame", br, 12);

            if (frameSize.Size == 0 && !parcelDSA.IsNull)
            {
                _fields.Clear();
                br.BaseStream.Position = Offset;
                var offset = CreateField<D>("Offset to divided parent record", br, 4);

                _name = "Divided parcel wrapper";
                var parentParcelInformation = FindParentOfType<ParcelInformationRecord>();
                br.BaseStream.Position = parentParcelInformation.Offset + offset.DValue;
                AddRecord(new ParcelInformationRecord(_frame), br);
                br.BaseStream.Position = Offset + 6;

                _wrapper = true;
            }
            else if (parcelDSA.IsNull)
                Hidden = true;

            _mapDataFrameAddress = parcelDSA.ComputedAddress;
            _mapDataFrameSize = frameSize.Size;
        }
        
        protected override void LoadChildsInternal()
        {
            if (_wrapper) return;

            using var file = _frame.OpenAt(_mapDataFrameAddress, asAbsoluteOffset: true);
            using (var br = new BinaryReader(file))
            {
                CreateField<SWS>("Header size", br);
                var pid = CreateField<PID>("Lower Left Reference Parcel ID Number", br);
                ParcelLong = pid.Longitude;
                ParcelLat = pid.Lattitude;
                var pos = CreateField<ParcelLocation>("Lower Left Reference Parcel Location Code within the Block", br);
                X = pos.XPos;
                Y = pos.YPos;
                var divideType = CreateField<ParcelCombination>("Divided/Integrated Parcel Identifier", br);
                IsPartOfDivided = divideType.Divided;
                IsPartOfIntegrated = divideType.Integrated;
                if (IsPartOfDivided || IsPartOfIntegrated)
                {
                    PartX = divideType.CombinationXInfo;
                    PartY = divideType.CombinationYInfo;
                }
                CreateField<PracticalManagement>("Practical Management Code", br);
                CreateField<DataSourceScale>("Data Source Flag", br);
                Height = CreateField<RealLengthData>("Real-length Data in Normalized Longitudinal (X-axis) Direction", br).Value;
                Width = CreateField<RealLengthData>("Real-length Data in Normalized Latitudinal (Y-axis) Direction", br).Value;
                CreateField<I>("Geomagnetic Strength Data", br);
                CreateField<I>("Geomagnetic Declination Data", br);
                CreateField<DSA>("Offset to Route Guidance Data Frame", br);
                CreateField<BS>("Size of Route Guidance Data Frame", br);
                var routeRegionsCount = CreateField<N>("Number of Regions used for Route Planning Data", br).Value;
                for (int i = 0; i < routeRegionsCount; i++)
                {
                    CreateField<RegionNumber>($"Region number {i}", br);
                }

                var parentLevel = FindParentOfType<LevelRecord>();
                DataFrameInfo roadsDataFrame = null;
                DataFrameInfo backgroundDataFrame = null;
                DataFrameInfo nameDataFrame = null;
                if (parentLevel.BasicDataFrameCount > 0)
                    roadsDataFrame = CreateField<DataFrameInfo>("Road Data Frame", br);
                if (parentLevel.BasicDataFrameCount > 1)
                    backgroundDataFrame = CreateField<DataFrameInfo>("Background Data Frame", br);
                if (parentLevel.BasicDataFrameCount > 2)
                    nameDataFrame = CreateField<DataFrameInfo>("Name Data Frame", br);

                for (int i = 0; i < parentLevel.ExtendedDataFrameCount; i++)
                {
                    CreateField<DataFrameInfo>($"Extended data frame {i}", br);
                }

                for (int i = 0; i < 8; i++)
                {
                    var adjacentField = CreateField<AdjacentParcelInfo>(((AdjacentPosition)i).ToString(), br);
                    if (adjacentField.ParcelLocation == null)
                    {
                        _fields.RemoveAt(_fields.Count - 1);
                        var currentPosition = br.BaseStream.Position;
                        br.BaseStream.Position = _mapDataFrameAddress + adjacentField.OffsetToDividedInfo.DValue;
                        for (int j = 0; j < adjacentField.DividedParcelInfo.AdjacentCount; j++)
                        {
                            CreateField<AdjacentParcelInfo>(((AdjacentPosition)i).ToString() + " " + j, br);
                        }
                        br.BaseStream.Position = currentPosition;
                    }
                }

                if (!backgroundDataFrame.Size.IsNull && backgroundDataFrame.Size.SWSValue > 0)
                {
                    br.BaseStream.Position = _mapDataFrameAddress + backgroundDataFrame.Displacement.DValue;
                    AddRecord(new BackgroundFrameRecord(_frame), br);
                }
                if (!roadsDataFrame.Size.IsNull && roadsDataFrame.Size.SWSValue > 0)
                {
                    br.BaseStream.Position = _mapDataFrameAddress + roadsDataFrame.Displacement.DValue;
                    AddRecord(new RoadsFrameRecord(_frame), br);
                }
            };
        }

    }
}
