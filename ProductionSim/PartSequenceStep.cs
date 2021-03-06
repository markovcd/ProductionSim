﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProductionSim
{
    public struct PartSequenceStep : IEnumerable<IPart>
    {
    	private readonly int _count;
    	private readonly IPart _part;
    	
    	public int Count { get { return _count; } }
        public IPart Part { get { return _part; } }

        public PartSequenceStep(IPart part, int count)
        {
            _count = count;
            _part = part;
        }

        public IEnumerator<IPart> GetEnumerator()
        {
            return Enumerable.Repeat(Part, Count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}