using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ProductionSim
{
    public class Buffer : IBuffer, IXmlSerializable
    {
        private readonly string _name;
        private readonly List<IPart> _parts;
        private readonly int _capacity;
        private readonly ISet<IBlock> _sourceBlocks;
        private readonly ISet<IBlock> _targetBlocks;

        public IEnumerable<IBlock> SourceBlocks { get { return _sourceBlocks; } }
        public IEnumerable<IBlock> TargetBlocks { get { return _targetBlocks; } }

        public string Name { get { return _name; } }

        public int Capacity { get { return _capacity; } }

        public bool Full { get { return _capacity == _parts.Count; } }

        public Buffer(string name, int capacity, IEnumerable<IBlock> sourceBlocks = null, IEnumerable<IBlock> targetBlocks = null)
        {
        	_name = name;
        	_parts = new List<IPart>(capacity);
            _capacity = capacity;

            _sourceBlocks = new HashSet<IBlock>();
            _targetBlocks = new HashSet<IBlock>();
            
            if (sourceBlocks != null) foreach (var block in sourceBlocks) AddSourceBlock(block);      
            if (targetBlocks != null) foreach (var block in targetBlocks) AddTargetBlock(block);
        }
        
        public void AddSourceBlock(IBlock block)
        {
            _sourceBlocks.Add(block);
            block.OutputBuffer = this;
        }

        public void RemoveSourceBlock(IBlock block)
        {
            if (!_sourceBlocks.Remove(block)) throw new InvalidOperationException(string.Format("Block {0} not present in source block list of buffer {1}.", block, this));
            block.OutputBuffer = null;
        }

        public void AddTargetBlock(IBlock block)
        {
            _targetBlocks.Add(block);
            block.InputBuffer = this;
        }

        public void RemoveTargetBlock(IBlock block)
        {
            if (!_targetBlocks.Remove(block)) throw new InvalidOperationException(string.Format("Block {0} not present in target block list of buffer {1}.", block, this));
            block.InputBuffer = null;
        }

        public void MakePart(IPart part, IBuffer input)
        {
        	if (Full) throw new InvalidOperationException(string.Format("Can't make part {0}. Buffer {1} full.", part, this));
            if (!input.CanMadePart(part)) throw new InvalidOperationException(string.Format("Can't make part {0}. Not all required input parts present in buffer {1}.", part, input));
            
            foreach (var madeFrom in part.MadeFrom) input.Remove(madeFrom);

            Add(part);
        }

        public bool CanMadePart(IPart part)
        {
            var inputBuffer = this.ToList();
            return part.MadeFrom.All(inputBuffer.Remove);
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
        
        #region IXmlSerializable implementation

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public void WriteXml(XmlWriter writer)
		{		
			WriteXml(this, writer);
		}
		
		public static void WriteXml(Buffer b, XmlWriter writer)
		{
			writer.WriteStartElement("Block");
			
			writer.WriteAttributeString("Name", b.Name);
			writer.WriteAttributeString("Capacity", b.Capacity.ToString());
			
			writer.WriteStartElement("SourceBlocks");
			
			foreach (var block in b.SourceBlocks) 
			{
				writer.WriteStartElement("Block");
				writer.WriteAttributeString("Name", block.Name);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
			
			writer.WriteStartElement("TargetBlocks");
			
			foreach (var block in b.TargetBlocks) 
			{
				writer.WriteStartElement("Block");
				writer.WriteAttributeString("Name", block.Name);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
			
			writer.WriteEndElement();
		}

		#endregion
    }
}