using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace ProductionSim
{	
	internal static class SimulationDeserializer
	{
		
		public static ISet<IBlock> ReadXml(XmlReader reader)
		{
			var xml = XDocument.Load(reader);
			
			var partsRec = ReadParts(xml);
			var blocksRec = ReadBlocks(xml);
			var buffersRec = ReadBuffers(xml);
			
			var parts = new Dictionary<string, Part>();
			CreateParts(partsRec.Values, partsRec, parts);
			
			var buffers = CreateBuffers(buffersRec.Values);
			
			return CreateBlocks(blocksRec.Values, parts, buffers).Values.ToHashSet<IBlock>();
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
				        						    .Select(e2 => e2.Attribute("Name").Value),
				        						   e.Attribute("InputBuffer").Value,
				        						   e.Attribute("OutputBuffer").Value)
				      ).ToDictionary(a => a.Name);
		}
		
		private static Dictionary<string, BufferRecord> ReadBuffers(XDocument xml)
		{
			return xml.Root
					  .Element("Buffers")
					  .Elements("Buffer")
				  	  .Select(e => new BufferRecord(e.Attribute("Name").Value, 
				                              		int.Parse(e.Attribute("Capacity").Value)) 
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
		
		private static Dictionary<string, Block> CreateBlocks(IEnumerable<BlockRecord> src, IDictionary<string, Part> parts, IDictionary<string, Buffer> buffers)
		{
			return src.Select(br => new Block(br.Name, 
			                                  br.ProducesParts.Select(s => parts[s]),
			                                  buffers[br.InputBuffer],
			                                  buffers[br.OutputBuffer]))
					  .ToDictionary(b => b.Name);
		}
		
		private static Dictionary<string, Buffer> CreateBuffers(IEnumerable<BufferRecord> src)
		{
			return src.Select(br => new Buffer(br.Name, br.Capacity))
					  .ToDictionary(b => b.Name);
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
	        public readonly string InputBuffer, OutputBuffer;

	        public BlockRecord(string name, IEnumerable<string> producesParts, string inputBuffer, string outputBuffer)
	        {
	            Name = name;
	            ProducesParts = producesParts;
	            InputBuffer = inputBuffer;
	            OutputBuffer = outputBuffer;
	        }
	    }

	    private class BufferRecord
	    {
	        public readonly string Name;
	        public readonly int Capacity;
	       
	        public BufferRecord(string name, int capacity)
	        {
	            Name = name;
	            Capacity = capacity;
	        }
	    }
	}
	
}