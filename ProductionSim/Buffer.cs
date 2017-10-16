using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{
    public class Buffer : IBuffer
    {
        private readonly string _name;
        private readonly List<IPart> _parts;
        private int _capacity;
        private readonly List<IBlock> _sourceBlocks;
        private readonly List<IBlock> _targetBlocks;

        public IEnumerable<IBlock> SourceBlocks { get { return _sourceBlocks; } }
        public IEnumerable<IBlock> TargetBlocks { get { return _targetBlocks; } }

        public string Name { get { return _name; } }

        public void AddSourceBlock(IBlock block)
        {
            _sourceBlocks.Add(block);
        }

        public void RemoveSourceBlock(IBlock block)
        {
            if (!_sourceBlocks.Remove(block)) throw new InvalidOperationException(string.Format("Block {0} not present in source block list of buffer {1}.", block, this));
        }

        public void AddTargetBlock(IBlock block)
        {
            _targetBlocks.Add(block);
        }

        public void RemoveTargetBlock(IBlock block)
        {
            if (!_targetBlocks.Remove(block)) throw new InvalidOperationException(string.Format("Block {0} not present in target block list of buffer {1}.", block, this));
        }

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                if (value < _parts.Count) throw new InvalidOperationException("Capacity less than count. Clear list first and then set this value.");
                _parts.Capacity = value;
                _capacity = value;
            }
        }

        public bool Full { get { return _capacity == _parts.Count; } }

        public Buffer(int capacity)
        {
            _parts = new List<IPart>(capacity);
            _capacity = capacity;

            _sourceBlocks = new List<IBlock>();
            _targetBlocks = new List<IBlock>();
        }

        public Buffer(int capacity, IEnumerable<IBlock> sourceBlocks, IEnumerable<IBlock> targetBlocks) : this(capacity)
        {
            _sourceBlocks = sourceBlocks.ToList();
            _targetBlocks = targetBlocks.ToList();
        }

        public void MakePart(IPart part, IBuffer input)
        {
            if (Full) throw new InvalidOperationException("Can't make this part. Buffer full.");
            if (!input.CanMadePart(part)) throw new InvalidOperationException("Can't make this part. Not all required input parts present.");
            foreach (var madeFrom in part.MadeFrom) input.Remove(madeFrom);

            Add(part);
        }

        public bool CanMadePart(IPart part)
        {
            var inputBuffer = this.ToList();
            return part.MadeFrom.All(p => inputBuffer.Remove(p));
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
            if (Full) throw new InvalidOperationException("Can't add item. Buffer is full.");
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