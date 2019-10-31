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

        public INode Read()
        {
            using (var allDataFile = File.OpenRead($"{KWIContext.DriveLetter}:\\ALLDATA.KWI"))
            using (var br = new BinaryReader(allDataFile))
            {
                var allDataFrame = new AllDataFrame();
                allDataFrame.Read(br);
                return allDataFrame;
            }
        }
    }
}
