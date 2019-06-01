using KWI.Format.Structure;
using KWI.Format.Structure.Base;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KWI.Format.Reader
{
    public class DVDReader
    {
        private readonly string _drive;

        public DVDReader(string driveLetter)
        {
            _drive = driveLetter;
        }

        public INode Read()
        {
            using (var allDataFile = File.OpenRead($"{_drive}:\\ALLDATA.KWI"))
            using (var br = new BinaryReader(allDataFile))
            {
                var allDataFrame = new AllDataFrame();
                allDataFrame.Read(br);
                return allDataFrame;
            }
        }
    }
}
