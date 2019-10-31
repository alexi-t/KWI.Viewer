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

        public long Offset { get; private set; }
        public string Name => Frame.Name;

        public bool HasChilds => Frame.HasChilds;

        public ObservableCollection<INode> Childs => Frame.Childs;

        public ObservableCollection<FieldBase> Fields => Frame.Fields;

        public bool Hidden => false;

        public T Frame { get; } = new T() { };

        public override void Read(BinaryReader br, int length = 0)
        {
            Offset = br.BaseStream.Position;
            var dsa = new Field<DSA>("Address of the Management Header");
            dsa.Read(br);
            var size = new Field<BS>("Size of the Management Header");
            size.Read(br);
            var fileName = new Field<C>("Management File Name", 12);
            fileName.Read(br);

            Frame.Fields.Add(dsa);
            Frame.Fields.Add(size);
            Frame.Fields.Add(fileName);

            var file = fileName.Value.Str;

            if (string.IsNullOrEmpty(file))
                file = "ALLDATA.KWI";

            using (var stream = File.OpenRead($"{KWIContext.DriveLetter}:\\{file}"))
            using (var reader = new BinaryReader(stream))
            {
                stream.Position = dsa.Value.ComputedAddress;
                Frame.Read(reader);
            }
        }
    }
}
