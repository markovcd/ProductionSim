﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{
	
	public class Part : IPart, IEquatable<Part>
	{
	    private readonly int _hashCode;
	    private readonly int _manufactureTime;
	    private readonly IEnumerable<IPart> _madeFrom;
	    private readonly string _name;
	    
	    public int ManufactureTime { get { return _manufactureTime; } }
	    public IEnumerable<IPart> MadeFrom { get { return _madeFrom; } }
	    public string Name { get { return _name; } }

	    public Part(string name, int manufactureTime, IEnumerable<IPart> madeFrom = null)
		{
			_name = name;
			_manufactureTime = manufactureTime;
			_madeFrom = (madeFrom ?? Enumerable.Empty<IPart>()).ToList();
			
			_hashCode = GetHashCode(this);
		}

	    public override string ToString()
	    {
	        return Name;
	    }

        #region Equals and GetHashCode implementation
        public override int GetHashCode()
		{
			return _hashCode;
		}

	    private static int GetHashCode(Part part)
		{
			unchecked
		    {
		        //var hashCode = part.MadeFrom.Any() ? part.MadeFrom.Aggregate(1, (current, p) => 31 * current + p.GetHashCode()) : 0;
		        var hashCode = 1;
		        hashCode += 1000000007 * part.ManufactureTime.GetHashCode();
		        hashCode += 1000000009 * part.Name.GetHashCode();
		        return hashCode;
            }
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Part);
		}
		
		public bool Equals(Part other)
		{
		    if (other == null) return false;
            return GetHashCode() == other.GetHashCode();
		}

		public static bool operator ==(Part lhs, Part rhs)
		{
		    if (ReferenceEquals(null, rhs)) return ReferenceEquals(lhs, null);
            return lhs.Equals(rhs);
		}

		public static bool operator !=(Part lhs, Part rhs) 
		{
			return !(lhs == rhs);
		}

		#endregion

	}
}