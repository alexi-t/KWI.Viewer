using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace KWI.Format.Structure.Base
{
    public abstract class RecordBase : INode
    {
        protected readonly ObservableCollection<FieldBase> _fields = new ObservableCollection<FieldBase>();
        protected readonly ObservableCollection<INode> _childs = new ObservableCollection<INode>();

        protected RecordBase _parent;

        protected readonly FrameBase _frame;

        public RecordBase(FrameBase frame)
        {
            _frame = frame;
        }

        public T FindParentOfType<T>() where T : RecordBase
        {
            var pointer = this._parent;
            while (pointer != null)
            {
                if (pointer is T)
                    return (T)pointer;
                pointer = pointer._parent;
            }
            return null;
        }
        public abstract string Name { get; }

        public abstract bool HasChilds { get; }

        public ObservableCollection<INode> Childs => _childs;

        public ObservableCollection<FieldBase> Fields => _fields;

        public long Offset { get; protected set; }

        public bool Hidden { get; protected set; }

        protected abstract void ReadInternal(BinaryReader br);

        public RecordBase Read(BinaryReader br)
        {
            Offset = br.BaseStream.Position;
            ReadInternal(br);
            return this;
        }

        protected T CreateField<T>(string name, BinaryReader br, int lenght = -1) where T : BinarySerializable, new()
        {
            var field = new Field<T>(name, lenght);
            field.Read(br);
            _fields.Add(field);
            return field.Value;
        }

        protected RecordBase AddRecord(RecordBase record, BinaryReader binaryReader)
        {
            record._parent = this;
            record.Read(binaryReader);
            if (!record.Hidden)
                _childs.Add(record);
            return record;
        }

        private bool _childsLoaded = false;
        protected virtual void LoadChildsInternal() { }
        public void LoadChilds() {
            if (!_childsLoaded)
            {
                LoadChildsInternal();
                _childsLoaded = true;
            }
        }
    }
}
