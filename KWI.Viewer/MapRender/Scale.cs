using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace KWI.Viewer.MapRender
{
    public class Scale
    {
        public string Name { get; }
        public int ParcelSize { get; set; }
        public int LevelCode { get; set; }
        public string LevelName { get; set; }

        public Scale(string name, int parcelSize, int levelCode, string levelName)
        {
            Name = name;
            ParcelSize = parcelSize;
            LevelCode = levelCode;
            LevelName = levelName;
        }
    }
}
