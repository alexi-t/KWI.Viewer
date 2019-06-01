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
    /// <summary>
    /// 5. All Data Management Frame
    /// </summary>
    public class AllDataFrame : BinarySerializable, INode
    {
        private ObservableCollection<FieldBase> _fields = new ObservableCollection<FieldBase>();
        private ObservableCollection<INode> _childs = new ObservableCollection<INode>();

        public INode Parent => null;

        public string Name => "All Data";

        public bool HasChilds => Childs?.Any() ?? false;

        public ObservableCollection<INode> Childs => _childs;

        public ObservableCollection<FieldBase> Fields => _fields;

        public long Offset => 0;

        public bool Hidden => false;

        private void ReadDataVolume(BinaryReader br)
        {
            var fields = new FieldBase[]
            {
                new Field<MidC>("System-specific Identification", 64),
                new Field<MidC>("Data author Identification", 64),
                new Field<MidC>("System Identification", 32),
                new Field<C>("Format Version Number", 64),
                new Field<C>("Data Version Number", 64),
                new Field<C>("Disk Title", 128),
                new Field<Bytes>("Data Contents", 8),
                new Field<C>("Media Version Number", 32),
                new Field<PID>("Lower Left Latitude and Longitude of the Maximum Coverage Area", 8),
                new Field<PID>("Upper Right Latitude and Longitude of the Maximum Coverage Area", 8),
                new Field<N>("Logical Sector Size", 2),
                new Field<N>("Sector Size", 2),
                new Field<Bytes>("Background data", 2),
                new Field<Bytes>("Reserved", 14)
            };

            foreach (var field in fields)
            {
                field.Read(br);
                _fields.Add(field);
            }
        }

        public override void Read(BinaryReader br, int length = -1)
        {
            ReadDataVolume(br);
            ReadLevelManagement(br);
            ReadManagementHeaders(br);
            base.Read(br);
        }

        private T AddFrame<T>(BinaryReader br) where T : BinarySerializable, INode, new()
        {
            var t = new T();
            t.Read(br);
            return t;
        }

        private void ReadManagementHeaders(BinaryReader br)
        {
            br.BaseStream.Position = 2048;
            _childs.Add(AddFrame<ManagementHeaderRecord<ParcelRelatedDataFrame>>(br));
        }

        private void ReadLevelManagement(BinaryReader br)
        {
            var count = new N();
            count.Read(br);
            for (int i = 0; i < count.Value; i++)
            {
                var type = br.ReadByte();
                var levelsCount = br.ReadByte();

                for (int l = 0; l < levelsCount; l++)
                {
                    var levelNumber = br.ReadByte();
                    br.ReadByte();
                    var recordCount = new N();
                    recordCount.Read(br);
                }
            }

        }
    }
}
