using KWI.Format.Structure.Base;
using KWI.Format.Typing;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    public class RouteGuidanceRecord : RecordBase
    {
        private readonly int _index = 0;
        private readonly int _parcelType = 0;

        private long _guidanceAddress = 0;
        private uint _guidanceDataSize = 0;
        private uint _guidanceBasicDataSize = 0;

        public RouteGuidanceRecord(int index, int parcelType, FrameBase frame) : base(frame)
        {
            _index = index;
            _parcelType = parcelType;
        }

        public override string Name => $"Route Guidance {_index}";

        public override bool HasChilds => true;

        protected override void ReadInternal(BinaryReader br)
        {
            var routeGuidanceDSA = CreateField<DSA>("First Address of Route Guidance Data Frame", br);
            var dataSize = CreateField<BS>("Size of Route Guidance Data Frame (parcel)", br);

            if (dataSize.Size == 0 && !routeGuidanceDSA.IsNull)
            {
                br.BaseStream.Position -= 6;
                _fields.Clear();

                var offset = CreateField<D>("Offset to divided parcel data", br, 4);
                var parentParcelInformation = FindParentOfType<ParcelInformationRecord>();
                br.BaseStream.Position = parentParcelInformation.Offset + offset.DValue;
                AddRecord(new ParcelInformationRecord(_frame), br);
                br.BaseStream.Position = Offset + 6;
            }
            else if (routeGuidanceDSA.IsNull)
                Hidden = true;
            else
            {
                _guidanceAddress = routeGuidanceDSA.ComputedAddress;
                _guidanceDataSize = dataSize.Size;

                if (_parcelType == 1 || _parcelType == 2)
                {
                    _guidanceBasicDataSize = CreateField<BS>("Size of Route Guidance Data Frame (basic data)", br).Size;
                }
                else if (_parcelType == 100)
                {
                    var fileName = CreateField<C>("File Name of Route Guidance Data Frame", br, 12);
                }
            }
        }

        protected override void LoadChildsInternal()
        {
            //using var file = _frame.OpenAt(_guidanceAddress, asAbsoluteOffset: true);
            //using var br = new BinaryReader(file)
            //{
                
            //};
        }
    }
}
