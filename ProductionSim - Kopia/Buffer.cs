using System;
using System.Collections;
using System.Collections.Generic;

namespace ProductionSim
{
    public class Buffer : IBuffer
    {
        private readonly List<IPart> _parts;

        public string Name { get; }

        public int Capacity { get; }

        public bool Full => Capacity == _parts.Count;

        public Buffer(string name, int capacity)
        {
        	Name = name;
        	_parts = new List<IPart>(capacity);
            Capacity = capacity;
        }

        public override string ToString()
        {
            return Name;
        }

        #region ICollection implementation

        public int Count => _parts.Count;
        bool ICollection<IPart>.IsReadOnly => ((ICollection<IPart>)_parts).IsReadOnly;

        public void Add(IPart part)
        {
        	if (Full) throw new InvalidOperationException($"Can't add part {part}. Buffer {this} is full.");
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