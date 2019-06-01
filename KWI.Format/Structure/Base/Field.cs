using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KWI.Format.Structure.Base
{
    public class Field<T> : FieldBase where T : BinarySerializable, new()
    {
        private readonly int _length = -1;

        public Field(string name, int length = -1)
        {
            Name = name;
            _length = length;
        }

        public string Name { get; }
        public T Value { get; set; }

        public override void Read(BinaryReader br, int length = -1)
        {
            var value = new T();
            value.Read(br, length > 0 ? length : _length);
            Value = value;
        }

        protected override object GetValue() => Value;
    }
}
