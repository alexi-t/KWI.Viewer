using System;
using System.Collections.Generic;
using System.Text;

namespace KWI.Format.Structure.Base
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ValueNameAttribute : Attribute
    {
        public ValueNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
