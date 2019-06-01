using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Typing.Base
{
    public abstract class BinarySerializable
    {
        public bool IsNull { get; protected set; }

        protected DateTime ReadDate(BinaryReader br)
        {
            var days = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray());
            return new DateTime(1997, 1, 1).AddDays(days);
        }

        public virtual void Read(BinaryReader br, int length = 0)
        {

        }
    }
}
