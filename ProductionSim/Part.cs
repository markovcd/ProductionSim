using System;
using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{
	
	public class Part : IPart, IEquatable<Part>
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
		        var hashCode = part.MadeFrom.Any() ? part.MadeFrom.Aggregate(1, (current, p) => 31 * current + p.GetHashCode()) : 0;
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
	}
}