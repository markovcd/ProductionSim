using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ProductionSim
{
    public class Buffer : IBuffer
    {
        private readonly string _name;
        private readonly List<IPart> _parts;
        private readonly int _capacity;

        public string Name { get { return _name; } }

        public int Capacity { get { return _capacity; } }

        public bool Full { get { return _capacity == _parts.Count; } }

        public Buffer(string name, int capacity)
        {
        	_name = name;
        	_parts = new List<IPart>(capacity);
            _capacity = capacity;
        }

        public override string ToString()
        {
            return _name;
        }

        #region ICollection implementation

        public int Count { get { return _parts.Count; } }
        bool ICollection<IPart>.IsReadOnly { get { return ((ICollection<IPart>)_parts).IsReadOnly; } }

        public void Add(IPart part)
        {
        	if (Full) throw new InvalidOperationException(string.Format("Can't add part {0}. Buffer {1} is full.", part, this));
            _parts.Add(part);
        }

        public void Clear()
        {
            _parts.Clear();
        }

        public bool Contains(IPart part)
        {
            return _parts.Contains(part);
        }

        public void CopyTo(IPart[] array, int arrayIndex)
        {
            _parts.CopyTo(array, arrayIndex);
        }

        public bool Remove(IPart item)
        {
            return _parts.Remove(item);
        }

        public IEnumerator<IPart> GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        
        
    }
}