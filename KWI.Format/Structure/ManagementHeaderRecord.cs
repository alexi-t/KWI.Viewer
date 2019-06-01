using KWI.Format.Structure.Base;
using KWI.Format.Typing;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Structure
{
    public class ManagementHeaderRecord<T> : BinarySerializable, INode where T : FrameBase, new()
    {
        private readonly List<FieldBase> _fields = new List<FieldBase>();
        private readonly T _frame = new T() { DriveLetter = 'J' };

        public long Offset { get; private set; }
        public string Name => _frame.Name;

        public bool HasChilds => _frame.HasChilds;

        public ObservableCollection<INode> Childs => _frame.Childs;

        public ObservableCollection<FieldBase> Fields => _frame.Fields;

        public bool Hidden => false;

        public T Frame => _frame;

        public override void Read(BinaryReader br, int length = 0)
        {
            Offset = br.BaseStream.Position;
            var dsa = new Field<DSA>("Address of the Management Header");
            dsa.Read(br);
            var size = new Field<BS>("Size of the Management Header");
            size.Read(br);
            var fileName = new Field<C>("Management File Name", 12);
            fileName.Read(br);

            _frame.Fields.Add(dsa);
            _frame.Fields.Add(size);
            _frame.Fields.Add(fileName);

            var file = fileName.Value.Str;

            if (string.IsNullOrEmpty(file))
                file = "ALLDATA.KWI";

            using (var stream = File.OpenRead($"J:\\{file}"))
            using (var reader = new BinaryReader(stream))
            {
                stream.Position = dsa.Value.ComputedAddress;
                _frame.Read(reader);
            }
        }
    }
}
