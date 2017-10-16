using System;
using System.Linq;
using System.Collections.Generic;

namespace ProductionSim
{

	public class Simulation : ISimulation
	{
	    private readonly IList<IBlock> _blocks;
	    private IBlock _currentBlock;
        private IPart _currentPart;

        public IList<IBlock> Blocks { get { return _blocks; } }

	    public IPart CurrentPart
	    {
	        get { return _currentPart; }
	        set
	        {
	            _currentPart = value;
	            _currentBlock = _blocks.First(b => b.ProducesParts.Any(p => p.Equals(value)));
	            _currentBlock.NextPart = value;
	        }
	    }

	    public Simulation()
		{
			
		}

        public void Tick()
        {
            
        }

        public void ResetState()
        {
            throw new NotImplementedException();
        }
    }
}
