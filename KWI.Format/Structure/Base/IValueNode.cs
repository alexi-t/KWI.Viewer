using System;
using System.Collections.Generic;
using System.Text;

namespace KWI.Format.Structure.Base
{
    public class ValueNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IEnumerable<ValueNode> Childs { get; set; }
    }
}
