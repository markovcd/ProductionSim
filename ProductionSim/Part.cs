using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ProductionSim
{
	
	public class Part : IPart, IEquatable<Part>, IXmlSerializable
	{
		private readonly int _manufactureTime;
		private readonly string _name;
		private readonly IEnumerable<IPart> _madeFrom;
		
		private readonly int _hashCode;
		
		public int ManufactureTime { get { return _manufactureTime; } }
		public IEnumerable<IPart> MadeFrom { get { return _madeFrom; } }
		public string Name { get { return _name; } }
		
		public Part(string name, int manufactureTime, IEnumerable<IPart> madeFrom)
		{
			_name = name;
			_manufactureTime = manufactureTime;
			_madeFrom = madeFrom;
			
			_hashCode = GetHashCode(this);
		}

	    public override string ToString()
	    {
	        return _name;
	    }

        #region Equals and GetHashCode implementation
        public override int GetHashCode()
		{
			return _hashCode;
		}
		
		protected static int GetHashCode(Part part)
		{
			unchecked
		    {
		        //var hashCode = part.MadeFrom.Any() ? part.MadeFrom.Aggregate(1, (current, p) => 31 * current + p.GetHashCode()) : 0;
		        var hashCode = 1;
		        hashCode += 1000000007 * part._manufactureTime.GetHashCode();
		        hashCode += 1000000009 * part._name.GetHashCode();
		        return hashCode;
            }
		}

		public override bool Equals(object obj)
		{
			var other = obj as Part;
			return other != null && Equals(other);
		}
		
		public bool Equals(Part other)
		{
		    if (other == null) return false;
            return GetHashCode() == other.GetHashCode();
		}

		public static bool operator ==(Part lhs, Part rhs) 
		{
			return lhs != null && lhs.Equals(rhs);
		}

		public static bool operator !=(Part lhs, Part rhs) 
		{
			return !(lhs == rhs);
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
		
		public static void WriteXml(Part p, XmlWriter writer)
		{
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

		#endregion
	}
}