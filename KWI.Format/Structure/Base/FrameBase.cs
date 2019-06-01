using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.Base
{
    public abstract class FrameBase : RecordBase
    {
        public char DriveLetter { get; set; } = 'J';
        public FrameBase() : base(null) { }

        public abstract string FileName { get; }

        public FileStream OpenAt(long position, bool asAbsoluteOffset = false)
        {
            var file = File.OpenRead($"{DriveLetter}:\\{FileName}");
            file.Position = (asAbsoluteOffset ? 0 : Offset) + position;
            return file;
        }
    }
}
