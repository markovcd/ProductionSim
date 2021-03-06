﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProductionSim
{
    public class PartSequence : List<PartSequenceStep>, IPartSequence
    {
    	public PartSequenceStep NextStep { get { return this.LastOrDefault(); } }
        public PartSequenceStep CurrentStep { get; private set; }
        public int PartsLeft { get; private set; }
        public IEnumerable<IPart> Parts { get { return this.SelectMany(pss => pss); } }

        public IPart NextPart { get { return PartsLeft == 0 ? NextStep.Part : CurrentStep.Part; } }
        
        public PartSequence(IEnumerable<PartSequenceStep> parts = null) : 
        	base(parts ?? Enumerable.Empty<PartSequenceStep>()) { }

        public IPart TakePart()
        {
            if (PartsLeft == 0)
            {
                CurrentStep = NextStep;
                if (Count != 0) RemoveAt(Count - 1);
                PartsLeft = CurrentStep.Count;
            }

            if (PartsLeft == 0) return null;

            PartsLeft--;
            return CurrentStep.Part;
        }

		
    }
}