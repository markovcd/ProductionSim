using System;
using System.Linq;
using System.Xml;

namespace ProductionSim
{
	internal static class SimulationSerializer
	{
		public static void WriteXml(Simulation simulation, XmlWriter writer)
		{		
			writer.WriteStartElement("Parts");
			
			foreach (var p in simulation.Parts) PartWriteXml(p as Part, writer);
			
			writer.WriteEndElement();
			
			writer.WriteStartElement("Blocks");
			
			foreach (var b in simulation.Blocks) BlockWriteXml(b as Block, writer);
			
			writer.WriteEndElement();
			
			writer.WriteStartElement("Buffers");
			
			foreach (var b in simulation.Buffers) BufferWriteXml(b as Buffer, writer);
			
			writer.WriteEndElement();		
			
		}
		
		private static void BlockWriteXml(Block b, XmlWriter writer)
		{
			if (b == null) return;
			
			writer.WriteStartElement("Block");
			
			writer.WriteAttributeString("Name", b.Name);
			writer.WriteAttributeString("InputBuffer", b.InputBuffer.Name);
			writer.WriteAttributeString("OutputBuffer", b.OutputBuffer.Name);
			
			writer.WriteStartElement("ProducesParts");
			
			foreach (var part in b.ProducesParts) 
			{
				writer.WriteStartElement("Part");
				writer.WriteAttributeString("Name", part.Name);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		
		private static void BufferWriteXml(Buffer b, XmlWriter writer)
		{
		    if (b == null) return;

            writer.WriteStartElement("Buffer");
			
			writer.WriteAttributeString("Name", b.Name);
			writer.WriteAttributeString("Capacity", b.Capacity.ToString());

			writer.WriteEndElement();
		}

		private static void PartWriteXml(Part p, XmlWriter writer)
		{
			if (p == null) return;
			
			writer.WriteStartElement("Part");
			
			writer.WriteAttributeString("Name", p.Name);
			writer.WriteAttributeString("ManufactureTime", p.ManufactureTime.ToString());
			
			writer.WriteStartElement("MadeFrom");
			
			foreach (var part in p.MadeFrom) 
			{
				writer.WriteStartElement("Part");
				writer.WriteAttributeString("Name", part.Name);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
}