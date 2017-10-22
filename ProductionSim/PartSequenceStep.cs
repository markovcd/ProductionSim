using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProductionSim
{
    public class PartSequenceStep : IEnumerable<IPart>
    {
        public int Count { get; }
        public IPart Part { get; }

        public PartSequenceStep(IPart part, int count)
        {
            Count = count;
            Part = part;
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