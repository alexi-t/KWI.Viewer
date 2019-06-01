using KWI.Format.Structure.Base;
using KWI.Format.Typing;
using KWI.Format.Typing.Base;
using KWI.Format.Typing.Block;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure
{
    public class BlockRecord : RecordBase
    {
        private uint _parcelInfoAddress;
        private uint _parcelInfoSize;

        public int Index { get; }

        public BlockRecord(int number, FrameBase frame) : base(frame)
        {
            Index = number;
        }

        public override string Name => $"Block {Index}";

        public override bool HasChilds => true;

        protected override void ReadInternal(BinaryReader br)
        {
            var parcelDSA = CreateField<DSA>("Address of Parcel Management Information", br);
            _parcelInfoAddress = parcelDSA.ComputedAddress;
            _parcelInfoSize = CreateField<BS>("Size of Parcel Management Information", br).Size;

            if (!parcelDSA.IsNull)
                ReadParcelInformation();
            else
                Hidden = true;
        }

        private void ReadParcelInformation()
        {
            using (var file = _frame.OpenAt(_parcelInfoAddress, asAbsoluteOffset: true))
            using (var br = new BinaryReader(file))
            {
                AddRecord(new ParcelInformationRecord(_frame), br);
            }
        }
    }
}
