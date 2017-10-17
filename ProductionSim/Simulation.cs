using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductionSim
{

	public class Simulation : Loggable, ISimulation, IXmlSerializable
	{
	    private ISet<IBlock> _blocks;
	    
        public int Ticks { get; protected set; }
	    public int IdleTicks { get; protected set; }
        
	    public IEnumerable<IBlock> Blocks { get { return _blocks; } }
	    
        public IEnumerable<IBuffer> Buffers 
        { 
        	get 
        	{ 
        		return _blocks.Select(b => b.InputBuffer)
        				  	  .Concat(_blocks.Select(b => b.OutputBuffer))
        				  	  .ToHashSet(); 
        	} 
        }
        
		public IEnumerable<IPart> Parts 
		{ 
			get 
			{ 
				return _blocks.SelectMany(b => b.ProducesParts)
							  .Concat(_blocks.SelectMany(b => b.UsesParts))
							  .ToHashSet();
			}
		}

	    public Simulation(IEnumerable<IBlock> blocks = null, ILogger logger = null)
		{
	    	_blocks = (blocks ?? Enumerable.Empty<IBlock>()).ToHashSet();
	    	Logger = logger;
	    	Log("Created.");
		}   

	    public void Tick()
	    {
	        Ticks++;
	        Log("Tick {0}.", Ticks);
	        
	        foreach (var block in _blocks) 
	        {
	        	var idle = block.IdleTicks;
	        	block.Tick();
	        	IdleTicks = IdleTicks + block.IdleTicks - idle;
	        }
	    }

        public void ResetState()
        {
            Ticks = 0;
            IdleTicks = 0;
            
            foreach (var block in _blocks) block.ResetState();
            foreach (var buffer in Buffers) buffer.Clear();
            
            Log("Resetting state.");
            	
        }
        
        #region IXmlSerializable implementation

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			var xml = XDocument.Load(reader);
			
			var partsRec = ReadParts(xml);
			var blocksRec = ReadBlocks(xml);
			var buffersRec = ReadBuffers(xml);
			
			var parts = new Dictionary<string, Part>();
			CreateParts(partsRec.Values, partsRec, parts);
			var blocks = CreateBlocks(blocksRec.Values, parts);
			var buffers = CreateBuffers(buffersRec.Values, blocks);
			
			_blocks = blocks.Values.ToHashSet<IBlock>();
		}
		
		private static Dictionary<string, PartRecord> ReadParts(XDocument xml)
		{
			return xml.Root
					  .Element("Parts")
					  .Elements("Part")
					  .Select(e => new PartRecord(e.Attribute("Name").Value, 
				        						  int.Parse(e.Attribute("ManufactureTime").Value),
				        						  e.Element("MadeFrom")
				        						   .Elements("Part")
				        						   .Select(e2 => e2.Attribute("Name").Value))
				        	 ).ToDictionary(a => a.Name);
		}
		
		private static Dictionary<string, BlockRecord> ReadBlocks(XDocument xml)
		{
			return xml.Root
					  .Element("Blocks")
					  .Elements("Block")
					  .Select(e => new BlockRecord(e.Attribute("Name").Value,
				        						   e.Element("ProducesParts")
				        						    .Elements("Part")
				        						    .Select(e2 => e2.Attribute("Name").Value))			        						 
				      ).ToDictionary(a => a.Name);
		}
		
		private static Dictionary<string, BufferRecord> ReadBuffers(XDocument xml)
		{
			return xml.Root
					  .Element("Buffers")
					  .Elements("Buffer")
				  	  .Select(e => new BufferRecord(e.Attribute("Name").Value, 
				                              		int.Parse(e.Attribute("Capacity").Value),
				        						    e.Element("SourceBlocks")
				        						  	 .Elements("Block")
				        						   	 .Select(e2 => e2.Attribute("Name").Value),
				        						   	e.Element("TargetBlocks")
				        						  	 .Elements("Block")
				        						  	 .Select(e2 => e2.Attribute("Name").Value)) 
				      ).ToDictionary(a => a.Name);
		}
		
		private static void CreateParts(IEnumerable<PartRecord> current, IDictionary<string, PartRecord> src, Dictionary<string, Part> dest)
		{
			foreach (var rec in current) 
			{
				Part part;
			    if (dest.TryGetValue(rec.Name, out part)) continue;

			    CreateParts(rec.MadeFrom.Select(s => src[s]), src, dest);
					
			    part = new Part(rec.Name, rec.ManufactureTime, rec.MadeFrom.Select(s => dest[s]));
			    dest.Add(part.Name, part);
			}
		}
		
		private static Dictionary<string, Block> CreateBlocks(IEnumerable<BlockRecord> src, IDictionary<string, Part> parts)
		{
			return src.Select(br => new Block(br.Name, 
			                                  br.ProducesParts.Select(s => parts[s])))
					  .ToDictionary(b => b.Name);
		}
		
		private static Dictionary<string, Buffer> CreateBuffers(IEnumerable<BufferRecord> src, IDictionary<string, Block> blocks)
		{
			return src.Select(br => new Buffer(br.Name, br.Capacity,
			                                   br.SourceBlocks.Select(s => blocks[s]), 
			                                   br.TargetBlocks.Select(s => blocks[s])))
					  .ToDictionary(b => b.Name);
		}
		
		

		public void WriteXml(XmlWriter writer)
		{		
			writer.WriteStartDocument();
			
			writer.WriteStartElement("Parts");
			
			foreach (var p in Parts) Part.WriteXml(p as Part, writer);
			
			writer.WriteEndElement();
			
			writer.WriteStartElement("Blocks");
			
			foreach (var b in Blocks) Block.WriteXml(b as Block, writer);
			
			writer.WriteEndElement();
			
			writer.WriteStartElement("Buffers");
			
			foreach (var b in Buffers) Buffer.WriteXml(b as Buffer, writer);
			
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}

	    private class PartRecord
	    {
	        public readonly string Name;
	        public readonly int ManufactureTime;
	        public readonly IEnumerable<string> MadeFrom;

	        public PartRecord(string name, int manufactureTime, IEnumerable<string> madeFrom)
	        {
	            Name = name;
	            ManufactureTime = manufactureTime;
	            MadeFrom = madeFrom;
	        }
	    }

	    private class BlockRecord
	    {
	        public readonly string Name;
	        public readonly IEnumerable<string> ProducesParts;

	        public BlockRecord(string name, IEnumerable<string> producesParts)
	        {
	            Name = name;
	            ProducesParts = producesParts;
	        }
	    }

	    private class BufferRecord
	    {
	        public readonly string Name;
	        public readonly int Capacity;
	        public readonly IEnumerable<string> SourceBlocks;
	        public readonly IEnumerable<string> TargetBlocks;

	        public BufferRecord(string name, int capacity, IEnumerable<string> sourceBlocks, IEnumerable<string> targetBlocks)
	        {
	            Name = name;
	            Capacity = capacity;
	            SourceBlocks = sourceBlocks;
	            TargetBlocks = targetBlocks;
	        }
	    }

        #endregion

    }
}
