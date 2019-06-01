using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.BackgroundFrame
{
    public class BackgroundFrameRecord : FrameBase
    {
        private bool _hasChilds = false;
        private readonly string _fileName = string.Empty;

        public BackgroundFrameRecord(FrameBase parentFrame)
        {
            _fileName = parentFrame.FileName;
        }

        public override string Name => "Background";

        public override bool HasChilds => _hasChilds;

        public override string FileName => _fileName;

        protected override void ReadInternal(BinaryReader br)
        {
            CreateField<SWS>("Size of Background Distribution Header", br);
            var parentLevel = FindParentOfType<LevelRecord>();
            var bgDisplayClassCount = parentLevel.BgDisplayClassCount;
            for (int i = 0; i < bgDisplayClassCount; i++)
            {
                AddRecord(new BgElementInformation(i, this), br);
            }
        }
    }
}
