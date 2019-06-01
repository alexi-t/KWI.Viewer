using KWI.Format.Typing.Base;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace KWI.Format.Structure.Base
{
    public abstract class FieldBase : BinarySerializable
    {
        protected abstract object GetValue();

        public IEnumerable<ValueNode> ValueNodes
        {
            get
            {
                var value = GetValue();
                if (value == null)
                    return null;

                return GetValueNodes(value);
            }
        }

        private IEnumerable<ValueNode> GetValueNodes(object obj)
        {
            var list = new List<ValueNode>();

            var props = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var prop in props)
            {
                var valueName = prop.GetCustomAttribute<ValueNameAttribute>();
                if (valueName != null)
                {
                    var propValue = prop.GetValue(obj);
                    var valueNode = new ValueNode
                    {
                        Name = valueName.Name,
                        Value = propValue.ToString(),
                        Childs = propValue is BinarySerializable ? GetValueNodes(propValue) : null
                    };
                    list.Add(valueNode);
                }
            }

            return list;
        }
    }
}
