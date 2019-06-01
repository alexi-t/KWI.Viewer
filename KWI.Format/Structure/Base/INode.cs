using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace KWI.Format.Structure.Base
{
    public interface INode
    {
        long Offset { get; }
        string Name { get; }
        bool HasChilds { get; }
        bool Hidden { get; }
        ObservableCollection<INode> Childs { get; }
        ObservableCollection<FieldBase> Fields { get; }
    }
}
